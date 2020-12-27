using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public static class Saves
{

    public static void Initialise()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SavedGames"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SavedGames");
        }
    }


    public static void CreateSaveFile(SaveData saveData) => File.WriteAllText(CreateSaveFilePath(saveData.saveDateTime), JsonConvert.SerializeObject(saveData));
    public static void DeleteSaveFile(DateTime dateTime) => File.Delete(GetSaveFilePath(dateTime));
    public static void LoadSaveFile(DateTime dateTime) => SetSaveData(GetSaveData(dateTime));

    public static SaveData GetSaveData(DateTime dateTime) => JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(GetSaveFilePath(dateTime)));
    public static SaveData[] GetSaveDatas()
    {
        string[] s = GetSaveFilesPaths();
        SaveData[] saveDatas = new SaveData[s.Length];
        for (int i = 0; i < saveDatas.Length; ++i)
        {
            saveDatas[saveDatas.Length - i - 1] = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(s[i]));
        }
        return saveDatas;
    }

    private static string[] GetSaveFilesPaths() => Directory.GetFiles(Application.persistentDataPath + "/SavedGames", "SavedGame_*.json", SearchOption.TopDirectoryOnly);

    public static string CreateSaveFilePath(DateTime dateTime) => Application.persistentDataPath + "/SavedGames/SavedGame_" + dateTime.ToFileTime() + ".json";
    public static string GetSaveFilePath(DateTime dateTime) => Application.persistentDataPath + "/SavedGames/SavedGame_" + dateTime.ToFileTime() + ".json";


    public static void CreateAutosaveFile() => File.WriteAllText(GetAutosaveFilePath(), JsonConvert.SerializeObject(GetCurrentDataToSave()));
    public static void LoadAutosave() => SetSaveData(JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(GetAutosaveFilePath())));
    public static bool GetAutosaveExists() =>  File.Exists(GetAutosaveFilePath());
    public static string GetAutosaveFilePath() => Application.persistentDataPath + "/SavedGames/autosavedGame.json";


    public static SaveData GetCurrentDataToSave()
    {
        SaveData saveData = new SaveData();
        saveData.GameProgress = StaticSettings.GameProgress;
        saveData.CheckpointID = StaticSettings.checkpointID;
        saveData.credits = StaticSettings.credits;
        saveData.currentStarshipsModificationsGrade = StarshipsModificatonsData.GetStarshipsDataForSave();
        saveData.saveDateTime = DateTime.Now;
        return saveData;
    }

    public static void SetSaveData(SaveData saveData)
    {
        StaticSettings.GameProgress = saveData.GameProgress;
        StaticSettings.checkpointID = saveData.CheckpointID;
        StaticSettings.credits = saveData.credits;
        StarshipsModificatonsData.SetStarshipsData(saveData.currentStarshipsModificationsGrade);
    }

    public class SaveData
    {
        public int GameProgress;
        public int CheckpointID;
        public int credits;
        public int[][] currentStarshipsModificationsGrade;
        public DateTime saveDateTime;
    }

}
