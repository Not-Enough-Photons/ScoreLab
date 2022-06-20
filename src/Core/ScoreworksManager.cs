using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.Scoreworks.Core
{
    public class ScoreworksManager
    {
        public ScoreworksManager()
        {
            Awake();
            Start();
        }

        public static ScoreworksManager instance { get; private set; }

        public static List<Data.SWValue> swValues = new List<Data.SWValue>();

        public static Action<Data.SWValue> OnScoreAdded;
        public static Action<Data.SWValue> OnScoreRemoved;

        public static Action<Data.SWValue> OnScoreChanged;
        public static Action<Data.SWValue> OnMultiplierChanged;

        public static Action<Data.SWValue> OnMultiplierAdded;
        public static Action<Data.SWValue> OnMultiplierRemoved;

        public static Dictionary<string, Data.SWHighScore> highScoreTable = new Dictionary<string, Data.SWHighScore>();

        public int currentScore;
        public float currentMultiplier;

        private string[] test = new string[]
        {
            "KILL",
            "HEADSHOT",
            "FLAG CAPTURED",
            "TEAM SCORE",
            "SLOW-MO KILL"
        };

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            OnMultiplierRemoved += Test;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                new Data.SWValue(test[UnityEngine.Random.Range(0, test.Length)], 20);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                new Data.SWValue(test[UnityEngine.Random.Range(0, test.Length)], 0.5f, UnityEngine.Random.Range(1f, 5f));
            }

            for(int valueIndex = 0; valueIndex < swValues.Count; valueIndex++)
            {
                Data.SWValue current = swValues[valueIndex];

                if(current == null)
                {
                    continue;
                }

                current?.Update();
            }
        }

        public void AddValues(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Score)
            {
                currentScore += value.score;
            }

            if (value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier += value.multiplier;
            }
        }

        public void Test(Data.SWValue value)
        {
            if(value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier -= value.multiplier;
                OnMultiplierChanged?.Invoke(value);
            }
        }

        public Data.SWHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }
    }
}

