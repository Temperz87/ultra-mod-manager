using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UMM.Restarter
{
    public class Restarter
    {
        public static void Main(string[] args) // thanks https://gitlab.com/vtolvr-mods/ModLoader/-/blob/release/Launcher/Program.cs
        {
            Thread t = new Thread(() =>
            {
                Console.WriteLine(Environment.CurrentDirectory);
                string bepinPath = Environment.CurrentDirectory + "\\BepInEx\\config\\BepInEx.cfg";
                Console.WriteLine(bepinPath);
                if (args.Length > 0)
                {
                    Console.WriteLine("Ultrakill's id is " + args[0]);

                    bool FileOpen()
                    {
                        try
                        {
                            using (Stream stream = new FileStream(bepinPath, FileMode.Open))
                            {
                                Console.WriteLine("BepInEx.cfg is not open");
                                stream.Close();
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("BepInEx.cfg is open, with exception\n" + e.ToString() + " \n" + e.Message);
                            return true;
                        }
                    }

                    try
                    {
                        while (Process.GetProcessById(int.Parse(args[0])) != null)
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception e)
                    {
                        while (FileOpen())
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                //string path = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("ULTRAKILL") + 9) + "\\ULTRAKILL.exe";
                string path = @"C:\Program Files (x86)\Steam\steamapps\common\ULTRAKILL\ULTRAKILL.exe";
                Console.WriteLine("Looking in directory " + path);
                Thread.Sleep(5000);
                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                };
                Process.Start(psi);
                Console.WriteLine("Ultrakill started!");
            });
            t.Start();
        }
    }
}
