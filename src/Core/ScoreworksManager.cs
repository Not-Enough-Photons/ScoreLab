using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public static Dictionary<Data.SWScoreType, Data.SWValue> scoreDict;
        public static Dictionary<Data.SWMultiplierType, Data.SWValue> multDict;

        public int currentScore;
        public float currentMultiplier;

        public string currentScene;
        public string currentSceneLiteral;
        public int currentHighScore;

        public List<Data.SWValue> scoreValues { get; private set; }
        public List<Data.SWValue> multiplierValues { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            scoreDict = new Dictionary<Data.SWScoreType, Data.SWValue>();
            multDict = new Dictionary<Data.SWMultiplierType, Data.SWValue>();
        }

        private void Start()
        {
            currentMultiplier = 1f;

            SubscribeActions();

            Data.DataManager.BuildScoreValues();
            Data.DataManager.BuildMultiplierValues();
        }

        private void SubscribeActions()
        {
            API.OnScorePreAdded += PreAddScore;
            API.OnScoreAdded += AddScore;

            API.OnMultiplierPreAdded += PreAddMultiplier;
            API.OnMultiplierAdded += AddMultiplier;

            API.OnScoreRemoved += RemoveScore;
            API.OnMultiplierRemoved += RemoveMultiplier;
        }

        public void Update()
        {
            for (int i = 0; i < swValues.Count; i++)
            {
                swValues[i]?.Update();
            }
        }

        public void PreAddScore(Data.SWValue value)
        {
            scoreValues = scoreDict.Values.ToList();

            if (value.stack)
            {
                if (scoreDict.ContainsKey(value.scoreType))
                {
                    var target = swValues?.FirstOrDefault((val) => val == scoreDict[value.scoreType]);
                    swValues.Remove(value);
                    target.score += value.score;
                    target.ResetDuration();

                    API.OnScoreDuplicated?.Invoke(target);
                }
            }
        }

        public void AddScore(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Score)
            {
                if (currentMultiplier > 1f)
                {
                    currentScore += Mathf.RoundToInt(value.score * currentMultiplier);
                }
                else
                {
                    currentScore += value.score;
                }

                if (!scoreDict.ContainsKey(value.scoreType))
                {
                    swValues.Add(value);
                    scoreDict.Add(value.scoreType, value);
                }

                if (currentScore >= currentHighScore)
                {
                    currentHighScore = currentScore;
                    API.OnHighScoreReached?.Invoke(value);
                }
            }
        }

        public void RemoveScore(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Score)
            {
                swValues.Remove(value);
                scoreDict.Remove(value.scoreType);
            }
        }

        public void PreAddMultiplier(Data.SWValue value)
        {
            multiplierValues = multDict.Values.ToList();

            if (value.stack)
            {
                if (multDict.ContainsKey(value.multiplierType))
                {
                    var target = swValues?.FirstOrDefault((val) => val == multDict[value.multiplierType]);
                    swValues.Remove(value);
                    target.multiplier += value.multiplier;
                    target.ResetDuration();

                    API.OnMultiplierDuplicated?.Invoke(target);
                }
            }
        }

        public void AddMultiplier(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier += value.multiplier;

                if (!multDict.ContainsKey(value.multiplierType))
                {
                    swValues.Add(value);
                    multDict.Add(value.multiplierType, value);
                }
            }
        }

        public void RemoveMultiplier(Data.SWValue value)
        {
            if (value.type == Data.SWValueType.Multiplier)
            {
                if (currentMultiplier < 1f)
                {
                    currentMultiplier = 1f;
                }
                
                currentMultiplier -= value.multiplier;
                
                swValues.Remove(value);
                multDict.Remove(value.multiplierType);
            }
        }
    }
}

