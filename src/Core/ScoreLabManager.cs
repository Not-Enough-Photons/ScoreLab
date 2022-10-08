using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Newtonsoft.Json;

namespace NEP.ScoreLab.Core
{
    public class ScoreLabManager
    {
        public ScoreLabManager()
        {
            Awake();
            Start();
        }

        public static ScoreLabManager instance { get; private set; }

        public static List<Data.SLValue> swValues = new List<Data.SLValue>();

        public static Dictionary<Data.SLScoreType, Data.SLValue> scoreDict;
        public static Dictionary<Data.SLMultiplierType, Data.SLValue> multDict;

        public int currentScore;
        public float currentMultiplier;

        public string currentScene;
        public string currentSceneLiteral;
        public int currentHighScore;

        public List<Data.SLValue> scoreValues { get; private set; }
        public List<Data.SLValue> multiplierValues { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            scoreDict = new Dictionary<Data.SLScoreType, Data.SLValue>();
            multDict = new Dictionary<Data.SLMultiplierType, Data.SLValue>();
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

        public void PreAddScore(Data.SLValue value)
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

        public void AddScore(Data.SLValue value)
        {
            if (value.type == Data.SLValueType.Score)
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

        public void RemoveScore(Data.SLValue value)
        {
            if (value.type == Data.SLValueType.Score)
            {
                swValues.Remove(value);
                scoreDict.Remove(value.scoreType);
            }
        }

        public void PreAddMultiplier(Data.SLValue value)
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

        public void AddMultiplier(Data.SLValue value)
        {
            if (value.type == Data.SLValueType.Multiplier)
            {
                currentMultiplier += value.multiplier;

                if (!multDict.ContainsKey(value.multiplierType))
                {
                    swValues.Add(value);
                    multDict.Add(value.multiplierType, value);
                }
            }
        }

        public void RemoveMultiplier(Data.SLValue value)
        {
            if (value.type == Data.SLValueType.Multiplier)
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

