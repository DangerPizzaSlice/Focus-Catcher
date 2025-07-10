// Focus Catcher v.1.3 — поддержка старого C#, ANSI-гиперссылка на папку, автодетект языка

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.Linq;
using System.IO;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    static void Main()
    {
        bool hasRussian = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Any(c => c.TwoLetterISOLanguageName == "ru");

        string header, desc1, desc2, desc3, desc4, desc5, desc6, desc7, desc8, focusPrefix;
        if (hasRussian)
        {
            header = "Focus Catcher v.1.3";
            desc1 = "Разработчик - DangerPizzaSlice";
			desc2 = "Контакты:";
			desc3 = "Mail: DangerPizzaSlice@gmail.com";
			desc4 = "https://steamcommunity.com/id/DangerPizzaSlice/";
			desc5 = "Telegram @DangerPizzaSlice";
            desc6 = "Здесь будет список программ, которые захватывали фокус.";
            desc7 = "Последняя в списке сразу после появления проблемы — скорее всего виновник.";
			desc8 = "Клик на путь к файлу с зажатым CTRL открывает папку в проводнике.";
			
            focusPrefix = "Фокус перехвачен:";
        }
        else
        {
            header = "Focus Catcher v.1.3";
            desc1 = "Developed by DangerPizzaSlice";
			desc2 = "Контакты:";
			desc3 = "Mail: DangerPizzaSlice@gmail.com";
			desc4 = "https://steamcommunity.com/id/DangerPizzaSlice/";
			desc5 = "Telegram @DangerPizzaSlice";
            desc6 = "Here is a list of programs that took focus.";
            desc7 = "Last one in the list right after the problem appeared is probably your culprit.";
			desc8 = "Click on a founded path with CTRL will open it in Explorer.";
			
            focusPrefix = "Focus Taken By:";
        }

        Console.WriteLine(header);
        Console.WriteLine();
        Console.WriteLine(desc1);
		Console.WriteLine(desc2);
		Console.WriteLine(desc3);
		Console.WriteLine(desc4);
		Console.WriteLine(desc5);
		Console.WriteLine();
        Console.WriteLine(desc6);
		Console.WriteLine(desc7);
		Console.WriteLine(desc8);
		Console.WriteLine();
		
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
                string exePath = "unknown";
                string folderPath = "unknown";
                try
                {
                    var proc = Process.GetProcessById((int)pid);
                    procName = proc.ProcessName;
                    try
                    {
                        exePath = proc.MainModule.FileName;
                        folderPath = Path.GetDirectoryName(exePath);
                    }
                    catch { }
                }
                catch { }

                // Игнорируем WindowsTerminal внутри которого бежит программа
                if (procName.Equals("WindowsTerminal", StringComparison.OrdinalIgnoreCase))
                {
                    Thread.Sleep(50);
                    continue;
                }

                string time = DateTime.Now.ToString("HH:mm:ss");
                string clickableFolder = folderPath;
                try
                {
                    if (!string.IsNullOrEmpty(folderPath) && folderPath != "unknown")
                    {
                        // Формируем ANSI-гиперссылку через string.Format
                        string ansiFormat = "\x1b]8;;file:///{0}\x1b\\{1}\x1b]8;;\x1b\\";
                        string unixStylePath = folderPath.Replace("\\", "/");
                        clickableFolder = string.Format(ansiFormat, unixStylePath, folderPath);
                    }
                }
                catch { clickableFolder = folderPath; }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("{0} {1}  PID {2}  \"{3}\"  [{4}]", focusPrefix, time, pid, procName, clickableFolder);
                Console.ResetColor();
            }
            Thread.Sleep(50);
        }
    }
}
