using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UMM.Loader;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace UMM
{
    public static class UKAPI
    {
        public static bool triedLoadingBundle = false;
        //public static Dictionary<string, Dictionary<string, WeaponInformation>> allCustomWeapons = new Dictionary<string, Dictionary<string, WeaponInformation>>();
        private static AssetBundle commonBundle;
        private static List<string> disableCybergrindReasons = new List<string>();

        /// <summary>
        /// Returns whether or not leaderboard submissions are allowed.
        /// </summary>
        public static bool CanSubmitCybergrindScore
        {
            get
            {
                return disableCybergrindReasons.Count == 0;
            }
        }

        /// <summary>
        /// Returns a clone of all found <see cref="ModInformation"/> instances.
        /// </summary>
        public static ModInformation[] AllModInfoClone => UltraModManager.foundMods.ToArray().Clone() as ModInformation[];

        /// <summary>
        /// Returns a clone of all loaded <see cref="ModInformation"/> instances.
        /// </summary>
        public static ModInformation[] AllLoadedModInfoClone => UltraModManager.allLoadedMods.ToArray().Clone() as ModInformation[];

        /// <summary>
        /// Initializes the API by loading the save file and common asset bundle
        /// </summary>
        internal static IEnumerator InitializeAPI()
        {
            if (triedLoadingBundle)
                yield break;
            SaveFileHandler.LoadData();
            Debug.Log("UMM: Trying to load common asset bundle from " + Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
            yield return request;
            int attempts = 1;
            while (request.assetBundle == null)
            {
                yield return new WaitForSeconds(0.2f); // why 0.2? I dunno I just chose it man
                if (attempts >= 5)
                {
                    Debug.Log("UMM: Could not load common asset bundle");
                    triedLoadingBundle = true;
                    yield break;
                }
                request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
                yield return request;
                attempts++;
            }

            Debug.Log("Loaded common asset bundle");
            commonBundle = request.assetBundle;
            triedLoadingBundle = true;
            UltraModManager.InitializeManager();

            while (MapLoader.Instance == null)
                yield return null;
            Dictionary<string, AssetBundle> bundles = Traverse.Create(MapLoader.Instance).Field("loadedBundles").GetValue() as Dictionary<string, AssetBundle>;
            if (bundles == null)
                bundles = new Dictionary<string, AssetBundle>();
            bundles.Add("common", commonBundle);
            Traverse.Create(MapLoader.Instance).Field("loadedBundles").SetValue(bundles);
            MapLoader.Instance.isCommonLoaded = true;
        }

        /// <summary>
        /// Disables CyberGrind submission, CyberGrind submissions can only be enabled if nothing else disables it
        /// </summary>
        /// <param name="reason">Why CyberGrind is disabled, if you want to reenable it later you can do so by removing the reason</param>
        public static void DisableCyberGrindSubmission(string reason)
        {
            if (!disableCybergrindReasons.Contains(reason))
                disableCybergrindReasons.Add(reason);
        }

        /// <summary>
        /// Disables CyberGrind submission, cybergrind submissions can only be enabled if nothing else disables it
        /// </summary>
        /// <param name="reason">The reason to remove</param>
        public static void RemoveDisableCyberGrindReason(string reason)
        {
            if (disableCybergrindReasons.Contains(reason))
                disableCybergrindReasons.Remove(reason);
            else
                Debug.Log("Tried to remove cg reason " + reason + " but could not find it!");
        }

        [Obsolete("Use CanSubmitLeaderboardScore instead.")]
        public static bool ShouldSubmitCyberGrindScore() => CanSubmitCybergrindScore;

        /// <summary>
        /// Tries to create a Ultrakill asset load request from ULTRAKILL_Data/StreamingAssets/common, note that this request has to be awaited
        /// </summary>
        /// <param name="name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns>A new asset bundle request for your item</returns>
        public static AssetBundleRequest LoadCommonAssetAsync(string name)
        {
            if (commonBundle == null)
            {
                Debug.LogError("UMM: Could not load asset " + name + " due to the common asset bundle not being loaded.");
                return null;
            }
            return commonBundle.LoadAssetAsync(name);
        }

        /// <summary>
        /// Tries to load an Ultrakill asset from ULTRAKILL_Data/StreamingAssets/common
        /// </summary>
        /// <param name="name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns>The asset from the bundle as an object if found, otherwise returns null</returns>
        public static object LoadCommonAsset(string name)
        {
            if (commonBundle == null)
            {
                Debug.LogError("UMM: Could not load asset " + name + " due to the common asset bundle not being loaded.");
                return null;
            }
            return commonBundle.LoadAsset(name);
        }

        [Obsolete("Use AllModInfoClone instead.")]
        public static ModInformation[] GetAllModInformation() => AllModInfoClone;

        [Obsolete("Use AllLoadedModInfoClone instead.")]
        public static ModInformation[] GetAllLoadedModInformation() => AllLoadedModInfoClone;

        /// <summary>
        /// Restarts Ultrakill
        /// </summary> 
        public static void Restart() // thanks https://gitlab.com/vtolvr-mods/ModLoader/-/blob/release/Launcher/Program.cs
        {
            Application.Quit();
            Debug.Log("Restarting Ultrakill!");

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"steam://run/1229490",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized
            };
            System.Diagnostics.Process.Start(psi);

            //Debug.Log("Path is \"" + Environment.CurrentDirectory + "\\BepInEx\\plugins\\UMM\\UltrakillRestarter.exe\"");
            //string strCmdText;
            //strCmdText = "/K \"" + Environment.CurrentDirectory + "\\BepInEx\\plugins\\UMM\\Ultrakill Restarter.exe\""/* + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + "\""*/;
            ////strCmdText = "/K \"" + Environment.CurrentDirectory + "\\ULTRAKILL.exe\"";
            //System.Diagnostics.Process.Start("CMD.exe", strCmdText);

            //var psi = new System.Diagnostics.ProcessStartInfo 
            //{
            //    FileName = Environment.CurrentDirectory + "\\BepInEx\\plugins\\UMM\\Ultrakill Restarter.exe",
            //    UseShellExecute = true,
            //    WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized,
            //    Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString()
            //};
            //System.Diagnostics.Process.Start(psi);
        }

        internal static class SaveFileHandler
        {
            private static Dictionary<string, Dictionary<string, string>> savedData = new Dictionary<string, Dictionary<string, string>>();
            private static string path = "";

            internal static void LoadData()
            {
                path = Assembly.GetExecutingAssembly().Location;
                path = path.Substring(0, path.LastIndexOf("\\")) + "\\persistent mod data.json";
                Debug.Log("Trying to mod persistent data file from " + path);
                FileInfo fInfo = new FileInfo(path);
                if (fInfo.Exists)
                {
                    using (StreamReader jFile = fInfo.OpenText())
                    {
                        savedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jFile.ReadToEnd());
                        if (savedData == null)
                            savedData = new Dictionary<string, Dictionary<string, string>>();
                        jFile.Close();
                    }
                }
                else
                {
                    Debug.Log("Couldn't find a save file, making one now");
                    fInfo.Create();
                }
            }

            internal static void DumpFile()
            {
                FileInfo fInfo = new FileInfo(path);
                Debug.Log("Dumping mod persistent data file to " + path);
                File.WriteAllText(fInfo.FullName, JsonConvert.SerializeObject(savedData));
            }

            /// <summary>
            /// Gets presistent mod data from a key and modname 
            /// </summary>
            /// <param name="modName">The name of the mod to retrieve data from</param>
            /// <param name="key">The value you want</param>
            /// <returns>The mod data if found, otherwise null</returns>
            public static string RetrieveModData(string modName, string key)
            {
                if (savedData.ContainsKey(modName))
                {
                    if (savedData[modName].ContainsKey(key))
                        return savedData[modName][key];
                }
                return null;
            }

            /// <summary>
            /// Adds persistent mod data from a key and mod name
            /// </summary>
            /// <param name="modName">The name of the mod to add data to</param>
            /// <param name="key">The key for the data</param>
            /// <param name="value">The data you want as a string, note you can only add strings</param>
            public static void SetModData(string modName, string key, string value)
            {
                if (!savedData.ContainsKey(modName))
                {
                    Dictionary<string, string> newDict = new Dictionary<string, string>();
                    newDict.Add(key, value);
                    savedData.Add(modName, newDict);
                }
                else if (!savedData[modName].ContainsKey(key))
                    savedData[modName].Add(key, value);
                else
                    savedData[modName][key] = value;
            }

            /// <summary>
            /// Removes persistent mod data from a key and a mod name
            /// </summary>
            /// <param name="modName">The name of the mod to remove data from</param>
            /// <param name="key">The key for the data</param>
            public static void RemoveModData(string modName, string key)
            {
                if (savedData.ContainsKey(modName))
                {
                    if (savedData[modName].ContainsKey(key))
                        savedData[modName].Remove(key);
                }
            }
        }

        #region CustomWeapons
        /* Shelved for now due to not being worth implementing over other features, will do later if the need arrises
        /// <summary>
        /// Adds a new custom weapon to UKAPI
        /// </summary>
        /// <param name="prefab">The prefab of the custom weapon that will be instantiated when loaded</param>
        /// <param name="weaponName">The name of the weapon to show in the menu</param>
        /// <param name="category">The name of the category to add the weapon to</param>
        public static void AddNewWeapon(GameObject prefab, string weaponName, string category, int slot = 5)
        {
            if (prefab == null)
            {
                Debug.Log("Tried to add a null prefab of name " + weaponName + " and category " + category);
                return;
            }
            if (!allCustomWeapons.ContainsKey(category))
            {
                Dictionary<string, WeaponInformation> toAdd = new Dictionary<string, WeaponInformation>();
                toAdd.Add(weaponName, new WeaponInformation(prefab, slot));
                allCustomWeapons.Add(category, toAdd);
            }
            else if (!allCustomWeapons[category].ContainsKey(weaponName))
                allCustomWeapons[category].Add(weaponName, new WeaponInformation(prefab, slot));
            else
                Debug.Log("Tried to add duplicate weapon name " + weaponName + " of category " + category + " with a prefab name of " + prefab.name);
        }

        /// <summary>
        /// Retrieves new custom weapon from UKAPI
        /// </summary>
        /// <param name="weaponName">The name of the weapon</param>
        /// <param name="category">The name of the category the weapon is in </param>
        /// <returns>The custom weapon if null, otherwise null</returns>
        public static WeaponInformation RetrieveWeapon(string category, string weaponName)
        {
            if (allCustomWeapons.ContainsKey(category) && allCustomWeapons[category].ContainsKey(weaponName))
            {
                if (allCustomWeapons[category][weaponName] == null)
                {
                    Debug.Log("Weapon of category " + category + " and weapon name " + weaponName + " was null!");
                    return null;
                }
                return allCustomWeapons[category][weaponName];
            }
            Debug.Log("Weapon of category " + category + " and weapon name " + weaponName + " was not found!");
            return null;
        }

        /// <summary>
        /// Retrieves new custom weapon from UKAPI
        /// </summary>
        /// <returns>An array of all custom weapons</returns>
        public static WeaponInformation[] RetrieveAllWeapons()
        {
            List<WeaponInformation> allWeapons = new List<WeaponInformation>();
            foreach (string category in allCustomWeapons.Keys)
                foreach (string weaponName in allCustomWeapons[category].Keys)
                    allWeapons.Add(allCustomWeapons[category][weaponName]);
            return allWeapons.ToArray();
        }

        public class WeaponInformation
        {
            public GameObject weaponPrefab;
            public WeaponIcon icon;
            public int slot;

            public WeaponInformation(GameObject weaponPrefab, int slot = 5)
            {
                this.weaponPrefab = weaponPrefab;
                this.icon = weaponPrefab.GetComponent<WeaponIcon>();
                this.slot = slot;
            }
        }
        */
        #endregion
    }
}
