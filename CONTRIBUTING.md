# Contributing

Bug reports, ideas and pull requests are welcome. For anything beyond a small fix, open an issue first so the approach can be agreed.

## Setup

1. Fork the repository and clone your fork.
2. Install the **.NET 8 SDK**. KernClean targets `net8.0-windows`, so building and testing needs Windows.
3. Build and run the tests:

       dotnet build WinCleaner.sln -c Release
       dotnet test WinCleaner.sln

4. Try the CLI from source:

       dotnet run --project WinCleaner\WinCleaner.csproj -- help

CI runs build and tests on `windows-latest` for every pull request.

## Adding a command

Each subcommand is one `ICommand` class in `WinCleaner/Commands/`. Commands are discovered by reflection, so `Program.cs` does not change and the help text generates itself. Add a test in `WinCleaner.Tests/` for the logic behind it.

## Rules

- Destructive or modifying commands are a dry run by default. Real action needs `--no-dry-run` plus a confirmation (or `--yes`).
- Deletions go to the recycle bin. System changes must be reversible (backup and undo), with a restore point before machine-wide changes.
- `--json` output goes to stdout, diagnostics and prompts to stderr.
- No telemetry, no bundled software, no background service.
- The user-facing product name is KernClean. The `WinCleaner` names for the executables, namespaces and project files stay as they are.

## Pull requests

1. Create a branch from `master`.
2. Keep the change focused: one fix or feature per pull request.
3. Describe user-visible changes in [CHANGELOG.md](CHANGELOG.md).
4. Open the pull request and fill in the template. CI must be green before it is merged.

By contributing you agree that your contribution is licensed under the MIT License, see [LICENSE](LICENSE).
