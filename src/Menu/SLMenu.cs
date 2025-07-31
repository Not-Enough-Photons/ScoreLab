using UnityEngine;
using BoneLib.BoneMenu;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;

namespace NEP.ScoreLab.Menu
{
    public static class SLMenu
    {
        private static Page _hudPage;
        
        public static void Initialize()
        {
            Page root = Page.Root.CreatePage("Not Enough Photons", Color.white);
            Page modPage = root.CreatePage("ScoreLab", Color.white);
            Page scorePage = modPage.CreatePage("Scores", Color.white);
            _hudPage = modPage.CreatePage("HUDs", Color.white);

            #if DEBUG
            modPage.CreateFunction("Reload HUDs", Color.white, () => HUDLoader.ReloadHUDs());
            #endif
            modPage.CreateBool("Audio", Color.white, Settings.UseAnnouncer, (value) => Settings.SetUseAnnouncer(value));
            modPage.CreateFloat("HUD Distance", Color.white, 1.125f, 0.025f, 0f, 2f,
                (value) => Settings.DistanceToCamera = value);
            modPage.CreateEnum("Show HUD", Color.white, Settings.HUDShowMode, (value) => Settings.SetHUDShowMode((HUDShowMode)value));
            
            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => HUDManager.LoadHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }

            scorePage.CreateFunction("Clear High Score", Color.red, () =>
            {
                BoneLib.BoneMenu.Menu.DisplayDialog(
                    "Clear High Score", 
                    "Clear the high score for this level? This action cannot be undone.",
                    null,
                    () => { ScoreTracker.ResetHighScore(); });
            });
            
            scorePage.CreateFunction("Clear All High Scores", Color.red, () =>
            {
                BoneLib.BoneMenu.Menu.DisplayDialog(
                    "Clear High Score", 
                    "Clear all high scores? This action cannot be undone.",
                    null,
                    () => { ScoreTracker.ResetAll(); });
            });
        }

        public static void RefreshHUDPage()
        {
            _hudPage.RemoveAll();
            
            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => HUDManager.LoadHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }
        }
    }
}