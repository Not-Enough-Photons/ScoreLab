using System;

using NEP.ScoreLab.Data;
using NEP.ScoreLab.UI;

namespace NEP.ScoreLab.Core
{
    public static class API
    {
        public static class Score
        {
            public static Action<PackedScore> OnScoreAdded;
        }

        public static class Multiplier
        {
            public static Action<PackedMultiplier> OnMultiplierAdded;
            public static Action<PackedMultiplier> OnMultiplierRemoved;

            public static Action<PackedMultiplier> OnMultiplierTimeBegin;
            public static Action<PackedMultiplier> OnMultiplierTimeExpired;
        }

        public static class HighScore
        {
            public static Action<PackedHighScore> OnHighScoreReached;
            public static Action<PackedHighScore> OnHighScoreUpdated;
            public static Action<PackedHighScore> OnHighScoreLoaded;
            public static Action<PackedHighScore> OnHighScoreSaved;
        }

        public static class GameConditions
        {
            public static Func<bool> IsPlayerMoving = new Func<bool>(() => true);
            public static Func<bool> IsPlayerInAir = new Func<bool>(() => Emulator._testCondition);
        }

        public static class UI
        {
            public static Action<UIModule> OnModuleEnabled;
            public static Action<UIModule> OnModuleDisabled;

            public static Action<UIModule> OnModuleDecayed;
            public static Action<UIModule> OnModulePostDecayed;
        }
    }
}

