using FrooxEngine;
using Scribe.Enums;
using System.Collections.Generic;

namespace Scribe.Parsers.Whisper;

internal class FasterWhisper : AbstractWhisperParser
{
    public FasterWhisper(string modelPath, string audioCache) : base(modelPath, audioCache)
    {
    }

    protected override string ExecutableName => Engine.Current.Platform == Platform.Windows ? "whisper-faster.exe" : "whisper-faster";

    protected override (List<string> arguments, string outputFile) ConstructArguments(string audioFile, Model model, Device device, OutputFormat outputFormat, bool verbose, TaskFormat task, Language language)
    {
        List<string> sb = new List<string>
        {
            audioFile,
            $"--device {device.EnumToString()}",
            $"--model {model.EnumToString()}",
            $"--output_format {outputFormat.EnumToString()}",
            $"--task {task.EnumToString()}",
            $"--language {language.EnumToString()}"
        };

        if (verbose)
        {
            sb.Add("--verbose");
        }

        return (sb, audioFile);
    }
}
