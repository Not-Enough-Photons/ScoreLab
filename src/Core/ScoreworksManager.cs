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

        public Action<Data.SWValue> OnScoreAdded;
        public Action<Data.SWValue> OnScoreRemoved;

        public Action<Data.SWValue> OnScoreChanged;
        public Action<Data.SWValue> OnMultiplierChanged;

        public Action<Data.SWValue> OnMultiplierAdded;
        public Action<Data.SWValue> OnMultiplierRemoved;

        public static Dictionary<Data.SWScoreType, Data.SWValueTemplate> scoreValues;
        public static Dictionary<Data.SWMultiplierType, Data.SWValueTemplate> multValues;

        public static Dictionary<string, Data.SWHighScore> highScoreTable = new Dictionary<string, Data.SWHighScore>();

        public int currentScore;
        public float currentMultiplier;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            OnMultiplierRemoved += RemoveMultiplier;

            BuildScoreValues();
            BuildMultiplierValues();
        }

        private void BuildScoreValues()
        {
            scoreValues = new Dictionary<Data.SWScoreType, Data.SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_score_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<Data.SWScoreType, Data.SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(Data.SWScoreType));

            foreach (string name in enumNames)
            {
                Data.SWScoreType scoreType = (Data.SWScoreType)Enum.Parse(typeof(Data.SWScoreType), name);
                Data.SWValueTemplate template = dictionary[scoreType];
                scoreValues?.Add(scoreType, template);
            }
        }

        private void BuildMultiplierValues()
        {
            multValues = new Dictionary<Data.SWMultiplierType, Data.SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_mult_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<Data.SWMultiplierType, Data.SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(Data.SWMultiplierType));

            foreach (string name in enumNames)
            {
                Data.SWMultiplierType multType = (Data.SWMultiplierType)Enum.Parse(typeof(Data.SWMultiplierType), name);
                Data.SWValueTemplate template = dictionary[multType];
                multValues?.Add(multType, template);
            }
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
            }

            if (value.type == Data.SWValueType.Multiplier)
            {
                currentMultiplier += value.multiplier;
            }
        }

        public void RemoveMultiplier(Data.SWValue value)
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

