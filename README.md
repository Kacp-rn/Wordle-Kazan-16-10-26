# 🟩 Wordle (WPF + C#)

Klasyczna gra słowna **Wordle** zaimplementowana jako aplikacja desktopowa w technologii **WPF** (Windows Presentation Foundation) z użyciem języka **C#**. Projekt oferuje nowoczesny, ciemny interfejs, dynamiczne wczytywanie słownika z zewnętrznego pliku tekstowego oraz zaawansowaną obsługę klawiatury fizycznej i ekranowej.

---

## 🚀 Główne Funkcje

* **Ekran startowy i końcowy:** Przejrzyste menu główne ze spersonalizowaną sygnaturą oraz ekran końcowy z opcjami *Replay* i *Exit*.
* **Inteligentne parsowanie słownika:** Automatyczne wyszukiwanie 5-literowych słów zapisanych wyłącznie wielkimi literami z pliku `dictionary.txt` za pomocą wyrażeń regularnych (`Regex`).
* **Sterowanie dwukierunkowe:** Graj za pomocą klawiatury fizycznej lub klikając przyciski na wbudowanej klawiaturze ekranowej.
* **Zaawansowany system kolorowania klawiszy:**
  * 🟩 **Zielony:** Poprawna litera na właściwym miejscu (ma pierwszeństwo i pozostaje do końca rundy).
  * 🟧 **Pomarańczowy:** Poprawna litera na złej pozycji (może zostać nadpisana przez zielony, jeśli trafisz na właściwe miejsce).
  * ⬛ **Czarny:** Błędna litera niedostępna w szukanym słowie.
* **Powiadomienia Toast:** Płynne, niestandardowe powiadomienia ekranowe zastępujące tradycyjne okienka `MessageBox`.

---

## ⚠️ Uruchomienie i Plik Wykonywalny (.exe)

Repozytorium zawiera gotowy plik wykonywalny (`.exe`). Aby aplikacja działała poprawnie:
1. Upewnij się, że w **tym samym folderze, w którym znajduje się plik `.exe`**, umieszczony jest plik ze słownikiem **`dictionary.txt`** (w przeciwnym razie gra uruchomi się w trybie awaryjnym ze słowem `XXXXX`).
2. Na komputerze musi być zainstalowane odpowiednie środowisko **.NET Runtime** dla aplikacji WPF.

Możesz również otworzyć projekt w środowisku **Visual Studio** (zalecana wersja 2022 lub nowsza) i skompilować kod źródłowy samodzielnie.

---

## 📂 Struktura Projektu

* `MainWindow.xaml` oraz `MainWindow.xaml.cs` – główny interfejs użytkownika, obsługa zdarzeń klawiatury oraz cała logika gry i wczytywania słownika.
* `dictionary.txt` – zewnętrzny słownik tekstowy (np. format Project Gutenberg).

---

## 📝 Autor

Projekt stworzony przez **Kacper Nalepa**.
