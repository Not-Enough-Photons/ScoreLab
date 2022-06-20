using MelonLoader;

using UnityEngine;

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
        private Core.ScoreworksManager scoreworksManager;
        private Core.Director director;
        private UI.UIManager uiManager;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            scoreworksManager = new Core.ScoreworksManager();
            director = new Core.Director();
            uiManager = new GameObject("UI Manager").AddComponent<UI.UIManager>();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if(scoreworksManager != null)
            {
                scoreworksManager = null;
            }
        }
    }
}
