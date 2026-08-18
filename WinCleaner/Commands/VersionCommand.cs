using WinCleaner.Util;

namespace WinCleaner.Commands;

public sealed class VersionCommand : ICommand
{
    public string Name => "version";
    public string Summary => "Versionsnummer anzeigen";
    public string Usage => "";

    public int Execute(CommandContext ctx)
    {
        Console.WriteLine($"KernClean {AppInfo.Version}");
        return 0;
    }
}
