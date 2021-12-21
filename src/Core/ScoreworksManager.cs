using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.Scoreworks.Core
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ScoreworksManager : MonoBehaviour
    {
        public ScoreworksManager(IntPtr ptr) : base(ptr) { }

        public static ScoreworksManager instance { get; private set; }

        public List<Data.SWValue> swValues = new List<Data.SWValue>();

        public static Action<Data.SWValue> OnScoreAdded;
        public static Action<Data.SWValue> OnScoreRemoved;

        public static Action<Data.SWValue> OnMultiplierAdded;
        public static Action<Data.SWValue> OnMultiplierRemoved;

        public static Dictionary<string, Data.SWHighScore> highScoreTable = new Dictionary<string, Data.SWHighScore>();

        public int currentScore { get; private set; }
        public float currentMultiplier { get; private set; }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }

            // Singletons are handled differently
            // in BONEWORKS mods.
            // We instead just recreate the instance and destroy it manually
            // through OnSceneWasLoaded/OnSceneWasUnloaded
        }

        private void OnEnable()
        {
            OnScoreAdded += swValues.Add;
            OnMultiplierAdded += swValues.Add;
        }

        private void OnDisable()
        {
            OnScoreAdded -= swValues.Add;
            OnMultiplierAdded -= swValues.Add;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Data.SWValue val = new Data.SWValue()
                {
                    name = "Some Score Name",
                    score = 100,
                    maxDuration = 5f,
                    type = Data.SWValueType.Score
                };

                SetScore(val);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Data.SWValue val = new Data.SWValue()
                {
                    name = "Test Mult",
                    multiplier = 2.5f,
                    maxDuration = 5f,
                    type = Data.SWValueType.Multiplier
                };

                SetMultiplier(val);
            }

            foreach(Data.SWValue value in swValues)
            {
                value.Update();
            }
        }

        public Data.SWHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }

        private Dictionary<string, Data.SWHighScore> RetrieveTableFromFile(string pathToDir)
        {
            string file = System.IO.File.ReadAllText(pathToDir);
            return highScoreTable = JsonUtility.FromJson(file, typeof(Dictionary<string, Data.SWHighScore>)) as Dictionary<string, Data.SWHighScore>;
        }

        public void SetScore(Data.SWValue scoreType)
        {
            currentScore += scoreType.score;

            OnScoreAdded?.Invoke(scoreType);
        }

        public void SetMultiplier(Data.SWValue multiplierType)
        {
            currentMultiplier += multiplierType.multiplier;

            OnMultiplierAdded?.Invoke(multiplierType);
        }
    }
}

