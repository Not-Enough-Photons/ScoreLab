using MelonLoader;

using System.Collections.Generic;

namespace NEP.Scoreworks.Utilities
{
    public static class Utils
    {
        public static void InitializeMelonPrefs()
        {
            MelonPreferences.CreateCategory("Scoreworks");
            MelonPreferences.CreateEntry("Scoreworks", "lastHud", "coda.hud");
        }

        public static string GetHUDFromPref()
        {
            return MelonPreferences.GetEntryValue<string>("Scoreworks", "lastHud");
        }

        public static string GetLevelFromSceneName(string currentScene)
        {
            switch (currentScene)
            {
                case "scene_mainMenu": return "Main Menu";
                case "scene_theatrigon_movie01": return "Theatrigon 01";
                case "scene_breakroom": return "Breakroom";
                case "scene_museum": return "Museum of Technical Demonstrations";
                case "scene_streets": return "Streets";
                case "scene_runoff": return "Runoff";
                case "scene_sewerStation": return "Sewers";
                case "scene_warehouse": return "Warehouse";
                case "scene_subwayStation": return "Central Station";
                case "scene_tower": return "Tower";
                case "scene_towerBoss": return "Clock Tower";
                case "scene_theatrigon_movie02": return "Theatrigon 02";
                case "scene_dungeon": return "Fantasy Land Dungeon";
                case "scene_arena": return "Arena (Story)";
                case "scene_throneRoom": return "Throne Room";
                case "sandbox_museumBasement": return "Museum Basement";
                case "sandbox_blankBox": return "Blankbox";
                case "scene_Tuscany": return "Tuscany";
                case "scene_redactedChamber": return "[REDACTED] Chamber";
                case "sandbox_handgunBox": return "Handgun Range";
                case "scene_hoverJunkers": return "Hover Junkers";
                case "arena_fantasy": return "Fantasy Arena";
                case "zombie_warehouse": return "Zombie Warehouse";
                case "custom_map_bbl": return "Custom Map";
                case "MelonVault": return "Melon Vault";
            }

            return "Unknown Scene";
        }
    }
}
