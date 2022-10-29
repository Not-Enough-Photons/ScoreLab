using System;

using NEP.ScoreLab.Data;

using HarmonyLib;

using SLZ.Rig;

using SLZ.Marrow.SceneStreaming;
using SLZ.Marrow.Utilities;
using SLZ.Marrow.Warehouse;

namespace NEP.ScoreLab.Patches
{
    public static class Hooks
    {
        public static class Game
        {
            [HarmonyPatch(typeof(SceneStreamer))]
            [HarmonyPatch(nameof(SceneStreamer.Load), new Type[] { typeof(LevelCrateReference), typeof(LevelCrateReference) })]
            public static class SceneStreamer_Patch
            {
                public static void Postfix(LevelCrateReference level, LevelCrateReference loadLevel)
                {
                    OnSceneMarrowInitialized(level, loadLevel);
                }
            }

            [HarmonyPatch(typeof(RigManager))]
            [HarmonyPatch(nameof(RigManager.Awake))]
            public static class RigManager_AwakePatch
            {
                public static void Postfix(RigManager __instance)
                {
                    OnRigManagerAwake(__instance);
                }
            }

            [HarmonyPatch(typeof(RigManager))]
            [HarmonyPatch(nameof(RigManager.OnDestroy))]
            public static class RigManager_DestroyPatch
            {
                public static void Postfix(RigManager __instance)
                {
                    OnRigManagerDestroy(__instance);
                }
            }

            public static Action OnMarrowGameStarted;

            public static Action<MarrowSceneInfo> OnMarrowSceneInitialized;
            public static Action<MarrowSceneInfo> OnMarrowSceneLoaded;
            public static Action<MarrowSceneInfo, MarrowSceneInfo> OnMarrowSceneUnloaded;

            static MarrowSceneInfo _lastScene;
            static MarrowSceneInfo _currentScene;
            static MarrowSceneInfo _nextScene;

            public static void Initialize()
            {
                MarrowGame.RegisterOnReadyAction(new Action(() => OnMarrowGameStarted?.Invoke()));
            }

            private static void OnSceneMarrowInitialized(LevelCrateReference level, LevelCrateReference loadLevel)
            {
                MarrowSceneInfo info = new MarrowSceneInfo()
                {
                    LevelTitle = level.Crate.Title,
                    Barcode = level.Barcode.ID,
                    MarrowScene = level.Crate.MainAsset.Cast<MarrowScene>()
                };

                _nextScene = info;
                OnMarrowSceneInitialized?.Invoke(info);
            }

            private static void OnSceneMarrowLoaded()
            {
                var level = SceneStreamer.Session.Level;

                MarrowSceneInfo info = new MarrowSceneInfo()
                {
                    LevelTitle = level.Title,
                    MarrowScene = level.MainScene,
                    Barcode = level.Barcode.ID
                };

                _currentScene = info;
                _lastScene = _currentScene;
                OnMarrowSceneLoaded?.Invoke(_currentScene);
            }

            private static void OnSceneMarrowUnloaded()
            {
                OnMarrowSceneUnloaded?.Invoke(_lastScene, _nextScene);
            }

            private static void OnRigManagerAwake(RigManager __instance)
            {
                OnSceneMarrowLoaded();
            }

            private static void OnRigManagerDestroy(RigManager __instance)
            {
                OnSceneMarrowUnloaded();
            }
        }

        public static void Initialize()
        {
            Game.Initialize();
        }

        
    }
}
