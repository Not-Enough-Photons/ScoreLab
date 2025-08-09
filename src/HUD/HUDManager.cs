#if DEBUG
using NEP.NEDebug.Console;
#endif
using NEP.ScoreLab.Core;
using UnityEngine;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.HUD
{
    public static class HUDManager
    {
        public static List<HUD> LoadedHUDs { get; private set; }
        public static HUD ActiveHUD { get; private set; }
        public static HUD LastHUD { get; private set; }

        private static GameObject _parentObject;
        
        internal static void Initialize()
        {
            _parentObject = new GameObject("[ScoreLab] - HUD Container");
            LoadedHUDs = new List<HUD>();
            Populate();

            if (HUDLoader.LoadedHUDs.ContainsKey(Settings.SavedHUD))
            {
                LoadHUD(Settings.SavedHUD);
            }
            else
            {
                // Load fallback, which is Coda.
                LoadHUD("Coda");
            }
        }

        internal static void Uninitialize()
        {
            
        }

        public static void Populate()
        {
            for(int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                var manifest = HUDLoader.LoadedHUDManifests[i];
                var hudObject = GameObject.Instantiate(HUDLoader.LoadedHUDs[manifest.Name]);
                hudObject.name = manifest.Name;

                var hud = hudObject.GetComponent<HUD>();

                // Not a valid UI
                if(hud == null)
                {
                    continue;
                }

                LoadedHUDs.Add(hud);
                hud.SetParent(_parentObject.transform);
                hud.gameObject.SetActive(false);
            }
        }

        public static void HUDShowModeUpdated(HUDShowMode mode)
        {
            if (ActiveHUD == null)
            {
                return;
            }
            
            if (mode == HUDShowMode.Always)
            {
                ActiveHUD.gameObject.SetActive(true);
            }
            else if (mode == HUDShowMode.Never)
            {
                ActiveHUD.gameObject.SetActive(false);
            }
        }
        
        #if DEBUG
        [NEConsoleCommand("scorelab.load")]
        #endif
        public static void LoadHUD(string name)
        {
            if (ActiveHUD != null)
            {
                LastHUD = ActiveHUD;
            }
            
            UnloadHUD();
            
            foreach (var hud in LoadedHUDs)
            {
                if (hud.name == name)
                {
                    ActiveHUD = hud;
                    break;
                }
            }

            if (ActiveHUD == null)
            {
                return;
            }
            
            Settings.SavedHUD = name;
            ValueManager.UsePackage(name);
            
            HUDShowModeUpdated(Settings.HUDShowMode);
            ActiveHUD.SetParent(null);
        }

        #if DEBUG
        [NEConsoleCommand("scorelab.unload")]
        #endif
        public static void UnloadHUD()
        {
            if(ActiveHUD != null)
            {
                ActiveHUD.SetParent(_parentObject.transform);
                ActiveHUD.gameObject.SetActive(false);
                ActiveHUD = null;
            }
        }

        public static void DestroyHUD()
        {
            if (ActiveHUD != null)
            {
                GameObject.Destroy(ActiveHUD);
                ActiveHUD = null;
            }
        }

        public static void DestroyLoadedHUDs()
        {
            if (LoadedHUDs == null || LoadedHUDs.Count == 0)
            {
                return;
            }

            foreach (var hud in LoadedHUDs)
            {
                GameObject.Destroy(hud.gameObject);
            }
            
            LoadedHUDs.Clear();
        }
    }
}