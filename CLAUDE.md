# KernClean

Windows-Cleaner (eigener CCleaner-Ersatz), Marke "KernClean" (Dachmarke "Kern"; Repo-Slug `kern-clean`). Technischer Unterbau (Projektordner, Assembly-/Exe-Namen, Namespaces) heißt weiterhin `WinCleaner` — siehe Hinweis unten. C# / .NET 8 (`net8.0-windows`), drei Projekte in `WinCleaner.sln`: CLI (`WinCleaner\`), WPF-GUI (`WinCleaner.Gui\`, MVVM, ruft die CLI auf), xUnit-Tests (`WinCleaner.Tests\`).

**Dieses Verzeichnis ist der Projekt-Root** (Git-Repo, Solution). Sessions hier starten — nicht im äußeren `Projekte\WinCleaner\` (das enthält nur `dist\`-Kopien).

## Befehle (aus diesem Verzeichnis)

```sh
dotnet build WinCleaner.sln -c Release          # Build
dotnet test WinCleaner.sln                      # Tests (xUnit, ~101 Tests)
dotnet run --project WinCleaner\WinCleaner.csproj -- menu    # CLI dev-run
dotnet run --project WinCleaner.Gui\WinCleaner.Gui.csproj    # GUI dev-run
```

Publish (self-contained single-file win-x64):
```sh
dotnet publish WinCleaner/WinCleaner.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

## Verifizieren nach Änderungen (Pflicht)

Clemens benutzt die **installierte** Version unter `%LOCALAPPDATA%\Programs\WinCleaner` (auf PATH) bzw. die Kopien im äußeren `dist\` / `dist-gui\`. Ein erfolgreicher Build ändert daran NICHTS — nach jeder Änderung die frisch gebauten Exes dorthin kopieren und die App real starten, sonst sieht er weiter die alte Version (bekannte Fehlerquelle: „steht 2.0, aber immer noch alte Ansicht").

## Version & Release

- Versionsnummer steht **doppelt**: `WinCleaner\WinCleaner.csproj` UND `WinCleaner.Gui\WinCleaner.Gui.csproj` (je `<Version>`-Block) — immer beide bumpen, plus `CHANGELOG.md` (Keep a Changelog / SemVer).
- Release = `v*`-Tag pushen → GitHub Actions (`.github\workflows\ci.yml`) baut, testet und erstellt das GitHub-Release mit Exe. Remote: `github.com/clemensjl/kern-clean` (bis 2026-08-19: `WinCleaner`, per `gh repo rename` umbenannt), Branch `master`.

## Architektur-Konventionen

- **Neues Subcommand = eine `ICommand`-Klasse in `Commands\`** — Auto-Discovery per Reflection (`CommandRegistry`), `Program.cs` nie anfassen, Help generiert sich selbst.
- `Core\` = Scan/Clean-Logik, `SystemTools\` = OS-Eingriffe (Registry, Dienste, Restore Points), `Util\` = Ausgabe/Prompts.
- GUI läuft `asInvoker`; Admin-Aktionen laufen über CLI-Relaunch via `runas` (`Elevation.cs`, `ElevatedCli.cs`). Diesen Weg nutzen, keine GUI-weite Elevation einführen.
- `--json`: Maschinenausgabe auf stdout, Diagnose auf stderr — nicht mischen.

## Sicherheitsmodell (nicht aufweichen)

Jedes destruktive Kommando ist **Dry-Run by default**; echt löschen nur mit `--no-dry-run` + Bestätigung (`--yes`). Löschen in Papierkorb, Registry-Tweaks reversibel (`TweakEngine`, JSON-Backup), Restore Point vor Systemänderungen. Nur `shred`/`wipe-free-space` sind irreversibel.

## Branding (KernClean, Stand 2026-08-19)

Sichtbarer Produktname ist **KernClean** (README, ANLEITUNG, `--version`/`help`-Banner,
GUI-Fenstertitel/Logo, `<Product>` in beiden .csproj). Bewusst **nicht** umbenannt, weil
es bestehende Installationen bricht: Exe-/Assembly-Namen (`WinCleaner.exe`,
`WinCleanerGui.exe`), C#-Namespaces (`WinCleaner.*`), Projekt-/Solution-Dateien
(`WinCleaner.sln`, `WinCleaner.csproj`, …), der `%LOCALAPPDATA%\Programs\WinCleaner`-
Installationsordner, `%LOCALAPPDATA%\WinCleaner\{tweaks,hash-cache.json}`, die
Scheduled-Task-Namen `WinCleaner Auto-Clean` / `WinCleaner Privacy-Reapply` /
`WinCleaner Auto-Update`, die hosts-Marker `# === WinCleaner Telemetrie-Block ... ===`
und die Desktop-Verknüpfungen (`WinCleaner.lnk`, `WinCleaner (Terminal).lnk`) auf Clemens'
Rechner. CLI-Beispielzeilen in README/ANLEITUNG (`WinCleaner scan-junk` etc.) sind
deshalb absichtlich unverändert geblieben — sie sind funktionale Syntax, kein
Branding. Ein echter technischer Rename (Exe/Assembly/Namespace) ist ein separates,
bewusst zu entscheidendes Vorhaben (bricht Task Scheduler + Shortcuts, braucht
Migration/Reinstall).

## Gotchas

- **Reparse Points überspringen:** Clemens' Documents-Ordner enthält versteckte Junctions (Spiele-Saves nach Reorg). Scanner/Cleaner dürfen Junctions nie folgen oder als Duplikate/Junk werten.
- GUI-Publish braucht `IncludeNativeLibrariesForSelfExtract` + `IncludeAllContentForSelfExtract`, sonst `DllNotFoundException` beim Start (Kommentar im Gui-csproj).
- Stale `bin\Debug\net9.0`-Ordner ignorieren — Target ist net8.0-windows.
