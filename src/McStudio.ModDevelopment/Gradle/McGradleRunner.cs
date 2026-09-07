using System.Diagnostics;
using System.Text;

namespace McStudio.ModDevelopment.Gradle;

/// <summary>
/// Ported from CCS GradleUtils.java + GradleDaemonUtils.java
/// Manages Gradle processes for building Minecraft mods.
/// </summary>
public class McGradleRunner : IDisposable
{
    private readonly string _workspaceFolder;
    private Process? _currentProcess;

    public McGradleRunner(string workspaceFolder)
    {
        _workspaceFolder = workspaceFolder;
    }

    public string WorkspaceFolder => _workspaceFolder;

    /// <summary>
    /// Check if Gradle wrapper exists in the workspace
    /// </summary>
    public bool HasGradleWrapper()
    {
        return File.Exists(Path.Combine(_workspaceFolder, "gradlew")) ||
               File.Exists(Path.Combine(_workspaceFolder, "gradlew.bat"));
    }

    /// <summary>
    /// Run a Gradle task
    /// </summary>
    public async Task<McGradleResult> RunTask(string task, int timeoutMs = 300000)
    {
        var isWindows = OperatingSystem.IsWindows();
        var gradlew = isWindows ? "gradlew.bat" : "./gradlew";
        var shell = isWindows ? "cmd" : "bash";
        var shellArgs = isWindows ? $"/c {gradlew} {task}" : $"-c \"{gradlew} {task}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = shell,
            Arguments = shellArgs,
            WorkingDirectory = _workspaceFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8,
        };

        // Set environment variables
        startInfo.EnvironmentVariables["GRADLE_OPTS"] = "-Xmx2g -Xms512m";

        var output = new StringBuilder();
        var error = new StringBuilder();

        _currentProcess = new Process { StartInfo = startInfo };
        _currentProcess.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                output.AppendLine(e.Data);
                Debug.WriteLine($"[Gradle] {e.Data}");
            }
        };
        _currentProcess.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                error.AppendLine(e.Data);
                Debug.WriteLine($"[Gradle ERR] {e.Data}");
            }
        };

        _currentProcess.Start();
        _currentProcess.BeginOutputReadLine();
        _currentProcess.BeginErrorReadLine();

        var exited = _currentProcess.WaitForExit(timeoutMs);

        if (!exited)
        {
            _currentProcess.Kill(entireProcessTree: true);
            return new McGradleResult(-1, output.ToString(), error.ToString(), true);
        }

        return new McGradleResult(_currentProcess.ExitCode, output.ToString(), error.ToString(), false);
    }

    /// <summary>
    /// Build the mod (runs gradle build)
    /// </summary>
    public async Task<McGradleResult> Build()
    {
        return await RunTask("build");
    }

    /// <summary>
    /// Clean the build
    /// </summary>
    public async Task<McGradleResult> Clean()
    {
        return await RunTask("clean");
    }

    /// <summary>
    /// Run the Eclipse IDE task
    /// </summary>
    public async Task<McGradleResult> Eclipse()
    {
        return await RunTask("eclipse");
    }

    /// <summary>
    /// Run the IDEA task
    /// </summary>
    public async Task<McGradleResult> Idea()
    {
        return await RunTask("idea");
    }

    public void Dispose()
    {
        _currentProcess?.Dispose();
    }
}

/// <summary>
/// Represents the result of a Gradle build task.
/// Ported from CCS GradleResultCode.java
/// </summary>
public class McGradleResult
{
    public int ExitCode { get; }
    public string Output { get; }
    public string Error { get; }
    public bool TimedOut { get; }

    public McGradleResult(int exitCode, string output, string error, bool timedOut)
    {
        ExitCode = exitCode;
        Output = output;
        Error = error;
        TimedOut = timedOut;
    }

    public bool IsSuccess => ExitCode == 0 && !TimedOut;

    public string? GetJarPath()
    {
        if (!IsSuccess) return null;

        var buildDir = "build/libs";
        if (!Directory.Exists(buildDir))
            return null;

        return Directory.GetFiles(buildDir, "*.jar")
            .OrderByDescending(f => new FileInfo(f).LastWriteTime)
            .FirstOrDefault();
    }
}