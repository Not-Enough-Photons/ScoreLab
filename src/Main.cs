using System.Linq;

using MelonLoader;

using ModThatIsNotMod.BoneMenu;

using UnityEngine;

using UnhollowerBaseLib;

using NEP.Scoreworks.Core.Data;
using NEP.Scoreworks.Utilities;

namespace NEP.Scoreworks
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

        private string[] bundleFiles;
        private AssetBundle[] bundles;
        private GameObject[] customUIs;

        private Il2CppReferenceArray<Object> bundleObjects;

        public override void OnApplicationStart()
        {
            instance = this;

            Utilities.Utilities.InitializeMelonPrefs();

            bundleFiles = System.IO.Directory.GetFiles(MelonUtils.UserDataDirectory + "/Scoreworks/HUDs/");
            bundles = new AssetBundle[bundleFiles.Length];
            customUIs = new GameObject[bundles.Length];

            for(int i = 0; i < bundles.Length; i++)
            {
                bundles[i] = AssetBundle.LoadFromFile(bundleFiles[i]);
                customUIs[i] = bundles[i].LoadAsset("SWHud.prefab").Cast<GameObject>();
                customUIs[i].name = bundles[i].name;

                MelonLogger.Msg($"Loaded " + bundles[i].name);
                customUIs[i].hideFlags = HideFlags.DontUnloadUnusedAsset;
            }

            lastUI = Utilities.Utilities.GetHUDFromPref();

            SetupBonemenu();

            new Core.ScoreworksManager();
            DataManager.Initialize();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (DataManager.highScoreTable.ContainsKey(sceneName))
            {
                SWHighScore highScore = new SWHighScore()
                {
                    currentScene = DataManager.highScoreTable[sceneName].currentScene,
                    highScore = DataManager.highScoreTable[sceneName].highScore
                };

                Core.ScoreworksManager.instance.currentScene = highScore.currentScene;
                Core.ScoreworksManager.instance.currentHighScore = highScore.highScore;
            }

            Core.ScoreworksManager.instance.currentScore = 0;
            Core.ScoreworksManager.instance.currentMultiplier = 0f;

            new Core.Director();
            new Audio.AudioManager();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if(sceneName == "NEW OBJECT COLLECTOR" || sceneName == "loadingScene" || sceneName == "")
            {
                return;
            }

            SWHighScore highScore = new SWHighScore()
            {
                currentScene = sceneName,
                highScore = Core.ScoreworksManager.instance.currentScore
            };

            DataManager.SaveHighScore(highScore);
        }

        public override void OnUpdate() => Core.ScoreworksManager.instance?.Update();

        private void SetupBonemenu()
        {
            MenuCategory mainCategory = MenuManager.CreateCategory("Scoreworks Settings", Color.white);
            MenuCategory hudCategory = mainCategory.CreateSubCategory("HUDs", Color.blue);
            MenuCategory hudSettingsCategory = mainCategory.CreateSubCategory("HUD Settings", Color.white);
            SetupHUDSettings(hudSettingsCategory);

            foreach(GameObject uiObject in customUIs)
            {
                hudCategory.CreateFunctionElement(uiObject.name, Color.white, () => SpawnHUD(uiObject));
            }
        }

        private void SetupHUDSettings(MenuCategory category)
        {
            category.CreateFloatElement("Follow Distance", Color.white, 1f, (newValue) => uiObject.GetComponent<UI.UIManager>().hudSettings.followDistance = newValue);
            category.CreateFloatElement("Follow Lerp", Color.white, 1f, (newValue) => uiObject.GetComponent<UI.UIManager>().hudSettings.followLerp = newValue);
        }

        private void SpawnHUD(GameObject hudObject)
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

            lastUI = uiObject.name;
        }

        private void SpawnHUD(string hudName)
        {
            if(customUIs == null)
            {
                return;
            }

            GameObject selectedHud = customUIs.FirstOrDefault((hud) => hud.name == hudName);

            if(selectedHud == null)
            {
                return;
            }

            SpawnHUD(selectedHud);
        }

        private AssetBundle GetBundle()
        {
            return AssetBundle.LoadFromFile(MelonUtils.UserDataDirectory + "/Scoreworks/basehud.hud");
        }

        private Object GetObject(string name)
        {
            return bundleObjects.FirstOrDefault((obj) => obj.name == name);
        }
    }
}
