using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Core.Data
{
    public static class DataManager
    {
        public static Dictionary<SLScoreType, SLValueTemplate> scoreValues { get; private set; }
        public static Dictionary<SLMultiplierType, SLValueTemplate> multiplierValues { get; private set; }

        public static Dictionary<string, SLHighScore> highScoreTable { get; private set; }

        public static string dir = MelonLoader.MelonUtils.UserDataDirectory + "/Not Enough Photons";

        public static void Initialize()
        {
            highScoreTable = new Dictionary<string, SLHighScore>();

            BuildScoreValues();
            BuildMultiplierValues();
            LoadHighScoreTable(dir + "/ScoreLab/sl_highscores.json");
        }

        // High Scores
        public static void SaveHighScore(string currentScene, int score)
        {
            if (highScoreTable.ContainsKey(currentScene))
            {
                highScoreTable[currentScene].currentScene = ScoreLabManager.instance.currentScene;
                highScoreTable[currentScene].highScore = ScoreLabManager.instance.currentHighScore;
            }
            else
            {
                SLHighScore highScore = new SLHighScore()
                {
                    currentScene = currentScene,
                    highScore = score
                };

                highScoreTable.Add(currentScene, highScore);
            }

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(dir + "/ScoreLab/sl_highscores.json", serialized);
        }

        public static SLHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }

        public static void LoadHighScoreTable(string path)
        {
            string data = System.IO.File.ReadAllText(path);
            highScoreTable = JsonConvert.DeserializeObject<Dictionary<string, SLHighScore>>(data);
        }

        public static void DeleteHighScore()
        {
            // Already empty
            if(highScoreTable.Count <= 0)
            {
                return;
            }

            var manager = ScoreLabManager.instance;

            highScoreTable.Remove(manager.currentSceneLiteral);

            // Clear high score from the manager

            manager.currentScore = 0;
            manager.currentHighScore = 0;

            // Save the data to the file

            SaveHighScore(manager.currentScene, 0);

            // so fucking hacky but it'll do for now
            UI.UIManager uiManager = GameObject.FindObjectOfType<UI.UIManager>();

            if (uiManager)
            {
                uiManager.scoreModule.valueText.text = "0";
            }

            API.OnHighScoreReached?.Invoke(null);
        }

        public static void DeleteAllHighScores()
        {
            highScoreTable.Clear();

            var manager = ScoreLabManager.instance;

            manager.currentScore = 0;
            manager.currentHighScore = 0;

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(dir + "/ScoreLab/sl_highscores.json", serialized);

            // so fucking hacky but it'll do for now
            UI.UIManager uiManager = GameObject.FindObjectOfType<UI.UIManager>();

            if (uiManager)
            {
                uiManager.scoreModule.valueText.text = "0";
            }

            API.OnHighScoreReached?.Invoke(null);
        }

        public static void BuildScoreValues()
        {
            scoreValues = new Dictionary<SLScoreType, SLValueTemplate>();

            string path = dir + "/ScoreLab/sl_score_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SLScoreType, SLValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SLScoreType));

            foreach (string name in enumNames)
            {
                SLScoreType scoreType = (SLScoreType)Enum.Parse(typeof(SLScoreType), name);
                SLValueTemplate template = dictionary[scoreType];
                scoreValues?.Add(scoreType, template);
            }
        }

        public static void BuildMultiplierValues()
        {
            multiplierValues = new Dictionary<SLMultiplierType, SLValueTemplate>();

            string path = dir + "/ScoreLab/sl_mult_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SLMultiplierType, SLValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SLMultiplierType));

            foreach (string name in enumNames)
            {
                Data.SLMultiplierType multType = (SLMultiplierType)Enum.Parse(typeof(SLMultiplierType), name);
                Data.SLValueTemplate template = dictionary[multType];
                multiplierValues?.Add(multType, template);
            }
        }

        // UI Data
        public static UI.UIPadding ReadPadding()
        {
            string path = dir + "/ScoreLab/hud_settings.json";
            string file = System.IO.File.ReadAllText(path);

            var hudData = JObject.Parse(file);

            var filePaddingSettings = hudData["padding"];

            if (filePaddingSettings["leftPadding"] == null)
            {
                return null;
            }

            float[] paddingLeft = new float[3]
            {
                (float)filePaddingSettings["leftPadding"][0],
                (float)filePaddingSettings["leftPadding"][1],
                (float)filePaddingSettings["leftPadding"][2],
            };

            if (filePaddingSettings["rightPadding"] == null)
            {
                return null;
            }

            float[] paddingRight = new float[3]
            {
                (float)filePaddingSettings["rightPadding"][0],
                (float)filePaddingSettings["rightPadding"][1],
                (float)filePaddingSettings["rightPadding"][2],
            };

            if (filePaddingSettings["topPadding"] == null)
            {
                return null;
            }

            float[] paddingTop = new float[3]
            {
                (float)filePaddingSettings["topPadding"][0],
                (float)filePaddingSettings["topPadding"][1],
                (float)filePaddingSettings["topPadding"][2],
            };

            if (filePaddingSettings["bottomPadding"] == null)
            {
                return null;
            }

            float[] paddingBottom = new float[3]
            {
                (float)filePaddingSettings["bottomPadding"][0],
                (float)filePaddingSettings["bottomPadding"][1],
                (float)filePaddingSettings["bottomPadding"][2],
            };

            var padding = new UI.UIPadding()
            {
                leftPadding = paddingLeft,
                rightPadding = paddingRight,
                topPadding = paddingTop,
                bottomPadding = paddingBottom,
            };

            return padding;
        }

        public static UI.UISettings ReadHUDSettings()
        {
            string path = dir + "/ScoreLab/hud_settings.json";
            string file = System.IO.File.ReadAllText(path);

            var hudData = JObject.Parse(file);

            UI.UISettings hudSettings = new UI.UISettings()
            {
                followDistance = (float)hudData["followDistance"],
                followLerp = (float)hudData["followLerp"],
            };

            return hudSettings;
        }

        public static void SaveHUDSettings()
        {
            UI.UIManager manager = GameObject.FindObjectOfType<UI.UIManager>();

            var data = JsonConvert.SerializeObject(manager.hudSettings, Formatting.Indented);

            System.IO.File.WriteAllText(dir + "/ScoreLab/hud_settings.json", data);
        }

        public static string GetLastHUD()
        {
            string path = dir + "/ScoreLab/sw_lasthud.txt";
            string data = System.IO.File.ReadAllText(path);
            return data;
        }

        public static void SaveLastHUD(string hud)
        {
            string path = dir + "/ScoreLab/sw_lasthud.txt";
            System.IO.File.WriteAllText(path, hud);
        }
    }
}