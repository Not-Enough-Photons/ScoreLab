using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.Scoreworks.Core.Data
{
    public static class DataManager
    {
        public static Dictionary<SWScoreType, SWValueTemplate> scoreValues { get; private set; }
        public static Dictionary<SWMultiplierType, SWValueTemplate> multiplierValues { get; private set; }

        public static Dictionary<string, SWHighScore> highScoreTable { get; private set; }

        private static string displayValueRegexPattern = @"\%(name|currentscore|score|highscore|currentmultiplier|multiplier|scenename)%";

        public static void Initialize()
        {
            highScoreTable = new Dictionary<string, SWHighScore>();

            BuildScoreValues();
            BuildMultiplierValues();
            LoadHighScoreTable(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json");
        }

        // High Scores
        public static void SaveHighScore(string currentScene, int score)
        {
            if (highScoreTable.ContainsKey(currentScene))
            {
                highScoreTable[currentScene].currentScene = ScoreworksManager.instance.currentScene;
                highScoreTable[currentScene].highScore = ScoreworksManager.instance.currentHighScore;
            }
            else
            {
                SWHighScore highScore = new SWHighScore()
                {
                    currentScene = currentScene,
                    highScore = score
                };

                highScoreTable.Add(currentScene, highScore);
            }

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json", serialized);
        }

        public static SWHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }

        public static void LoadHighScoreTable(string path)
        {
            string data = System.IO.File.ReadAllText(path);
            highScoreTable = JsonConvert.DeserializeObject<Dictionary<string, SWHighScore>>(data);
        }

        public static void DeleteHighScore()
        {
            // Already empty
            if(highScoreTable.Count <= 0)
            {
                return;
            }

            var manager = ScoreworksManager.instance;

            highScoreTable.Remove(manager.currentSceneLiteral);

            // Clear high score from the manager

            manager.currentHighScore = 0;

            // Save the data to the file

            SaveHighScore(manager.currentScene, 0);
        }

        public static void DeleteAllHighScores()
        {
            highScoreTable.Clear();

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json", serialized);
        }

        public static void BuildScoreValues()
        {
            scoreValues = new Dictionary<SWScoreType, SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_score_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SWScoreType, SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SWScoreType));

            foreach (string name in enumNames)
            {
                SWScoreType scoreType = (SWScoreType)Enum.Parse(typeof(SWScoreType), name);
                SWValueTemplate template = dictionary[scoreType];
                scoreValues?.Add(scoreType, template);
            }
        }

        public static void BuildMultiplierValues()
        {
            multiplierValues = new Dictionary<SWMultiplierType, SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_mult_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SWMultiplierType, SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SWMultiplierType));

            foreach (string name in enumNames)
            {
                Data.SWMultiplierType multType = (SWMultiplierType)Enum.Parse(typeof(SWMultiplierType), name);
                Data.SWValueTemplate template = dictionary[multType];
                multiplierValues?.Add(multType, template);
            }
        }

        // UI Data
        public static UI.UIPadding ReadPadding()
        {
            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json";
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
            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json";
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

            System.IO.File.WriteAllText(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json", data);
        }

        public static string OutputDisplayTextFromRegex(string input, SWValue value)
        {
            Match match = Regex.Match(input, displayValueRegexPattern);

            switch (match.Value)
            {
                case "%name%":
                    input = input.Replace(match.Value, value.name);
                    break;
                case "%currentscore%":
                    input = input.Replace(match.Value, ScoreworksManager.instance.currentScore.ToString());
                    break;
                case "%score%":
                    input = input.Replace(match.Value, value.score.ToString());
                    break;
                case "%highscore":
                    input = input.Replace(match.Value, ScoreworksManager.instance.currentHighScore.ToString());
                    break;
                case "%currentmultiplier":
                    input = input.Replace(match.Value, ScoreworksManager.instance.currentMultiplier.ToString());
                    break;
                case "%multiplier%":
                    input = input.Replace(match.Value, value.multiplier.ToString());
                    break;
                case "%scenename":
                    input = input.Replace(match.Value, ScoreworksManager.instance.currentScene.ToString());
                    break;
            }

            return input;
        }

        public static string OutputDisplayTextFromRegex(string input, string name = "", int score = 0, float multiplier = 0f)
        {
            Match match = Regex.Match(input, displayValueRegexPattern);

            while (match.Success)
            {
                switch (match.Value)
                {
                    case "%n%":
                        input = input.Replace(match.Value, name);
                        break;
                    case "%cs%":
                        input = input.Replace(match.Value, ScoreworksManager.instance.currentScore.ToString());
                        break;
                    case "%s%":
                        input = input.Replace(match.Value, score.ToString());
                        break;
                    case "%hs%":
                        input = input.Replace(match.Value, ScoreworksManager.instance.currentHighScore.ToString());
                        break;
                    case "%cm%":
                        input = input.Replace(match.Value, ScoreworksManager.instance.currentMultiplier.ToString());
                        break;
                    case "%m%":
                        input = input.Replace(match.Value, multiplier.ToString());
                        break;
                    case "%sn%":
                        input = input.Replace(match.Value, ScoreworksManager.instance.currentScene.ToString());
                        break;
                }

                match = match.NextMatch();
            }

            return input;
        }
    }

}