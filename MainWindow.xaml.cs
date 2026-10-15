using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable

namespace WordleApp
{
    public partial class MainWindow : Window
    {
        private int currentRow = 0;
        private int currentCol = 0;
        private string targetWord = "XXXXX";
        private List<string> fiveLetterWords = new List<string>();
        private List<string> incorrectLetters = new List<string>();
        private Border[,] tiles = new Border[6, 5];
        private TextBlock[,] textBlocks = new TextBlock[6, 5];
        private Dictionary<string, Button> keyboardButtons = new Dictionary<string, Button>();
        private bool gameOver = false;
        private DispatcherTimer toastTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            RegisterButtons(this);
            LoadWordsFromTxt();

            toastTimer = new DispatcherTimer();
            toastTimer.Interval = TimeSpan.FromSeconds(2);
            toastTimer.Tick += (s, e) =>
            {
                ToastBorder.Visibility = Visibility.Collapsed;
                toastTimer.Stop();
            };
        }

        private void InitializeBoard()
        {
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    var border = new Border
                    {
                        Width = 55,
                        Height = 55,
                        Margin = new Thickness(4),
                        Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#121213"),
                        BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#3a3a3c"),
                        BorderThickness = new Thickness(2),
                        Child = new TextBlock
                        {
                            Text = "",
                            Foreground = Brushes.White,
                            FontSize = 26,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    tiles[r, c] = border;
                    textBlocks[r, c] = (TextBlock)border.Child;
                    BoardGrid.Children.Add(border);
                }
            }
        }

        private void RegisterButtons(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                {
                    string key = null;
                    if (btn.Tag is string tag && !string.IsNullOrEmpty(tag))
                    {
                        key = tag.ToUpperInvariant();
                    }
                    else if (btn.Content is string content && !string.IsNullOrEmpty(content))
                    {
                        key = content.ToUpperInvariant();
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        keyboardButtons[key] = btn;
                    }
                }
                RegisterButtons(child);
            }
        }

        private void LoadWordsFromTxt()
        {
            try
            {
                string[] candidatePaths =
                {
                    Path.Combine(AppContext.BaseDirectory, "dictionary.txt"),
                    Path.Combine(Directory.GetCurrentDirectory(), "dictionary.txt"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "dictionary.txt"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "dictionary.txt"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "dictionary.txt")
                };

                string filePath = candidatePaths.FirstOrDefault(File.Exists) ?? Path.Combine(Directory.GetCurrentDirectory(), "dictionary.txt");

                if (!File.Exists(filePath))
                {
                    File.WriteAllLines(filePath, new string[] { "XXXXX" });
                }

                string fileContent = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                var matches = Regex.Matches(fileContent, @"(?<!\S)[A-Z]{5}(?!\S)");

                fiveLetterWords = new List<string>();
                foreach (Match match in matches)
                {
                    fiveLetterWords.Add(match.Value);
                }

                fiveLetterWords = fiveLetterWords.Distinct().ToList();

                if (fiveLetterWords.Count == 0)
                {
                    fiveLetterWords = new List<string> { "XXXXX" };
                }

                PickNewTargetWord();
            }
            catch
            {
                ShowToast("Błąd wczytywania pliku TXT!");
                fiveLetterWords = new List<string> { "XXXXX" };
                targetWord = "XXXXX";
            }
        }

        private void PickNewTargetWord()
        {
            if (fiveLetterWords.Count > 0)
            {
                var rnd = new Random();
                targetWord = fiveLetterWords[rnd.Next(fiveLetterWords.Count)];
            }
            else
            {
                targetWord = "XXXXX";
            }
        }

        private void ShowToast(string message)
        {
            toastTimer.Stop();
            ToastText.Text = message;
            ToastBorder.Visibility = Visibility.Visible;
            toastTimer.Start();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartScreenGrid.Visibility = Visibility.Collapsed;
            GameScreenGrid.Visibility = Visibility.Visible;
            this.Focus();
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            currentRow = 0;
            currentCol = 0;
            gameOver = false;
            incorrectLetters.Clear();
            EndGameGrid.Visibility = Visibility.Collapsed;

            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    textBlocks[r, c].Text = "";
                    tiles[r, c].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#121213");
                }
            }

            foreach (var btn in keyboardButtons.Values)
            {
                btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#818384");
            }

            PickNewTargetWord();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver || StartScreenGrid.Visibility == Visibility.Visible || EndGameGrid.Visibility == Visibility.Visible) return;

            if (e.Key == Key.Back)
            {
                DeleteLetter();
            }
            else if (e.Key == Key.Enter)
            {
                SubmitGuess();
            }
            else
            {
                string keyStr = e.Key.ToString();
                if (keyStr.Length == 1 && keyStr[0] >= 'A' && keyStr[0] <= 'Z')
                {
                    AddLetter(keyStr[0]);
                }
            }
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver || StartScreenGrid.Visibility == Visibility.Visible || EndGameGrid.Visibility == Visibility.Visible) return;
            if (sender is Button btn)
            {
                string tag = btn.Tag as string ?? btn.Content?.ToString();
                if (tag == "ENTER")
                {
                    SubmitGuess();
                }
                else if (tag == "BACK")
                {
                    DeleteLetter();
                }
                else if (!string.IsNullOrEmpty(tag) && tag.Length == 1)
                {
                    AddLetter(tag[0]);
                }
            }
        }

        private void AddLetter(char letter)
        {
            if (currentCol < 5)
            {
                textBlocks[currentRow, currentCol].Text = char.ToUpper(letter).ToString();
                currentCol++;
            }
        }

        private void DeleteLetter()
        {
            if (currentCol > 0)
            {
                currentCol--;
                textBlocks[currentRow, currentCol].Text = "";
            }
        }

        private void SubmitGuess()
        {
            if (currentCol < 5)
            {
                ShowToast("Słowo musi mieć 5 liter!");
                return;
            }

            string guess = "";
            for (int c = 0; c < 5; c++)
            {
                guess += textBlocks[currentRow, c].Text;
            }

            guess = guess.Trim().ToUpperInvariant();

            if (!fiveLetterWords.Contains(guess, StringComparer.OrdinalIgnoreCase))
            {
                ShowToast("Słowo nie istnieje w słowniku!");
                return;
            }

            char[] targetChars = targetWord.ToCharArray();
            char[] guessChars = guess.ToCharArray();
            var colors = new string[5];
            for (int i = 0; i < 5; i++) colors[i] = "#3a3a3c";

            bool[] targetMatched = new bool[5];
            bool[] guessMatched = new bool[5];

            for (int i = 0; i < 5; i++)
            {
                if (guessChars[i] == targetChars[i])
                {
                    colors[i] = "#538d4e";
                    targetMatched[i] = true;
                    guessMatched[i] = true;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (guessMatched[i]) continue;

                for (int j = 0; j < 5; j++)
                {
                    if (!targetMatched[j] && guessChars[i] == targetChars[j])
                    {
                        colors[i] = "#b59f3b";
                        targetMatched[j] = true;
                        guessMatched[i] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                tiles[currentRow, i].Background = (SolidColorBrush)new BrushConverter().ConvertFrom(colors[i]);

                string letterStr = guessChars[i].ToString();

                if (colors[i] == "#3a3a3c")
                {
                    if (!incorrectLetters.Contains(letterStr))
                    {
                        incorrectLetters.Add(letterStr);
                    }
                }

                if (keyboardButtons.ContainsKey(letterStr))
                {
                    var btn = keyboardButtons[letterStr];
                    string currentBg = btn.Background.ToString();

                    if (colors[i] == "#538d4e")
                    {
                        btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#538d4e");
                    }
                    else if (colors[i] == "#b59f3b" && currentBg != "#FF538D4E")
                    {
                        btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#b59f3b");
                    }
                }
            }

            ApplyIncorrectKeysBackground();

            if (guess == targetWord)
            {
                ShowToast("Gratulacje! Odgadłeś słowo!");
                gameOver = true;
                EndGameMessageText.Text = "GRATULACJE!\nOdgadłeś słowo dnia.";
                EndGameGrid.Visibility = Visibility.Visible;
                return;
            }

            currentRow++;
            currentCol = 0;

            if (currentRow == 6)
            {
                ShowToast($"Koniec gry! Słowo to: {targetWord}");
                gameOver = true;
                EndGameMessageText.Text = $"KONIEC GRY!\nSzukane słowo to: {targetWord}";
                EndGameGrid.Visibility = Visibility.Visible;
            }
        }

        private void ApplyIncorrectKeysBackground()
        {
            foreach (var kvp in keyboardButtons)
            {
                string letter = kvp.Key;
                Button btn = kvp.Value;

                if (incorrectLetters.Contains(letter))
                {
                    string currentBg = btn.Background.ToString();
                    if (currentBg != "#FF538D4E" && currentBg != "#FFB59F3B")
                    {
                        btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
                    }
                }
            }
        }
    }
}