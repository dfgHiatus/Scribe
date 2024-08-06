using FrooxEngine;
using Scribe.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Scribe.Parsers;

internal abstract class AbstractWhisperParser
{
    public event Action<string> OnDataReceived;
    public event Action<string> OnErrorReceived;

    private readonly string ModelPath;
    private readonly string AudioCache;

    public AbstractWhisperParser(string modelPath, string audioCache) 
    {
        ModelPath = modelPath;
        AudioCache = audioCache;
    }

    protected abstract string ExecutableName { get; }

    protected abstract (List<string> arguments, string outputFile) ConstructArguments(string audioFile, Model model, Device device, OutputFormat outputFormat, bool verbose, TaskFormat task, Language language);

    public async Task<string> Transcribe(string audioFile, Model model, Device device, OutputFormat outputFormat, bool verbose, bool log, Enums.TaskFormat task, Language language)
    {
        await default(ToBackground);

        (List<string> arguments, string outputFile) output = ConstructArguments(audioFile, model, device, outputFormat, verbose, task, language);
        output.arguments.Add($"--model_dir {ModelPath}");
        output.arguments.Add($"--output_dir {AudioCache}");

        var process = new Process();
        process.StartInfo = new ProcessStartInfo(string.Join(" ", output.arguments))
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        if (log)
        {
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;
        }
        process.Start();

        await process.WaitForExitAsync();

        return Path.Combine(Path.GetTempPath(), output.outputFile);
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        OnDataReceived?.Invoke(e.Data);
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        OnErrorReceived?.Invoke(e.Data);
    }
}
