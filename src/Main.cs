using System.Linq;

using MelonLoader;

using UnityEngine;

using NEP.ScoreLab.Core.Data;
using NEP.ScoreLab.Utilities;

namespace NEP.ScoreLab
{
    public static class BuildInfo
    {
        public const string Name = "Scoreworks - Version 3.0"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "3.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        public static Main instance;

        public string lastUI;

        public GameObject uiObject { get; private set; }
        public UI.UIManager uiComponent { get; private set; }
        public GameObject[] customUIs { get; private set; }

        private string[] bundleFiles;
        private AssetBundle[] bundles;

        public override void OnApplicationStart()
        {
            instance = this;

            InitializeBundles();

            new Core.ScoreLabManager();
            DataManager.Initialize();

            Utils.ImpactPropertiesPatch.Patch();
            Utils.RigidbodyProjectilePatch.Patch();
        }

        public override void OnApplicationQuit()
        {
            DataManager.SaveHighScore(Core.ScoreLabManager.instance.currentSceneLiteral, Core.ScoreLabManager.instance.currentHighScore);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            lastUI = DataManager.GetLastHUD();

            ResetScoreManager(sceneName, false);

            new Core.Director();
            new Audio.AudioManager();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if(sceneName == "NEW OBJECT COLLECTOR" || sceneName == "loadingScene" || sceneName == "")
            {
                return;
            }

            DataManager.SaveHighScore(Core.ScoreLabManager.instance.currentSceneLiteral, Core.ScoreLabManager.instance.currentScore);
        }

        public override void OnUpdate()
        {
            Core.ScoreLabManager.instance?.Update();
            Core.Director.Update();
        }

        public void SpawnHUD(GameObject hudObject)
        {
            if (hudObject == null)
            {
                return;
            }

            GameObject.Destroy(uiObject);
            
            uiObject = GameObject.Instantiate(hudObject);

            if (!uiObject.GetComponent<UI.UIManager>())
            {
                uiObject.AddComponent<UI.UIManager>();
            }

            lastUI = uiObject.name.Replace("(Clone)", "");
            DataManager.SaveLastHUD(lastUI);
        }

        public void SpawnHUD(string hudName)
        {
            if(customUIs == null)
            {
                return;
            }

            GameObject selectedHud = customUIs.FirstOrDefault((hud) => hud.gameObject.name == hudName);

            if(selectedHud == null)
            {
                return;
            }

            SpawnHUD(selectedHud);
        }

        public void ResetScoreManager(string sceneName, bool isCustomMap)
        {
            Core.ScoreLabManager.instance.currentSceneLiteral = sceneName;
            Core.ScoreLabManager.instance.currentScene = isCustomMap ?  sceneName : Utils.GetLevelFromSceneName(sceneName);

            if (DataManager.highScoreTable.ContainsKey(sceneName))
            {
                Core.ScoreLabManager.instance.currentHighScore = DataManager.RetrieveHighScore(sceneName).highScore;
            }
            else
            {
                Core.ScoreLabManager.instance.currentHighScore = 0;
            }

            Core.ScoreLabManager.instance.currentScore = 0;
            Core.ScoreLabManager.instance.currentMultiplier = 1f;

            Core.ScoreLabManager.scoreDict.Clear();
            Core.ScoreLabManager.multDict.Clear();
            Core.ScoreLabManager.swValues.Clear();

            SpawnHUD(lastUI);
        }

        private void InitializeBundles()
        {
            bundleFiles = System.IO.Directory.GetFiles(MelonUtils.UserDataDirectory + "/Scoreworks/HUDs/");
            bundles = new AssetBundle[bundleFiles.Length];
            customUIs = new GameObject[bundles.Length];

            for (int i = 0; i < bundles.Length; i++)
            {
                bundles[i] = AssetBundle.LoadFromFile(bundleFiles[i]);
                customUIs[i] = bundles[i].LoadAsset("SWHud.prefab").Cast<GameObject>();
                customUIs[i].name = bundles[i].name;

                MelonLogger.Msg($"Loaded " + bundles[i].name);
                customUIs[i].hideFlags = HideFlags.DontUnloadUnusedAsset;
            }
        }

    }
}
