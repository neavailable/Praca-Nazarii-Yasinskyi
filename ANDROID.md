# Build Android

Otwórz projekt w Unity **2022.3.60f1** z modułami Android Build Support, Android SDK & NDK Tools oraz OpenJDK.

Wybierz **Build → Android APK**. Wynik zapisuje się w `Builds/Android/BallFinisher.apk`. Build używa IL2CPP oraz ARMv7 i ARM64 i zawiera wszystkie włączone sceny z Build Settings. To APK do lokalnej instalacji, podpisane domyślnym kluczem debug Unity.

Możesz też uruchomić w PowerShell w katalogu projektu:

```powershell
./Build-Android.ps1
```

Skrypt przygotowuje osobną kopię pod `Builds/AndroidWorkspace-*`, więc główny projekt może pozostać otwarty w Unity. Log znajduje się w `Builds/Android/build.log`. Kopię roboczą można usunąć po zakończonym buildzie; APK jest w osobnym katalogu. Pierwszy build potrzebuje internetu do pobrania zależności Android (Firebase i Appodeal).

Konfiguracje nadal są plikami JSON. Edytuj je w **`Assets/Resources/Configurations`**; Unity automatycznie pakuje je do APK. `link.xml` zachowuje konstruktory i pola konfiguracji, żeby odczyt Newtonsoft.Json działał również z IL2CPP.

Sceny nadal wybiera się w Inspectorze jako assety. Loader zapisuje również nazwę sceny dla aplikacji Android. Po zmianie nazwy sceny lub wyboru w Inspectorze zapisz scenę/prefab i sprawdź, że docelowa scena jest włączona w Build Settings.
