using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace NEP.ScoreLab.Data
{
    public static class DataManager
    {
        public static class Bundle
        {
            public static List<AssetBundle> Bundles { get; private set; }

            public static void Init()
            {
                Bundles = new List<AssetBundle>();

                InitializeDirectories();
                LoadBundles(Path_CustomUIs);
            }

            public static void LoadBundles(string path)
            {
                string[] files = LoadAllFiles(path);

                foreach (var file in files)
                {
                    if (!file.EndsWith(".hud"))
                    {
                        return;
                    }

                    AssetBundle bundle = AssetBundle.LoadFromFile(file);
                    Bundles.Add(bundle);
                }
            }
        }

        public static class PackedValues
        {
            public static void Init()
            {

            }
        }

        public static class HighScore
        {
            public static Dictionary<string, int> BestTable;

            public static void Init()
            {
                BestTable = new Dictionary<string, int>();
                BestTable = ReadFromFile();
            }

            public static void WriteBest(PackedHighScore highScore)
            {
                string sceneName = highScore.name;
                int bestScore = highScore.bestScore;

                BestTable.Add(sceneName, bestScore);
            }

            public static Dictionary<string, int> ReadFromFile()
            {
                string directory = File_HighScores;

                if (!File.Exists(directory))
                {
                    Debug.LogWarning("High score file doesn't exist! Creating one.");
                    File.Create(directory);
                    return null;
                }

                return JsonConvert.DeserializeObject(directory) as Dictionary<string, int>;
            }

            public static void WriteToFile()
            {
                string directory = File_HighScores;

                if (!File.Exists(directory))
                {
                    Debug.LogWarning("High score file doesn't exist! Creating one.");
                    File.Create(directory);
                    return;
                }

                var data = JsonConvert.SerializeObject(BestTable);
                File.WriteAllText(directory, data);
            }
        }

        public static class UI
        {
            public static void Init()
            {
                LoadedUIObjects = new List<GameObject>();
                UINames = new List<string>();

                LoadCustomUIs(Bundle.Bundles);
                LoadUINames();

                SpawnDefaultUI();
            }

            public static List<GameObject> LoadedUIObjects { get; private set; }
            public static List<string> UINames { get; private set; }

            private static readonly string MainUIName = "Coda";
            private static readonly string Prefix_Hud = "[SLHUD] - ";

            public static GameObject GetObjectFromList(GameObject[] list, string query)
            {
                foreach (var obj in list)
                {
                    if (obj.name == query)
                    {
                        return obj;
                    }
                }

                return null;
            }

            public static void LoadCustomUIs(List<AssetBundle> bundles)
            {
                if (bundles == null)
                {
                    return;
                }

                foreach (var bundle in bundles)
                {
                    var loadedObjects = bundle.LoadAllAssets();

                    foreach (var bundleObject in loadedObjects)
                    {
                        if (bundleObject is GameObject go)
                        {
                            bundleObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
                            LoadedUIObjects.Add(go);
                        }
                    }
                }
            }

            public static void LoadUINames()
            {
                foreach (var uiObject in LoadedUIObjects)
                {
                    if (uiObject.name.StartsWith("[SLHUD]"))
                    {
                        UINames.Add(uiObject.name.Substring(Prefix_Hud.Length));
                    }
                }
            }

            public static void SpawnDefaultUI()
            {
                SpawnUI(MainUIName);
            }

            public static void SpawnUI(string name)
            {
                foreach (var obj in LoadedUIObjects)
                {
                    if (GetHUDName(obj) == name)
                    {
                        SpawnUI(obj);
                    }
                }
            }

            public static void SpawnUI(GameObject uiObject)
            {
                GameObject createdUI = GameObject.Instantiate(uiObject);
                createdUI.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }

            private static string GetHUDName(GameObject obj)
            {
                return obj.name.Substring(Prefix_Hud.Length);
            }
        }

        static readonly string Path_UserData = Application.dataPath + "/Data/";
        static readonly string Path_Developer = Path_UserData + "Not Enough Photons/";
        static readonly string Path_Mod = Path_Developer + "ScoreLab/";
        static readonly string Path_CustomUIs = Path_Mod + "Custom UIs/";

        static readonly string Path_ScoreData = Path_Mod + "Data/Score/";
        static readonly string Path_MultiplierData = Path_Mod + "Data/Multiplier/";
        static readonly string Path_HighScoreData = Path_Mod + "Data/High Score/";

        static readonly string File_HighScores = Path_HighScoreData + "high_score_table.json";
        static readonly string File_HUDSettings = Path_Mod + "sl_hud_settings.json";
        static readonly string File_CurrentHUD = Path_Mod + "sl_current_hud.txt";

        public static void Init()
        {
            InitializeDirectories();

            Bundle.Init();
            UI.Init();
            PackedValues.Init();
            HighScore.Init();
        }

        public static string[] LoadAllFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        private static void InitializeDirectories()
        {
            Directory.CreateDirectory(Path_Mod);
            Directory.CreateDirectory(Path_CustomUIs);

            Directory.CreateDirectory(Path_ScoreData);
            Directory.CreateDirectory(Path_MultiplierData);
        }
    }
}

