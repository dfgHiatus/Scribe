using FrooxEngine;
using Scribe.Enums;
using System;
using System.Collections.Generic;

namespace Scribe.Parsers.Whisper;

internal class FasterWhisperXXL : AbstractWhisperParser
{
    public FasterWhisperXXL(string modelPath, string audioCache) : base(modelPath, audioCache)
    {
    }

    protected override string ExecutableName => Engine.Current.Platform == Platform.Windows ? "faster-whisper-xxl.exe" : "faster-whisper-xxl";

    protected override (List<string> arguments, string outputFile) ConstructArguments(string audioFile, Model model, Device device, OutputFormat outputFormat, bool verbose, TaskFormat task, Language language)
    {
        throw new NotImplementedException();
    }
}
