using MelonLoader;

namespace NEP.Scoreworks.Utilities
{
    public static class Utilities
    {
        private static MelonPreferences_Entry lastHud_Entry;

        public static void InitializeMelonPrefs()
        {
            MelonPreferences.CreateCategory("Scoreworks");
            MelonPreferences.CreateEntry("Scoreworks", "lastHud", "coda.hud");
        }

        public static string GetHUDFromPref()
        {
            return MelonPreferences.GetEntryValue<string>("Scoreworks", "lastHud");
        }
    }
}
