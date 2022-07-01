using System.Linq;

using MelonLoader;

using ModThatIsNotMod.BoneMenu;

using UnityEngine;

using UnhollowerBaseLib;

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
        public GameObject uiObject { get; private set; }

        public string lastUI;

        private Core.Director director;
        private UI.UIManager uiManager;

        private string[] bundleFiles;
        private AssetBundle[] bundles;
        private GameObject[] customUIs;

        private Il2CppReferenceArray<Object> bundleObjects;

        public override void OnApplicationStart()
        {
            instance = this;

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

            SetupBonemenu();
        }

        public override void OnApplicationLateStart()
        {
            new Core.ScoreworksManager();
            Core.Data.DataManager.Initialize();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Core.Data.SWHighScore highScore = new Core.Data.SWHighScore()
            {
                currentScene = Core.ScoreworksManager.instance.currentScene,
                highScore = Core.ScoreworksManager.instance.currentHighScore
            };

            Core.Data.DataManager.SaveHighScore(highScore);

            Core.ScoreworksManager.instance.currentScore = 0;
            Core.ScoreworksManager.instance.currentMultiplier = 0f;

            director = new Core.Director();
            new Audio.AudioManager();

            //SpawnHUD(lastUI);
        }

        public override void OnUpdate() => Core.ScoreworksManager.instance?.Update();

        private void SetupBonemenu()
        {
            MenuCategory mainCategory = MenuManager.CreateCategory("Scoreworks Settings", Color.white);
            MenuCategory hudCategory = mainCategory.CreateSubCategory("HUDs", Color.blue);
            MenuCategory hudSettingsCategory = mainCategory.CreateSubCategory("HUD Settings", Color.white);

            SetupPaddingCategory(hudSettingsCategory);
            SetupSizeCategory(hudSettingsCategory);

            foreach(GameObject uiObject in customUIs)
            {
                hudCategory.CreateFunctionElement(uiObject.name, Color.white, () => SpawnHUD(uiObject));
            }
        }

        private void SetupPaddingCategory(MenuCategory category)
        {
            MenuCategory paddingCategory = category.CreateSubCategory("Padding", Color.white);

            MenuCategory leftRegionCat = paddingCategory.CreateSubCategory("Left Padding", Color.white);
            MenuCategory rightRegionCat = paddingCategory.CreateSubCategory("Right Padding", Color.white);
            MenuCategory topRegionCat = paddingCategory.CreateSubCategory("Top Padding", Color.white);
            MenuCategory bottomRegionCat = paddingCategory.CreateSubCategory("Bottom Padding", Color.white);

            leftRegionCat.CreateFloatElement("X", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            leftRegionCat.CreateFloatElement("Y", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            leftRegionCat.CreateFloatElement("Z", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);

            rightRegionCat.CreateFloatElement("X", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            rightRegionCat.CreateFloatElement("Y", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            rightRegionCat.CreateFloatElement("Z", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);

            topRegionCat.CreateFloatElement("X", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            topRegionCat.CreateFloatElement("Y", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            topRegionCat.CreateFloatElement("Z", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);

            bottomRegionCat.CreateFloatElement("X", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            bottomRegionCat.CreateFloatElement("Y", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);
            bottomRegionCat.CreateFloatElement("Z", Color.white, 0f, null, 0.25f, float.NegativeInfinity, float.PositiveInfinity, true);

            paddingCategory.CreateFunctionElement("Save Settings", Color.green, null);
        }

        private void SetupSizeCategory(MenuCategory category)
        {
            MenuCategory sizeCategory = category.CreateSubCategory("Size", Color.white);

            sizeCategory.CreateFloatElement("Left Scale", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("Right Scale", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("Top Scale", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("Bottom Scale", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("HUD Scale", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("Follow Distance", Color.white, 1f, null);
            sizeCategory.CreateFloatElement("Follow Lerp", Color.white, 1f, null);

            sizeCategory.CreateFunctionElement("Save Settings", Color.green, null);
        }

        private void SpawnHUD(GameObject hudObject)
        {
            if(hudObject == null)
            {
                return;
            }

            if(uiObject == null)
            {
                uiObject = GameObject.Instantiate(hudObject);
                uiObject.AddComponent<UI.UIManager>();
                lastUI = uiObject.name;
            }
            else
            {
                GameObject.Destroy(uiObject);

                uiObject = GameObject.Instantiate(hudObject);
                uiObject.AddComponent<UI.UIManager>();
                lastUI = uiObject.name;
            }
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
