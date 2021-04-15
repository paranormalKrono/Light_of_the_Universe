

public static class StaticSettings
{

    public static bool isGameComplete;
    public static bool isEquations = true;

    // Игровые параметры
    public static int credits = 50;
    public static int GameProgress;

    public static int checkpointID;

    public static bool isRestart;
    public static bool isCompleteSomething;

    public static void ReInitialize()
    {
        checkpointID = 0;
        credits = 50;
        GameProgress = 0;
        isRestart = false;
        isCompleteSomething = false;
        StarshipsModificatonsData.ResetStarshipDatas();
    }
}