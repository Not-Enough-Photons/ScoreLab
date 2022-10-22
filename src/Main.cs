using MelonLoader;
using BoneLib;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab
{
    public static class BuildInfo
    {
        public const string Name = "ScoreLab"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "4.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Hooking.OnMarrowGameStarted += OnMarrowGameStarted;
            Hooking.OnMarrowSceneLoaded += OnMarrowSceneLoaded;
        }

        public void OnMarrowGameStarted()
        {
            DataManager.Init();
            ScoreDirector.Patches.InitPatches();

            new ScoreTracker().Initialize();
        }

        public void OnMarrowSceneLoaded(MarrowSceneInfo sceneInfo)
        {
            DataManager.SpawnDefaultUI();
        }
    }
}
