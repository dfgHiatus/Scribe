using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Scribe;

internal static class Utils
{
    private static readonly MD5 hasher = MD5.Create();

    /// <summary>
    /// https://stackoverflow.com/questions/470256/process-waitforexit-asynchronously
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
    /// immediately as canceled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    internal static Task WaitForExitAsync(this Process process,
        CancellationToken cancellationToken = default)
    {
        if (process.HasExited) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(null);
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.SetCanceled());

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }

    public static string EnumToString(this Enum e)
    {
        return Enum.GetName(e.GetType(), e);
    }


    internal static bool ContainsUnicodeCharacter(string input)
    {
        const int MaxAnsiCode = 255;
        return input.Any(c => c > MaxAnsiCode);
    }

    internal static string GenerateMD5(string filepath)
    {
        // Credit to delta for this method https://github.com/XDelta/
        var stream = File.OpenRead(filepath);
        var hash = hasher.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "");
    }
}

