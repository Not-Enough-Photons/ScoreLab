using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

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

        public int currentScore;
        public float currentMultiplier;

        public int currentHighScore;
        public string currentScene;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            API.OnMultiplierRemoved += RemoveMultiplier;
        }

        public void Update()
        {
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

                if (currentScore >= currentHighScore)
                {
                    currentHighScore = currentScore;
                }
            }

            if (value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier += value.multiplier;
            }
        }

        public void RemoveMultiplier(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier -= value.multiplier;
                API.OnMultiplierChanged?.Invoke(value);
            }
        }
    }
}

