# UnitTestRunnerMVVM
# 🧪 NeuUITest – Ein MVVM-basiertes WPF-Projekt

**NeuUITest** ist eine moderne WPF-Anwendung zur Ausführung und Visualisierung von .NET-Unit-Tests.  
Gleichzeitig dient dieses Projekt als **praxisnahes Beispiel**, wie man die **MVVM-Architektur (Model-View-ViewModel)** sauber und effektiv in WPF-Anwendungen implementiert.

---

## 📌 Ziel dieses Projekts

- ✅ Zeigt die **korrekte Trennung von Logik und UI** mit MVVM
- ✅ Unterstützt das **asynchrone Ausführen von Unit-Tests**
- ✅ Verwendet **Command-Binding**, `INotifyPropertyChanged`, `RelayCommand` und `IProgress<T>`
- ✅ Visualisiert **Test-Ausgaben und Ergebnisse live**

---

## 🧠 Was ist MVVM?

### MVVM = **Model – View – ViewModel**

| Ebene         | Aufgabe                                                                 |
|---------------|-------------------------------------------------------------------------|
| **Model**     | Stellt Datenstrukturen & Anwendungslogik bereit                         |
| **View**      | Präsentationsebene (XAML), bindet an ViewModel, keine Logik             |
| **ViewModel** | Vermittelt zwischen View und Model, enthält Befehle und Zustände        |

### Warum MVVM?

- Trennung von UI und Logik
- Einfach zu testen und zu warten
- Unterstützt Data Binding und Commands
- Keine Logik im Code-Behind notwendig
- Erleichtert Teamarbeit (UI-Designer & Entwickler getrennt)

---

## 📁 Projektstruktur & Bedeutung der Ordner

Das Projekt ist streng nach MVVM-Prinzipien strukturiert:

### `Helpers/`
Hilfsklassen zur MVVM-Umsetzung.

- **`RelayCommand.cs`**  
  Ermöglicht Command-Bindings in ViewModels (`ICommand`). Unterstützt sowohl synchrone als auch asynchrone Befehle.

📌 **Nutzen**: Trennung von UI und Logik durch Commands statt Event-Handler.

---

### `Kern/` (Model)

Hier liegt die Geschäftslogik und Datenstruktur.

- **`TestResult.cs`**  
  Modell für ein Testergebnis inkl. Statusfarbe für UI.
  
- **`TestRunner.cs`**  
  Führt `vstest.console.exe` aus, liest und parst die Ausgabe asynchron.

- **`About/ProgramInfo.cs`**  
  Enthält Metadaten wie App-Name, Version und Autor.

📌 **Nutzen**: Reine Logik- und Datenklassen – völlig unabhängig von UI. Ideal testbar und wiederverwendbar.

---

### `Services/` (Service-Schicht)

Dient zur Abstraktion von externen Vorgängen oder UI-nahem Verhalten.

- **Interfaces**:
  - `ITestExecutionService.cs`: Abstraktion der Testlogik
  - `IWindowService.cs`: Zeigt Fenster (z. B. About-Dialog)

- **Implementierungen**:
  - `TestExecutionService.cs`: Nutzt `TestRunner`, um Tests auszuführen
  - `WindowService.cs`: Öffnet das About-Fenster

📌 **Nutzen**: ViewModels bleiben frei von direkter UI-Interaktion.

---

### `UI/` (View)

XAML-Dateien zur Gestaltung der Benutzeroberfläche.  
**Keine Logik in Code-Behind!** Alles wird über Bindings gesteuert.

- **`MainWindow.xaml`**: Hauptfenster mit UI-Elementen für Dateiauswahl, Testausführung, Terminalausgabe und Zusammenfassung.
- **`AboutWindow.xaml`**: Zeigt App-Informationen aus dem ViewModel.
- **`TestSummaryControl.xaml`**: Wiederverwendbare UI-Komponente für die Test-Zusammenfassung.

📌 **Nutzen**: Die Views zeigen nur Daten an, die vom ViewModel bereitgestellt werden.

---

### `ViewModels/` (ViewModel)

Verbindet View ↔ Model, verarbeitet Benutzerinteraktionen und enthält UI-Zustände.

- **Basisklasse**:
  - `MainWindowBase.cs`: Implementiert `INotifyPropertyChanged`

- **Haupt-ViewModel**:
  - `MainMainWindow.cs`: Komponiert alle Sub-ViewModels

- **Sub-ViewModels**:
  - `RunTestsMainWindow.cs`: Führt Tests aus, aktualisiert UI
  - `BrowseDllMainWindow.cs`: Öffnet Datei-Dialog für DLL
  - `MainTestSummaryControl.cs`: Zählt Passed/Failed/Total
  - `AboutAboutWindow.cs`: Command für About-Fenster
  - `MainAboutWindow.cs`: Stellt AppInfo bereit

📌 **Nutzen**: Alle ViewModels sind lose gekoppelt, testbar, wiederverwendbar und UI-unabhängig.

---

## 🔁 MVVM-Datenfluss – Beispiel Testausführung

```text
[Benutzer klickt "Run Tests"]
        ↓
RelayCommand → RunTestsMainWindow.RunTestsAsync()
        ↓
TestExecutionService ruft TestRunner auf
        ↓
vstest.console.exe wird gestartet (async)
        ↓
Progress<string> aktualisiert ConsoleOutput
        ↓
Testergebnisse (TestResult) werden in SummaryResults übernommen
        ↓
MainTestSummaryControl berechnet Zählwerte
        ↓
UI aktualisiert sich automatisch über DataBinding