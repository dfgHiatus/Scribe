using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using Scribe.Enums;
using Scribe.Parsers;
using Scribe.Parsers.Whisper;
using System.IO;

namespace Scribe;

public class ScribeMod : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> UseXXLModel =
        new("useXXLModel", "Use WhisperXXL model. Requires restart.", () => false);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> ShowWhisperOutput = 
        new("showWhisperOutputInLogs", "Show Whisper's output in game.", () => false);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> Verbose =
        new("verbose", "Show Whisper's verbose logs.", () => false);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<Language> TransciptionLangauge =
        new("transciptionLangauge", "Transciption Langauge.", () => Language.en);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<Language> TranslationLangauge =
        new("translationLangauge", "Translation Langauge.", () => Language.en);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<Model> Model =
        new("model", "Model. Can be \"small\", \"medium\", or \"large\"", () => Enums.Model.medium);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<Device> Device =
        new("device", "Device. Can be \"cpu\" or \"cuda\".", () => Enums.Device.cpu);

    internal static readonly string ModelPath = Path.Combine(
        Engine.Current.AppPath,
        "rml_mods",
        "scribe");

    internal static readonly string CachePath = Path.Combine(
        Path.GetTempFileName());

    //internal static readonly string CachePath = Path.Combine(
    //    Engine.Current.CachePath,
    //    "Cache",
    //    "WhisperAudioCache");

    public override string Name => "Scribe";

    public override string Author => "dfgHiatus";

    public override string Version => "1.0.0";

    internal static ModConfiguration Config;
    internal static AbstractWhisperParser WhisperParser;

    public override void OnEngineInit()
    {
        Config = GetConfiguration();
        WhisperParser = Config.GetValue(UseXXLModel) ? 
            new FasterWhisperXXL(ModelPath, CachePath) : 
            new FasterWhisper(ModelPath, CachePath);
        new Harmony("net.dfgHiatus.Scribe").PatchAll();

        WhisperParser.OnDataReceived += OnDataReceived;
        WhisperParser.OnErrorReceived += OnErrorReceived;
    }
    private void OnDataReceived(string text)
    {
        UniLog.Log(text);
    }

    private void OnErrorReceived(string text)
    {
        UniLog.Error(text);
    }
}
