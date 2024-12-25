namespace TMFChar.Gen.Util;

public static class ConsoleLogger
{
    public static void LogInfo(string message)
    {
        ChangeConsoleColor(
            ConsoleColor.Cyan,
            () =>
            {
                Console.WriteLine($"INFO: {message}");
            }
        );
    }

    public static void LogWarning(string message)
    {
        ChangeConsoleColor(
            ConsoleColor.Yellow,
            () =>
            {
                Console.WriteLine($"WARNING: {message}");
            }
        );
    }

    public static void LogError(string message)
    {
        ChangeConsoleColor(
            ConsoleColor.Red,
            () =>
            {
                Console.WriteLine($"ERROR: {message}");
            }
        );
    }

    private static void ChangeConsoleColor(ConsoleColor color, Action logAction)
    {
        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            logAction();
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }
}
