# Dokumentacja Projektu - Food portal
Wprowadzenie
Projekt Food Portal to aplikacja internetowa stworzona w ASP.NET Core MVC, której celem jest zarządzanie przepisami użytkowników użytkowników. Użytkownicy mogą tworzyć, edytować oraz usuwać przepis. Następnie stworzone (lub wcześniej gotowe) przepisy, zarówne własne jak i cudze, mogą dodawać do wybranego planu posiłków, ustawiając przy tym także inne parametry dotyczące planu, jak np. dzień w który będą spożywać dany posiłek. Można go również edytować, zobczyć szczegóły, a także usunąć. Dodatkową funkcjonalnością jest filtrowanie przepisów po ocenach.

## Instalacja
Sklonuj repozytorium na swój lokalny komputer:
```
git clone [adres-repozytorium]
```
Otwórz repozytorium w wybranym edytorze kodu (Visual Studio, Visual Studio Code).
Następnie do terminala wpisz następujące polecenia, aby przejść do docelowego folderu:
```
cd [path to directory]
```
Uruchom aplikację:
```
dotnet run
```
  
## Funkcjonalności
### Użytkownicy
- Rejestracja nowych użytkowników
- Logowanie użytkowników
- Wylogowywanie użytkowników

#### Administrator:
- zarządzanie użytkownikami
- dodawanie kategorii


### Przepisy
- Tworzenie nowych przepisów
- Edytowanie istniejących przepisów
- Usuwanie przepisów
- Filtrowanie przepisów po najwyżej ocenianych

### Plany przepisów
- Tworzenie nowych planów
- Edytowanie planów
- Usuwanie planów
- Przypisywanie przepisów do planu


## Przykład użycia
Podczas pierwszego uruchomienia aplikacji tworzy się domyślny użytkownik administratora. Dane logowania:

Username: admin
Password: admin
Admin ma możliwość dodawania użytkowników.

Po zalogowaniu przejdź do zakładki "Add Recipe". Strona przeniesie cię na nowy widok formularza do wypełnienia danymi przepisu. Stworzony przepis możesz edytować przyciskiem "Edit" lub podejrzeć szczegóły poprzez przycis "Details".
Możesz następnie przejść do zakładki Meal Plans, gdzie masz możliwość stworzyć nowy plan klikając "Create New Plan". Wybierz datę początku planu. Następnie będziesz mógł dodawać przepisy do tego planu.
