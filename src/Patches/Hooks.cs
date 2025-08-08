using BoneLib;
using Il2CppSLZ.Marrow.PuppetMasta;
using NEP.ScoreLab.Data;

using Il2CppSLZ.Marrow.Utilities;

namespace NEP.ScoreLab.Patches
{
    public static class Hooks
    {
        public static class Game
        {
            public static Action OnMarrowGameStarted;
            
            public static Action<MarrowSceneInfo> OnMarrowSceneLoaded;

            static MarrowSceneInfo _lastScene;
            static MarrowSceneInfo _currentScene;
            static MarrowSceneInfo _nextScene;

            public static void Initialize()
            {
                MarrowGame.RegisterOnReadyAction(new Action(() => OnMarrowGameStarted?.Invoke()));
                BoneLib.Hooking.OnLevelLoaded += OnSceneMarrowLoaded;
            }
            
            private static void OnSceneMarrowLoaded(LevelInfo info)
            {
                MarrowSceneInfo marrowSceneInfo = new MarrowSceneInfo()
                {
                    Barcode = info.barcode,
                    LevelTitle = info.title,
                    MarrowScene = info.levelReference.Crate.MainScene
                };
                
                _currentScene = marrowSceneInfo;
                _lastScene = _currentScene;
                OnMarrowSceneLoaded?.Invoke(_currentScene);
            }
        }
        
        public static void Initialize()
        {
            Game.Initialize();
        }
    }
}
