using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Doorstop
{
    public static class Loader
    {
        private static bool Initialized = false;
        public static void Main(string[] args)
        {
            using (TextWriter textWriter = File.CreateText((Directory.GetCurrentDirectory() + "\\UKMM\\UKMM_PrePatcher.log")))
            {
                try
                {
                    textWriter.WriteLine(string.Format("Beginning assembly injection...", DateTime.Now));
                    textWriter.Flush();

                    foreach (FileInfo info in new DirectoryInfo(Directory.GetCurrentDirectory() + "\\UKMM\\Dependencies\\").GetFiles("*.dll"))
                    {
                        Assembly.Load(File.ReadAllBytes(info.FullName));
                        textWriter.WriteLine(string.Format("tried loading " + info.FullName, DateTime.Now));
                        textWriter.Flush();
                    }

                    textWriter.WriteLine(string.Format("Executing assembly is " + Assembly.GetExecutingAssembly().FullName, DateTime.Now));
                    textWriter.Flush();
                    // new Harmony("tempy.UKMM").PatchAll(Assembly.Load(File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ULTRAKILL_Data\\Managed\\UnityEngine.CoreModule.dll")));
                    Assembly harmonyAssembly = Assembly.Load(File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\UKMM\\0Harmony.dll"));
                    textWriter.WriteLine(string.Format("tried getting harmony", DateTime.Now));
                    object harmony = harmonyAssembly.GetType("HarmonyLib.Harmony", true).GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { "tempy.UKMM" });
                    textWriter.WriteLine(string.Format("tried making new harmony" , DateTime.Now));
                    harmony.GetType().GetMethod("PatchAll", new Type[] {typeof(Assembly)}).Invoke(harmony, new object[] { Assembly.Load(File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ULTRAKILL_Data\\Managed\\UnityEngine.CoreModule.dll")) });
                    textWriter.WriteLine(string.Format("tried patching", DateTime.Now));
                    textWriter.Flush();
                }
                catch (Exception e)
                {
                    textWriter.WriteLine(e.ToString(), DateTime.Now);
                    textWriter.Flush();
                }
            }
        }

        [HarmonyPatch(typeof(Camera), ".ctor")]
        public static void Prefix()
        {
            using (TextWriter textWriter = File.CreateText((Directory.GetCurrentDirectory() + "\\UKMM\\UKMM_Harmony.log")))
            {
                Assembly modded = Assembly.Load(File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\UKMM\\UKMM.dll"));
                textWriter.WriteLine(string.Format("tried loading modded", DateTime.Now));
                modded.GetType("UKModManager").GetMethod("InitializeManager").Invoke(null, null);
                textWriter.WriteLine(string.Format("End assembly injection", DateTime.Now));
            }
        }
    }
}