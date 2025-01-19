System Zarządzania Zadaniami
Opis projektu
Aplikacja internetowa umożliwiająca zarządzanie zadaniami w organizacji. Projekt wykorzystuje technologie .NET 8 oraz Entity Framework Core,
a jego funkcjonalności obejmują m.in. tworzenie, edycję, usuwanie oraz dynamiczne przypisywanie zadań.

Wykorzystane biblioteki NuGet

Microsoft.AspNetCore.Identity.EntityFrameworkCore
Integracja Identity z Entity Framework Core w ASP.NET Core.
Wersja: 8.0.12.

Microsoft.EntityFrameworkCore
Narzędzie ORM do zarządzania bazą danych, obsługujące SQL Server, SQLite i inne.
Wersja: 9.0.1.

Microsoft.EntityFrameworkCore.Sqlite
Provider SQLite dla Entity Framework Core.
Wersja: 9.0.1.

Microsoft.EntityFrameworkCore.Tools
Narzędzia wspierające pracę z migracjami w Entity Framework Core.
Wersja: 9.0.1.

Jak uruchomić projekt?
Otwórz projekt w Visual Studio.
Upewnij się, że wszystkie wymagane biblioteki są zainstalowane (pakiety NuGet wymienione powyżej).
Skonfiguruj bazę danych (domyślnie SQLite) w pliku appsettings.json.
Uruchom następujące polecenia w terminalu projektu: dotnet ef database update
Uruchom projekt za pomocą zielonego przycisku w Visual Studio.

Najważniejsze funkcjonalności
Zarządzanie zadaniami (tworzenie, edycja, usuwanie).
Automatyczne powiadomienia o nadchodzących terminach.
Historia zmian zadań, logowania i rejestracji.
Dynamiczne raporty i przypisywanie zadań do innych użytkowników.
