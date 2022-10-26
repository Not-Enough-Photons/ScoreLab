namespace NEP.ScoreLab.Data
{
    public static class EventType
    {
        public static readonly string None = "None";

        public static class Score
        {
            public static readonly string Kill = "SCORE_KILL";
            public static readonly string EnemyMidAirKill = "SCORE_KILL_IN_AIR";
            public static readonly string GameWaveCompleted = "SCORE_WAVE_COMPLETE";
            public static readonly string GameRoundCompleted = "SCORE_ROUND_COMPLETE";
        }

        public static class Mult
        {
            public static readonly string Kill = "MULT_KILL";
            public static readonly string MidAir = "MULT_MIDAIR";
            public static readonly string Seated = "MULT_SEATED";
            public static readonly string SecondWind = "MULT_SECONDWIND";
        }
    }
}