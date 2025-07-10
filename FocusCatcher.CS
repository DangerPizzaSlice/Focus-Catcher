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
        Console.WriteLine("Developed by / Разработчик - DangerPizzaSlice");
        Console.WriteLine();
        Console.WriteLine("Here is a list of programs that takes focus.");
        Console.WriteLine("Last one in the list right after the problem appeared, probably is your culprit.");
        Console.WriteLine();
        Console.WriteLine("Тут будет указан список программ, которые получали приоритет фокуса.");
        Console.WriteLine("Последнее в списке сразу после репликации проблемы скорее всего и есть виновник.");
        Console.WriteLine(new string('-', 95));

        int lastPid = -1;

        while (true)
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            if ((int)pid != lastPid)
            {
                lastPid = (int)pid;
                string procName = "unknown";
                try
                {
                    procName = Process.GetProcessById((int)pid).ProcessName;
                }
                catch { }

                // Игнорируем WindowsTerminal
                if (procName.Equals("WindowsTerminal", StringComparison.OrdinalIgnoreCase))
                {
                    Thread.Sleep(50);
                    continue;
                }

                string time = DateTime.Now.ToString("HH:mm:ss");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Focus Taken By: {0}  PID {1}  \"{2}\"", time, pid, procName);
                Console.ResetColor();
            }
            Thread.Sleep(50);
        }
    }
}
