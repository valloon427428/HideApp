using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HotKeyManager.RegisterHotKey(Keys.Z, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(Keys.Z, KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Windows);
            HotKeyManager.RegisterHotKey(Keys.X, KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Windows);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            while (true)
            {
                System.Threading.Thread.Sleep(3600 * 1000);
            }
        }

        static IntPtr LastHandle = IntPtr.Zero;

        static void HotKeyManager_HotKeyPressed(object _, HotKeyEventArgs e)
        {
            Console.WriteLine(e);
            try
            {
                if (e.Key == Keys.Z && e.Modifiers == KeyModifiers.Alt)
                {
                    (var handle, var pid, var windowTitle, var windowClass, var processName, var filename) = HideAppHelper.GetActiveProcessInfo();
                    if (handle != IntPtr.Zero)
                    {
                        HideAppHelper.HideOnScreenshot(pid);
                        HideAppHelper.SetWindowText(handle, "");
                        HideAppHelper.HideOnTaskbar(handle);
                        LastHandle = handle;
                    }
                }
                else if (e.Key == Keys.Z && e.Modifiers == (KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Windows))
                {
                    (var handle, var pid, var windowTitle, var windowClass, var processName, var filename) = HideAppHelper.GetActiveProcessInfo();
                    if (handle != IntPtr.Zero)
                    {
                        HideAppHelper.UnhideOnScreenshot(pid);
                    }
                }
                else if (e.Key == Keys.X && e.Modifiers == (KeyModifiers.Alt | KeyModifiers.Control | KeyModifiers.Windows))
                {
                    if (LastHandle != IntPtr.Zero)
                    {
                        HideAppHelper.ShowWindow(LastHandle, HideAppHelper.SW_RESTORE);
                        HideAppHelper.SetForegroundWindow(LastHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
