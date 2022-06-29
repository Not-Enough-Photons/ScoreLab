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

        public Action<string, int> OnHighScoreUpdated;

        public static Dictionary<Data.SWScoreType, Data.SWValueTemplate> scoreValues;
        public static Dictionary<Data.SWMultiplierType, Data.SWValueTemplate> multValues;

        public Dictionary<string, Data.SWHighScore> highScoreTable;

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
            highScoreTable = new Dictionary<string, Data.SWHighScore>();

            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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

                if(currentScore >= currentHighScore)
                {
                    currentHighScore = currentScore;
                    OnHighScoreUpdated?.Invoke(currentScene, currentHighScore);
                }
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

        public void SaveHighScore(Data.SWHighScore score)
        {
            if (highScoreTable.ContainsKey(score.currentScene))
            {
                highScoreTable[score.currentScene].highScore = currentHighScore;
            }
            else
            {
                highScoreTable.Add(score.currentScene, score);
            }

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json", serialized);
        }

        public void OnLevelChange(string currentScene)
        {
            highScoreTable = LoadHighScoreTable(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json");
            this.currentScene = currentScene;
        }

        public Data.SWHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }

        public Dictionary<string, Data.SWHighScore> LoadHighScoreTable(string path)
        {
            string data = System.IO.File.ReadAllText(path);
            var highScoreTable = JsonConvert.DeserializeObject<Dictionary<string, Data.SWHighScore>>(data);

            return highScoreTable;
        }
    }
}

