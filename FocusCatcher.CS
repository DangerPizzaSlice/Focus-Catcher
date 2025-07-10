using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    static void Main()
    {
        int lastPid = -1;
        string lastProc = "";

        while (true)
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            if (pid != lastPid)
            {
                lastPid = (int)pid;
                string procName = "unknown";
                try
                {
                    procName = Process.GetProcessById((int)pid).ProcessName;
                }
                catch { }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0:HH:mm:ss.fff} Фокус: PID {1}  {2}", DateTime.Now, pid, procName);
                Console.ResetColor();
            }

            Thread.Sleep(50);
        }
    }
}