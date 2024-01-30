using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

internal static class HideAppHelper
{

    //const uint WDA_NONE = 0x00000000;
    //const uint WDA_MONITOR = 0x00000001;
    //const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

    //[DllImport("user32.dll")]
    //public static extern uint SetWindowDisplayAffinity(IntPtr handle, uint dwAffinity);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr handle, StringBuilder text, int count);

    [DllImport("user32.dll")]
    public static extern int SetWindowText(IntPtr handle, string text);

    [DllImport("User32.dll")]
    public static extern int SetWindowLong(IntPtr handle, int nIndex, int dwNewLong);
    [DllImport("User32.dll")]
    public static extern int GetWindowLong(IntPtr handle, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr handle);

    public const int SW_HIDE = 0x00;
    public const int SW_SHOW = 0x05;
    public const int SW_RESTORE = 0x09;
    public const int WS_EX_APPWINDOW = 0x40000;
    public const int GWL_EX_STYLE = -0x14;
    public const int WS_EX_TOOLWINDOW = 0x0080;


    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowThreadProcessId(IntPtr handle, out uint ProcessId);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr handle, StringBuilder lpClassName, int nMaxCount);

    public static (IntPtr handle, uint pid, string windowTitle, string windowClass, string processName, string filename) GetActiveProcessInfo()
    {
        IntPtr handle = default;
        uint pid = 0;
        string windowTitle = null, windowClass = null, processName = null, filename = null;
        try
        {
            handle = GetForegroundWindow();
            if (handle != IntPtr.Zero)
            {
                try
                {
                    const int nChars = 256;
                    StringBuilder buff = new StringBuilder(nChars);
                    if (GetWindowText(handle, buff, nChars) > 0)
                    {
                        windowTitle = buff.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                try
                {
                    const int nChars = 256;
                    StringBuilder buff = new StringBuilder(nChars);
                    if (GetClassName(handle, buff, nChars) > 0)
                        windowClass = buff.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                try
                {
                    GetWindowThreadProcessId(handle, out pid);
                    Process p = Process.GetProcessById((int)pid);
                    processName = p.ProcessName;
                    filename = p.GetMainModuleFileName();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return (handle, pid, windowTitle, windowClass, processName, filename);
    }

    public static bool HideOnScreenshot(uint pid)
    {
        var process = Process.Start(new ProcessStartInfo()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = "cmd.exe",
            Arguments = $"/c Invisiwind.exe -h {pid}",
            WorkingDirectory = @"_iv"
        });
        StringBuilder output = new StringBuilder();
        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            output.Append(">>" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            output.Append(">>" + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        output.Append("ExitCode: " + process.ExitCode);
        process.Close();

        var result = output.ToString();
        return result.Contains("Success!");
    }

    public static bool UnhideOnScreenshot(uint pid)
    {
        var process = Process.Start(new ProcessStartInfo()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = "cmd.exe",
            Arguments = $"/c Invisiwind.exe -u {pid}",
            WorkingDirectory = @"_iv"
        });
        StringBuilder output = new StringBuilder();
        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            output.Append(">>" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            output.Append(">>" + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        output.Append("ExitCode: " + process.ExitCode);
        process.Close();

        var result = output.ToString();
        return result.Contains("Success!");
    }

    public static void HideOnTaskbar(IntPtr handle)
    {
        //ShowWindow(handle, SW_HIDE);
        SetWindowLong(handle, GWL_EX_STYLE, (GetWindowLong(handle, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        //ShowWindow(handle, SW_SHOW);
    }

}

internal static class Extensions
{
    [DllImport("Kernel32.dll")]
    private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

    public static string GetMainModuleFileName(this Process process, int buffer = 1024)
    {
        var fileNameBuilder = new StringBuilder(buffer);
        uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
        return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
            fileNameBuilder.ToString() :
            null;
    }
}
