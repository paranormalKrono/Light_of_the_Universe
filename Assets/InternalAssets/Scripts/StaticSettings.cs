

public static class StaticSettings
{

    public static bool isCompanyMenu; // Меню выбора компании в главном меню игры
    public static bool isPart1Complete;
    public static bool isPart2Complete;
    public static bool isNoEquations = true;
    public static bool isCreatedGameMenus = false;
    public static bool isMenu = false; // Главное меню игры

    // Игровые параметры
    public static int GameProgress;
    public static int MissionStagesProgress;
    public static int credits = 50;
    public static int SlidesID;
    public static int EquationsID;

    public static Scenes nextSceneToLoad;
    public static bool isRestart;
    public static bool isNextSlides;
    public static bool isCompleteSomething;

    public static void ReInitialize()
    {
        SlidesID = 0;
        MissionStagesProgress = 0;
        credits = 50;
        EquationsID = 0;
        GameProgress = 0;
        nextSceneToLoad = 0;
        isRestart = false;
        isCompleteSomething = false;
        StarshipModificatonsData.ResetStarshipDatas();
    }
}