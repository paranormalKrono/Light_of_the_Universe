using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Origin,
    Dialog_1,
    Dialog_2,
    Dialog_3,
    Prologue_1,
    Prologue_2,
    Prologue_3,
    Cutscene,
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
    Meeting,
    End,
    Last_1,
    Last_2,
    Menu,
    Slides,
    Equations,
    Space_Base,
    Space_Base_Bar,
    Abyss
}

public enum ScenesLocations
{
    SL_Mindime,
    SL_Preservation,
    SL_3_Race1,
    SL_4_Race2,
    SL_5_Mission,
    SL_6_Meeting,
    SL_7_End,
    L_Abyss
}


public static class SceneController
{

    static private int slidesID;
    static public int SlidesID { get => slidesID; }

    static private int equationsID;
    static public int EquationsID { get => equationsID; }

    static private int spaceBaseID;
    static public int SpaceBaseID { get => spaceBaseID; }

    static private bool isMenu;
    static public bool IsMenu { get => isMenu; }

    static private bool isCreateSave;


    static public void Initialise()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            isMenu = true;
        }
        SceneManager.activeSceneChanged += OnSceneChanged;
    }


    static public void RestartScene()
    {
        StaticSettings.isRestart = true;
        SceneTransitionTo((Scenes)Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name), false);
    }

    static public void LoadStory()
    {
        StaticSettings.GameProgress = 0;
        SceneTransitionTo(StoryScenesSequence[0], true);
    }

    static public void LoadSceneWithTransition(Scenes scene)
    {
        SceneTransitionTo(scene, false);
    }

    static public void LoadNextStoryScene()
    {
        int progress = StaticSettings.GameProgress;
        Scenes currentScene = (Scenes)Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name, true);

        if (progress == 0)
        {
            if (currentScene == Scenes.Slides)
            {
                progress = GetIndexOfStoryScene(Scenes.Slides);
            }
            else if (currentScene == Scenes.Equations)
            {
                progress = GetIndexOfStoryScene(Scenes.Equations);
            }
            else if (currentScene == Scenes.Space_Base)
            {
                progress = GetIndexOfStoryScene(Scenes.Space_Base);
            }
            StaticSettings.GameProgress = progress;
        }
        if (currentScene == Scenes.Slides)
        {
            DefineRepeatingSceneID(ref slidesID, progress, Scenes.Slides);
            slidesID += 1;
        }
        else if (currentScene == Scenes.Equations)
        {
            DefineRepeatingSceneID(ref equationsID, progress, Scenes.Equations);
        }
        else if (currentScene == Scenes.Space_Base)
        {
            DefineRepeatingSceneID(ref spaceBaseID, progress, Scenes.Space_Base);
        }
        else
        {
            for (int i = 0; i < StoryScenesSequence.Length; ++i)
            {
                if (currentScene == StoryScenesSequence[i])
                {
                    StaticSettings.GameProgress = i;
                    progress = i;
                    break;
                }
            }
            if (progress < StoryScenesSequence.Length)
            {
                DefineRepeatingSceneID(ref slidesID, progress, Scenes.Slides);
                DefineRepeatingSceneID(ref equationsID, progress, Scenes.Equations);
                DefineRepeatingSceneID(ref spaceBaseID, progress, Scenes.Space_Base);
            }
        }
        if (progress >= StoryScenesSequence.Length - 1)
        {
            PlayerPrefs.SetInt("GameComplete", Convert.ToInt32(true));
            SceneTransitionTo(Scenes.Menu, false);
        }
        else
        {
            StaticSettings.GameProgress += 1;
            progress += 1;
            if (StaticSettings.isEquations || StoryScenesSequence[progress] != Scenes.Equations)
            {
                SceneTransitionTo(StoryScenesSequence[progress],true);
            }
            else
            {
                StaticSettings.GameProgress += 1;
                progress += 1;
                SceneTransitionTo(StoryScenesSequence[progress], false);
            }
        }

    }

    static public void LoadSave()
    {
        int progress = StaticSettings.GameProgress;
        DefineRepeatingSceneID(ref slidesID, progress, Scenes.Slides);
        DefineRepeatingSceneID(ref equationsID, progress, Scenes.Equations);
        DefineRepeatingSceneID(ref spaceBaseID, progress, Scenes.Space_Base);
        SceneTransitionTo(StoryScenesSequence[progress], true);
    }



    static private Scenes sceneToLoad;
    static public void SceneTransitionTo(Scenes scene, bool isCreateSave)
    {
        SceneController.isCreateSave = isCreateSave;
        sceneToLoad = scene;
        GameMenu.SetGameCursorLock(true, CursorLockMode.None);
        GameAudio.StopAudioEvent();
        GameScreenDark.DarkEvent(OnDarkEnd);
    }
    static private void OnDarkEnd()
    {
        if (sceneToLoad == Scenes.Menu)
        {
            isMenu = true;
            StaticSettings.ReInitialize();
        }
        else
        {
            isMenu = false;
        }
        GameTimer.DeactivateEvent();

        LoadScene(sceneToLoad);
    }

    static public void OnSceneChanged(Scene scene1, Scene scene2)
    {
        GameMenu.SetGameCursorLock(false, CursorLockMode.None);
        GameScreenDark.TransparentEvent(OnTransparentEnd);
    }
    static private void OnTransparentEnd()
    {
        if (isCreateSave)
        {
            isCreateSave = false;
            Saves.CreateAutosaveFile();
        }
    }



    static public int GetIndexOfStoryScene(Scenes scene) => Array.IndexOf(StoryScenesSequence, scene);
    static public Scenes GetNextStoryScene() => StoryScenesSequence[StaticSettings.GameProgress + 1];

    static public void LoadAdditiveScene(ScenesLocations sceneAdditiveName) => SceneManager.LoadScene(sceneAdditiveName.ToString(), LoadSceneMode.Additive); 
    
    static public int GetNextMissionStage()
    {
        for (int t = StaticSettings.GameProgress; t >= 0; ++t)
        {
            for (int i = 0; i < MissionStages.Length; ++i)
            {
                for (int j = 0; j < MissionStages[i].Length; ++j)
                {
                    if (StoryScenesSequence[t] == MissionStages[i][j])
                    {
                        return i;
                    }
                }
            }
        }
        return 0;
    }


    static private void DefineRepeatingSceneID(ref int ID, int sceneProgress, Scenes sceneType)
    {
        ID = 0;
        for (int i = 0; i < sceneProgress; ++i)
        {
            if (StoryScenesSequence[i] == sceneType)
            {
                ID += 1;
            }
        }
    }

    static private void LoadScene(Scenes scene) => SceneManager.LoadScene(scene.ToString());

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


    static public Scenes[] StoryScenesSequence = new Scenes[62]
    {

        // Story 1

        Scenes.Origin,
        Scenes.Slides,
        Scenes.Dialog_1,
        Scenes.Slides,
        Scenes.Prologue_1,
        Scenes.Equations,

        Scenes.Dialog_2,
        Scenes.Slides,
        Scenes.Equations,
        Scenes.Prologue_2,

        Scenes.Slides,
        Scenes.Equations,
        Scenes.Prologue_3,
        Scenes.Dialog_3,
        Scenes.Slides,
        Scenes.Cutscene,
        Scenes.Slides,

        // Story 2

        Scenes.Slides,
        Scenes.Space_Base,
        Scenes.SM_0_TimeRace1,
        Scenes.Equations,
        Scenes.Slides,

        Scenes.Space_Base,

        Scenes.SM_1_1vs1,
        Scenes.Equations,
        Scenes.SM_2_3vs3,
        Scenes.Equations,

        Scenes.Space_Base,

        Scenes.SM_3_5vs5,
        Scenes.Equations,
        Scenes.SM_4_Race6,
        Scenes.Equations,
        Scenes.SM_5_Tactic_4vs4,
        Scenes.Equations,

        Scenes.Space_Base,

        Scenes.SM_6_3vs3vs1,
        Scenes.Equations,

        Scenes.Space_Base,
        Scenes.Slides,
        Scenes.Space_Base,

        Scenes.SM_7_TimeRace2,
        Scenes.Equations,
        Scenes.SM_8_3vs8,
        Scenes.Equations,

        Scenes.Slides,

        Scenes.Space_Base,

        Scenes.SM_9_6vs4vs4vs4,
        Scenes.Equations,
        Scenes.SM_10_Race8,
        Scenes.Equations,
        Scenes.SM_11_3vs3vs3,
        Scenes.Equations,

        Scenes.Space_Base,
        Scenes.Space_Base_Bar,
        Scenes.Space_Base,

        Scenes.SM_12_End,

        // Story 3

        Scenes.Meeting,
        Scenes.End,
        Scenes.Last_1,
        Scenes.Slides,
        Scenes.Last_2,
        Scenes.Slides
    };
}