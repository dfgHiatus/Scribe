using Elements.Assets;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using Scribe.Enums;
using SkyFrost.Base;
using System.IO;
using System.Threading.Tasks;

namespace Scribe.Patches;

[HarmonyPatch(typeof(StaticAudioClip), "BuildInspectorUI", typeof(UIBuilder))]
public class StaticAudioClipPatch
{
    public static World CurrentWorld { get => Engine.Current.WorldManager.FocusedWorld; }

    public static void Postfix(StaticAudioClip __instance, UIBuilder ui)
    {
        // Title
        ui.Text("Scribe by dfgHiatus", bestFit: true, Alignment.MiddleLeft);
        ui.Text("You can change Scribe settings in your mod config.", bestFit: true, Alignment.MiddleLeft);
        ui.Text("Transcribe:", bestFit: true, Alignment.MiddleLeft);

        // First row
        ui.HorizontalLayout(4f);
        var t1 = ui.Button("Transcribe to TXT");
        t1.LocalPressed += (button, _) =>
        {
            TranscribeToText(__instance.Slot);
        };
        var t2 = ui.Button("Transcribe to JSON");
        t2.LocalPressed += (button, _) =>
        {
            TranscribeToJson(__instance.Slot);
        };
        var t3 = ui.Button("Transcribe to SRT");
        t3.LocalPressed += (button, _) =>
        {
            TranscribeToSrt(__instance.Slot);
        };
        ui.NestOut();

        // Second row
        ui.HorizontalLayout(4f);
        var t4 = ui.Button("Transcribe to LRC");
        t4.LocalPressed += (button, _) =>
        {
            TranscribeToLrc(__instance.Slot);
        };
        var t5 = ui.Button("Transcribe to VTT");
        t5.LocalPressed += (button, _) =>
        {
            TranscribeToVtt(__instance.Slot);
        };
        var t6 = ui.Button("Transcribe to TSV");
        t6.LocalPressed += (button, _) =>
        {
            TranscribeToTsv(__instance.Slot);
        };
        ui.NestOut();

        // Third row
        ui.Text("Translate:", bestFit: true, Alignment.MiddleLeft);
        ui.HorizontalLayout(4f);
        var t7 = ui.Button("Translate to TXT");
        t7.LocalPressed += (button, _) =>
        {
            TranslateToText(__instance.Slot);
        };
        var t8 = ui.Button("Translate to JSON");
        t8.LocalPressed += (button, _) =>
        {
            TranslateToJson(__instance.Slot);
        };
        var t9 = ui.Button("Translate to SRT");
        t9.LocalPressed += (button, _) =>
        {
            TranslateToSrt(__instance.Slot);
        };
        ui.NestOut();

        // Fourth row
        ui.HorizontalLayout(4f);
        var t10 = ui.Button("Translate to LRC");
        t10.LocalPressed += (button, _) =>
        {
            TranslateToLrc(__instance.Slot);
        };
        var t11 = ui.Button("Translate to VTT");
        t11.LocalPressed += (button, _) =>
        {
            TranslateToVtt(__instance.Slot);
        };
        var t12 = ui.Button("Translate to TSV");
        t12.LocalPressed += (button, _) =>
        {
            TranslateToTsv(__instance.Slot);
        };
        ui.NestOut();
    }

    private static void TranslateToTsv(Slot slot)
    {
        
    }

    private static void TranslateToVtt(Slot slot)
    {
        
    }

    private static void TranslateToLrc(Slot slot)
    {
        
    }

    private static void TranslateToSrt(Slot slot)
    {
        
    }

    private static void TranslateToJson(Slot slot)
    {
        
    }

    private static void TranslateToText(Slot slot)
    {
        slot.StartGlobalTask(async () =>
        {
            await TranslateTranscribe(slot, OutputFormat.txt, TaskFormat.translate);
        });
    }

    private static void TranscribeToTsv(Slot slot)
    {
        
    }

    private static void TranscribeToVtt(Slot slot)
    {

    }

    private static void TranscribeToLrc(Slot slot)
    {
        
    }

    private static void TranscribeToSrt(Slot slot)
    {
        
    }

    private static void TranscribeToJson(Slot slot)
    {
        
    }

    private static void TranscribeToText(Slot slot)
    {
        slot.StartGlobalTask(async () =>
        {
            await TranslateTranscribe(slot, OutputFormat.txt, TaskFormat.transcribe);
        });
    }

    private static async Task TranslateTranscribe(Slot slot, OutputFormat outputFormat, TaskFormat taskFormat)
    {
        // Set up the progress indicator
        await default(ToWorld);
        UniLog.Log("2");
        var progressSlot = CurrentWorld.RootSlot.AddSlot("Scribe Import Slot", persistent: false);
        progressSlot.PositionInFrontOfUser();
        UniLog.Log("3");
        var pbi = await progressSlot.SpawnEntity<ProgressBarInterface, LegacySegmentCircleProgress>(FavoriteEntity.ProgressBar); // Needs to be run in World thread
        UniLog.Log("4");
        pbi?.Initialize(canBeHidden: false);                                                                                     // Mark this nullable in case the user destroys the progress bar during the import
        UniLog.Log("5");
        ScribeMod.WhisperParser.OnDataReceived += (string message) => pbi?.UpdateProgress(0f, message, string.Empty);            // Will this leak??
        ScribeMod.WhisperParser.OnErrorReceived += (string message) => pbi?.ProgressFail(message);

        // Save the AudioX file as a WAV file on disk
        UniLog.Log("6");
        pbi.UpdateProgress(0f, "Saving audio file to disk...", string.Empty);
        UniLog.Log("7");
        var tempAudioFile = Path.GetTempFileName();
        tempAudioFile = Path.ChangeExtension(tempAudioFile, "wav");
        var audioAsset = slot.GetComponentInParents<StaticAudioClip>().Asset;
        UniLog.Log("8");

        await default(ToBackground);
        UniLog.Log("9");
        var audioData = await audioAsset.GetOriginalAudioData();
        audioData.Save(tempAudioFile);
        UniLog.Log("10");

        // Do the work
        pbi.UpdateProgress(0f, "Starting work...", string.Empty);
        var tempTranscribedTextFile = await ScribeMod.WhisperParser.Transcribe(
            tempAudioFile,
            ScribeMod.Config.GetValue(ScribeMod.Model),
            ScribeMod.Config.GetValue(ScribeMod.Device),
            outputFormat,
            ScribeMod.Config.GetValue(ScribeMod.Verbose),
            ScribeMod.Config.GetValue(ScribeMod.ShowWhisperOutput),
            taskFormat,
            ScribeMod.Config.GetValue(ScribeMod.TransciptionLangauge));
        UniLog.Log("11");

        pbi?.ProgressDone("Finished!");
        pbi?.UpdateProgress(1f, "Finished!", string.Empty);
        await ImportTextFile(tempTranscribedTextFile);

        await default(ToBackground);
        File.Delete(tempAudioFile);
    }

    private static async Task ImportTextFile(string tempTranscribedTextFile)
    {
        await default(ToWorld);
        CurrentWorld.LocalUser.GetPointInFrontOfUser(out var point, out var rotation);
        UniversalImporter.Import(
            AssetClass.Text,
            new string[] { tempTranscribedTextFile },
            CurrentWorld,
            point,
            rotation);

        await default(ToBackground);
        File.Delete(tempTranscribedTextFile);
    }
}
