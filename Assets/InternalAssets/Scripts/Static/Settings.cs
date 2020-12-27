using System;
using UnityEngine;

public enum AudioMixerExposedParameter
{
    Master,
    GlobalMusic,
    SoundEffects
}

public static class Settings
{
    public const string OPTIONS_KEY_NAME = "SETTINGS";
    public const float SensetivityDefault = 1.6f;

    public static bool isLoaded { get; private set; }
    public static bool isModified { get; set; }
    public static string Packed { get; private set; }


    public static int resolution;
    public static int textureQuality;
    public static int vSyncCount;
    public static int fullScreenMode;
    public static int antiAliasing;
    public static int shadowQuality;
    public static int shadowResolution;
    private static int sensitivity;
    public static int[] volumes = new int[3];

    public static int Sensitivity { get => sensitivity; set { OnSensitivityChanged?.Invoke(value); sensitivity = value; } }

    public delegate void EventHandler(int value);
    public static event EventHandler OnSensitivityChanged;


    public static void Save()
    {

        if (isLoaded)
        {
            //Debug.Log($"Saved");
            Pack();
            PlayerPrefs.SetString(OPTIONS_KEY_NAME, Packed);
            PlayerPrefs.Save();
            isModified = false;
        }

    }

    public static bool TryLoad()
    {
        bool isNormallyLoaded = PlayerPrefs.HasKey(OPTIONS_KEY_NAME);
        if (isNormallyLoaded)
        {
            //Debug.Log("Loading");
            isNormallyLoaded = TryUnpack(PlayerPrefs.GetString(OPTIONS_KEY_NAME));
        }
        else
        {
            //Debug.Log("Creating");
            Pack();
        }
        isLoaded = true;
        return isNormallyLoaded;
    }



    private static void Pack()
    {
        Packed = $"{resolution}_{textureQuality}_{vSyncCount}_{fullScreenMode}_{antiAliasing}_{shadowQuality}_{shadowResolution}_{Sensitivity}";
        for (int i=0; i < volumes.Length; ++i)
        {
            Packed += $"_{volumes[i]}";
        }
        //Debug.Log($"Packed: \"{Packed}\"");

    }

    private static bool TryUnpack(string options)
    {
        //Debug.Log($"Unpacked: \"{options}\"");
        string[] parsed = options.Split('_');
        try
        {
            resolution = int.Parse(parsed[0]);
            textureQuality = int.Parse(parsed[1]);
            vSyncCount = int.Parse(parsed[2]);
            fullScreenMode = int.Parse(parsed[3]);
            antiAliasing = int.Parse(parsed[4]);
            shadowQuality = int.Parse(parsed[5]);
            shadowResolution = int.Parse(parsed[6]);
            Sensitivity = int.Parse(parsed[7]); 
            for (int i = 0; i < volumes.Length; ++i)
            {
                volumes[i] = int.Parse(parsed[8 + i]);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace + "\n\nReseting Settings");
            Save();
            return false;
        }
    }

}