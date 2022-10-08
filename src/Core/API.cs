using System;

using NEP.ScoreLab.Core.Data;

namespace NEP.ScoreLab.Core
{
    public static class API
    {
        public static Action<SLValue> OnScorePreAdded;
        public static Action<SLValue> OnScoreAdded;

        public static Action<SLValue> OnScorePreRemoved;
        public static Action<SLValue> OnScoreRemoved;
        public static Action<SLValue> OnScoreLateRemoved;

        public static Action<SLValue> OnMultiplierPreAdded;
        public static Action<SLValue> OnMultiplierAdded;

        public static Action<SLValue> OnMultiplierPreRemoved;
        public static Action<SLValue> OnMultiplierRemoved;
        public static Action<SLValue> OnMultiplierLateRemoved;

        public static Action<SLValue> OnScoreChanged;
        public static Action<SLValue> OnMultiplierChanged;

        public static Action<SLValue> OnHighScoreReached;

        public static Action<SLValue> OnScoreDuplicated;
        public static Action<SLValue> OnMultiplierDuplicated;
    }
}