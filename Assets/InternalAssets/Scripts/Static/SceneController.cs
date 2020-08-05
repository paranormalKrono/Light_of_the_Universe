using UnityEngine.SceneManagement;

public enum Scenes
{
    Prologue_1,
    Prologue_2,
    Prologue_3,
    SM_0_TimeRace1,
    SM_1_1vs1,
    SM_2_3vs3,
    SM_3_5vs5,
    SM_4_Race6,
    SM_5_Tactic_4vs4,
    SM_6_3vs3vs1,
    SM_7_TimeRace2,
    SM_8_3vs8,
    SM_9_6vs4vs4vs4,
    SM_10_Race8,
    SM_11_3vs3vs3,
    SM_12_End,
    Menu,
    Slides,
    Equations,
    Space_Base,
    Space_Base_Bar
}
public enum ScenesLocations
{
    SL_1_2Teams,
    SL_2_3Teams,
    SL_3_Race1,
    SL_4_Race2,
    SL_5_Mission
}

public static class SceneController
{

    static public Scenes[][] MissionStages = new Scenes[][] 
    {
        new Scenes[] 
        { 
            Scenes.SM_0_TimeRace1
        },
        new Scenes[]
        {
            Scenes.SM_1_1vs1,
            Scenes.SM_2_3vs3
        },
        new Scenes[]
        {
            Scenes.SM_3_5vs5,
            Scenes.SM_4_Race6,
            Scenes.SM_5_Tactic_4vs4
        },
        new Scenes[]
        {
            Scenes.SM_6_3vs3vs1
        },
        new Scenes[]
        {
            Scenes.SM_7_TimeRace2,
            Scenes.SM_8_3vs8
        },
        new Scenes[]
        {
            Scenes.SM_9_6vs4vs4vs4,
            Scenes.SM_10_Race8,
            Scenes.SM_11_3vs3vs3
        },
        new Scenes[]
        {
            Scenes.SM_12_End
        }
    };

    static public int GetMissionStage(Scenes scene)
    {
        for (int i = 0; i < MissionStages.Length; ++i)
        {
            for (int j = 0; j < MissionStages[i].Length; ++j)
            {
                if (scene == MissionStages[i][j])
                {
                    return i;
                }
            }
        }
        return 0;
    }

    static public void RestartScene()
    {
        StaticSettings.isRestart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    static public void LoadScene(Scenes scene) => SceneManager.LoadScene(scene.ToString());

    static public void LoadAdditiveScene(ScenesLocations sceneAdditiveName)
    {
        SceneManager.LoadScene(sceneAdditiveName.ToString(), LoadSceneMode.Additive);
    }


    static public void LoadEquationsAndSlides(Scenes scene, int EquationsID, int SlidesID)
    {
        if (StaticSettings.isNoEquations)
        {
            LoadSlides(scene, SlidesID);
        }
        else
        {
            StaticSettings.SlidesID = SlidesID;
            StaticSettings.isNextSlides = true;
            LoadEquations(scene, EquationsID);
        }
    }
    static public void LoadEquations(Scenes scene, int EquationsID)
    {
        if (StaticSettings.isNoEquations)
        {
            LoadScene(scene);
        }
        else
        {
            StaticSettings.EquationsID = EquationsID;
            LoadSceneWithNext(Scenes.Equations, scene);
        }
    }
    static public void LoadSlides()
    {
        LoadScene(Scenes.Slides);
    }
    static public void LoadSlides(Scenes scene, int SlidesID)
    {
        StaticSettings.SlidesID = SlidesID;
        LoadSceneWithNext(Scenes.Slides, scene);
    }


    static private void LoadSceneWithNext(Scenes scene, Scenes nextSceneName)
    {
        StaticSettings.nextSceneToLoad = nextSceneName;
        LoadScene(scene);
    }
}
