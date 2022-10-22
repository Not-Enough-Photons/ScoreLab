using System.IO;
using System.Collections.Generic;

using UnityEngine;

namespace NEP.ScoreLab.Data
{
    public static class DataManager
    {
        public static AssetBundle[] Bundles { get; private set; }
        public static GameObject[] LoadedUIObjects { get; private set; }
        public static string[] UINames { get; private set; }

        private static readonly string MainUIName = "coda.hud";

        private static readonly string Path_UserData = MelonLoader.MelonUtils.UserDataDirectory + "/";
        private static readonly string Path_Developer = Path_UserData + "Not Enough Photons/";
        private static readonly string Path_Mod = Path_Developer + "ScoreLab/";
        private static readonly string Path_CustomUI = Path_Mod + "Custom UIs/";

        private static readonly string Path_HighScores = Path_Mod + "sl_high_scores.json";
        private static readonly string Path_HUDSettings = Path_Mod + "sl_hud_settings.json";
        private static readonly string Path_CurrentHUD = Path_Mod + "sl_current_hud.txt";

        public static void Init()
        {
            InitializeDirectories();
            LoadBundles(Path_CustomUI);
            LoadCustomUIs(Bundles);
        }

        public static GameObject GetObjectFromList(GameObject[] list, string query)
        {
            foreach(var obj in list)
            {
                if(obj.name == query)
                {
                    return obj;
                }
            }

            return null;
        }

        public static string[] LoadAllFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public static void LoadBundles(string path)
        {
            List<AssetBundle> loadedObjects = new List<AssetBundle>();
            string[] files = LoadAllFiles(path);
            UINames = files;

            foreach(var file in files)
            {
                if (!file.EndsWith(".hud"))
                {
                    return;
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(file);
                loadedObjects.Add(bundle);
            }

            Bundles = loadedObjects.ToArray();
        }

        public static void LoadCustomUIs(AssetBundle[] bundles)
        {
            List<GameObject> loadedUIs = new List<GameObject>();

            if(bundles == null)
            {
                return;
            }

            foreach(var bundle in bundles)
            {
                var loadedObjects = bundle.LoadAllAssets();

                foreach(var bundleObject in loadedObjects)
                {
                    if (bundleObject.TryCast<GameObject>() != null)
                    {
                        bundleObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
                        loadedUIs.Add(bundleObject.Cast<GameObject>());
                    }
                }
            }

            LoadedUIObjects = loadedUIs.ToArray();
        }

        public static void SpawnDefaultUI()
        {
            SpawnUI(MainUIName);
        }

        public static void SpawnUI(string name)
        {
            foreach(string uiName in UINames)
            {
                string uiNameCleaned = uiName.Substring(Path_CustomUI.Length + 1).TrimEnd('.');

                if(uiNameCleaned == name)
                {
                    foreach(var obj in LoadedUIObjects)
                    {
                        string uiObjectName = obj.name.ToLower();

                        if (uiObjectName.EndsWith(uiNameCleaned))
                        {
                            SpawnUI(obj);
                        }
                    }
                }
            }
        }

        public static void SpawnUI(GameObject uiObject)
        {
            GameObject createdUI = GameObject.Instantiate(uiObject);
            createdUI.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private static void InitializeDirectories()
        {
            Directory.CreateDirectory(Path_Mod);
            Directory.CreateDirectory(Path_CustomUI);
        }
    }
}

