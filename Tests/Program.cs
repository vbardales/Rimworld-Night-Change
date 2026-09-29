using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NightChange;
using Verse;

internal static class Program
{
    private static int checks;
    private static int Main(string[] args)
    {
        if (args.Length != 2) { Console.Error.WriteLine("Usage: NightChange.Tests.exe <game Managed directory> <temporary output directory>"); return 2; }
        AppDomain.CurrentDomain.AssemblyResolve += (sender, request) =>
        {
            var path = Path.Combine(args[0], new AssemblyName(request.Name).Name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };
        try { Run(args[1]); Console.WriteLine($"PASS: {checks} assertions; real mod assembly and game Scribe, no game process."); return 0; }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Run(string output)
    {
        Directory.CreateDirectory(output);
        var defaults = new NightChangeSettings();
        Check(defaults.inheritOwnerFromBed && defaults.coldGuard && defaults.coldGuardMargin == 2 && defaults.maxStandDistance == 12, "Clean defaults");
        foreach (float value in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            defaults.coldGuardMargin = value; defaults.Normalize();
            Check(defaults.coldGuardMargin == 2, "Non-finite margin fallback");
        }
        defaults.coldGuardMargin = -5; defaults.maxStandDistance = int.MinValue; defaults.Normalize();
        Check(defaults.coldGuardMargin == 0 && defaults.maxStandDistance == 3, "Lower bounds");
        defaults.coldGuardMargin = 99; defaults.maxStandDistance = int.MaxValue; defaults.Normalize();
        Check(defaults.coldGuardMargin == 10 && defaults.maxStandDistance == 40, "Upper bounds");
        Check(TemperatureGuard.AllowsTemperature(-10, 0, 12, 2), "Exact cold threshold permitted");
        Check(!TemperatureGuard.AllowsTemperature(-10, 0, 11.9f, 2), "Below threshold refused");
        Check(TemperatureGuard.AllowsTemperature(-10, 0, 11.9f, 0), "Margin changes actual decision");
        Check(TemperatureGuard.AllowsTemperature(0, 0, -100, 2), "No insulation loss permitted");

        var path = Path.Combine(output, "settings.xml");
        var settings = new NightChangeSettings { inheritOwnerFromBed = false, coldGuard = false, coldGuardMargin = 7, maxStandDistance = 31 };
        Scribe.saver.InitSaving(path, "settings"); settings.ExposeData(); Scribe.saver.FinalizeSaving();
        var restored = Load(path);
        Check(!restored.inheritOwnerFromBed && !restored.coldGuard && restored.coldGuardMargin == 7 && restored.maxStandDistance == 31, "Scribe round trip");
        File.WriteAllText(path, "<settings />"); restored = Load(path);
        Check(restored.inheritOwnerFromBed && restored.coldGuard && restored.coldGuardMargin == 2 && restored.maxStandDistance == 12, "Missing values use defaults");
        File.WriteAllText(path, "<settings><coldGuardMargin>99</coldGuardMargin><maxStandDistance>-5</maxStandDistance></settings>"); restored = Load(path);
        Check(restored.coldGuardMargin == 10 && restored.maxStandDistance == 3, "Loaded out-of-range values normalized");
    }

    private static NightChangeSettings Load(string path)
    {
        var settings = new NightChangeSettings();
        Scribe.loader.InitLoading(path); settings.ExposeData();
        Scribe.mode = LoadSaveMode.PostLoadInit; settings.ExposeData();
        Scribe.ForceStop();
        return settings;
    }
}
