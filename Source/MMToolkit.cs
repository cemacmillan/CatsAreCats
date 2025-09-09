using Verse;

namespace SlippersNmSpc;

public static class MMToolkit
{
    private static readonly System.Collections.Generic.HashSet<string> GripeMessages = new();
    private static int GripeKeyCounter = 0;

        

    public static void DebugLog(string message)
    {
        if (!SlippersMod.Settings.EnableLogging) return; // Respect the logging setting

        Log.Message($"[Slippers] {message}");
    }
        
    public static void DebugWarn(string message)
    {
        if (!SlippersMod.Settings.EnableLogging) return; // Respect the logging setting

        Log.Warning($"[Slippers] {message}");
    }

    public static void GripeOnce(string message)
    {
        if (!SlippersMod.Settings.EnableLogging) return; // Respect the logging setting

        int gripeKey = GetGripeKey(message);
        Log.WarningOnce($"[Slippers] {message}", gripeKey);
    }

    private static int GetGripeKey(string message)
    {
        // Generate or retrieve a unique key for the message
        if (!GripeMessages.Contains(message))
        {
            GripeMessages.Add(message);
            GripeKeyCounter++;
        }

        return message.GetHashCode();
    }
}