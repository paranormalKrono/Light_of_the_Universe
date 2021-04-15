using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public enum TheoryThemes
{
    Binary,
    Octal,
    Hexadecimal
}

public static class EducationStatistics
{
    public static Statistics statistics { get; private set; }

    public static void Initialise()
    {
        if (File.Exists(GetStatisticsFilePath()))
        {
            statistics = LoadStatistics();
        }
        else
        {
            statistics = new Statistics();
            SaveStatistics();
        }
    }

    public static void SaveStatistics() => File.WriteAllText(GetStatisticsFilePath(), JsonConvert.SerializeObject(statistics));
    private static Statistics LoadStatistics() => JsonConvert.DeserializeObject<Statistics>(File.ReadAllText(GetStatisticsFilePath()));
    public static string GetStatisticsFilePath() => Application.persistentDataPath + "/Statistics.json";


    


    public class Statistics
    {
        public int Experience; // Опыт

        public ThemeTasksData[] ThemeTasksDatas = new ThemeTasksData[3];
        

        // Общий коэффициент сложности
        public float GeneralDifficultyCoefficient() => DifficultyCoefficient(GeneralSolvabilityPercentage(), Experience);

        // Общая оценка
        public int GeneralAssessment() => ThemeTasksData.Assessment(GeneralSolvabilityPercentage());

        // Общий процент решаемости
        public float GeneralSolvabilityPercentage() => ThemeTasksData.SolvabilityPercentage(GeneralWrongAnswers(), GeneralCorrectAnswers());

        public int GeneralWrongAnswers()
        {
            int WrongAnswers = 0;
            for (int i = 0; i < ThemeTasksDatas.Length; ++i)
            {
                WrongAnswers += ThemeTasksDatas[i].WrongAnswers;
            }
            return WrongAnswers;
        }
        public int GeneralCorrectAnswers()
        {
            int CorrectAnswers = 0;
            for (int i = 0; i < ThemeTasksDatas.Length; ++i)
            {
                CorrectAnswers += ThemeTasksDatas[i].CorrectAnswers;
            }
            return CorrectAnswers;
        }



        // Оценка по теме
        public float GetAssessmentByTheme(TheoryThemes theoryThemes) => ThemeTasksData.Assessment(GetSolvabilityPercentageByTheme(theoryThemes));

        // Процент решаемости по теме
        public float GetSolvabilityPercentageByTheme(TheoryThemes theoryThemes) => GetThemeTasksData(theoryThemes).SolvabilityPercentage();

        // Получение данных по теме
        public ThemeTasksData GetThemeTasksData(TheoryThemes theoryThemes)
        {
            switch (theoryThemes)
            {
                case TheoryThemes.Binary:
                    return ThemeTasksDatas[0];
                case TheoryThemes.Octal:
                    return ThemeTasksDatas[1];
                case TheoryThemes.Hexadecimal:
                    return ThemeTasksDatas[2];
                default:
                    return ThemeTasksDatas[0];
            }
        }



        // Коэффициент сложности
        public static float DifficultyCoefficient(float SolvabilityPercentage, int Experience)
        {
            if (SolvabilityPercentage / 50 * ((float)Experience / 100) > 0.01f)
            {
                return SolvabilityPercentage / 50 * ((float)Experience / 100);
            }
            else
            {
                return 1;
            }
        }



        public struct ThemeTasksData
        {
            public int CorrectAnswers;
            public int WrongAnswers;

            public int Assessment() => Assessment(SolvabilityPercentage());
            public float SolvabilityPercentage() => SolvabilityPercentage(WrongAnswers, CorrectAnswers);

            // Процент решаемости
            public static float SolvabilityPercentage(int WrongAnswers, int CorrectAnswers)
            {
                if (WrongAnswers > 0 || CorrectAnswers > 0)
                {
                    return 100 / (float)(CorrectAnswers + WrongAnswers) * CorrectAnswers;
                }
                else
                {
                    return 0;
                }
            }
            
            // Оценка
            public static int Assessment(float SolvabilityPercentage)
            {
                if (SolvabilityPercentage < 41)
                {
                    return 2;
                }
                else if (SolvabilityPercentage < 65)
                {
                    return 3;
                }
                else if (SolvabilityPercentage < 86)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
        }
    }
}
