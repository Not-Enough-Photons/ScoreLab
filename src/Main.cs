using MelonLoader;

using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.Patches;

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
        public override void OnLateInitializeMelon()
        {
            Hooks.Initialize();

            Hooks.Game.OnMarrowGameStarted += OnMarrowGameStarted;
            Hooks.Game.OnMarrowSceneLoaded += OnMarrowSceneLoaded;
        }

        public void OnMarrowGameStarted()
        {
            DataManager.Init();
            ScoreDirector.Patches.InitPatches();
            new ScoreTracker().Initialize();
        }

        public void OnMarrowSceneLoaded(MarrowSceneInfo sceneInfo)
        {
            new GameObject("[ScoreLab] - UI Manager").AddComponent<UI.UIManager>();
            new GameObject("[ScoreLab] - Audio Manager").AddComponent<Audio.AudioManager>();
        }

        public override void OnUpdate()
        {
            if(ScoreTracker.Instance == null)
            {
                return;
            }

            ScoreTracker.Instance.Update();
        }
    }
}