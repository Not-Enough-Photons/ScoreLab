using System.Linq;

using MelonLoader;

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

        private Core.Director director;
        private UI.UIManager uiManager;

        private AssetBundle bundle;

        private Il2CppReferenceArray<Object> bundleObjects;

        public override void OnApplicationStart()
        {
            instance = this;

            bundle = GetBundle();
            bundleObjects = bundle.LoadAllAssets();

            foreach(Object obj in bundleObjects)
            {
                obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }
        }

        public override void OnApplicationLateStart() => new Core.ScoreworksManager();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Core.Data.SWHighScore highScore = new Core.Data.SWHighScore()
            {
                currentScene = Core.ScoreworksManager.instance.currentScene,
                highScore = Core.ScoreworksManager.instance.currentHighScore
            };

            Core.ScoreworksManager.instance.SaveHighScore(highScore);

            Core.ScoreworksManager.instance.OnLevelChange(sceneName);
            Core.ScoreworksManager.instance.currentScore = 0;
            Core.ScoreworksManager.instance.currentMultiplier = 0f;

            director = new Core.Director();
            new Audio.AudioManager();

            uiObject = GameObject.Instantiate(GetObject("SWHud").Cast<GameObject>());
            uiManager = uiObject.AddComponent<UI.UIManager>();
        }

        public override void OnUpdate() => Core.ScoreworksManager.instance?.Update();

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
