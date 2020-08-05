


public static class SpaceMissions
{
    public static Scenes FirstSpaceMission = Scenes.SM_0_TimeRace1;

    public static SpaceMissionData GetSpaceMissionData(int id) => SpaceMissionDatas[id];
    public static SpaceMissionData GetSpaceMissionData(Scenes scene) => SpaceMissionDatas[scene - FirstSpaceMission];
    public static int GetRewardsTo(Scenes scene)
    {
        int reward = 0;
        for (int i = 0; i < scene - FirstSpaceMission; ++i)
        {
            reward += SpaceMissionDatas[i].reward;
        }
        return reward;
    }

    private static SpaceMissionData[] SpaceMissionDatas = new SpaceMissionData[]
    {
        new SpaceMissionData // SM_0_TimeRace1
        {
            reward = 100
        },
        new SpaceMissionData // SM_1_1vs1
        {
            reward = 130
        },
        new SpaceMissionData // SM_2_3vs3
        {
            reward = 165
        },
        new SpaceMissionData // SM_3_5vs5
        {
            reward = 185
        },
        new SpaceMissionData // SM_4_Race6
        {
            reward = 200
        },
        new SpaceMissionData // SM_5_Tactic_4vs4
        {
            reward = 245
        },
        new SpaceMissionData // SM_6_3vs3vs1
        {
            reward = 750
        },
        new SpaceMissionData // SM_7_TimeRace2
        {
            reward = 890
        },
        new SpaceMissionData // SM_8_3vs8
        {
            reward = 1025
        },
        new SpaceMissionData // SM_9_6vs4vs4vs4
        {
            reward = 1530
        },
        new SpaceMissionData // SM_10_Race8
        {
            reward = 1270
        },
        new SpaceMissionData // SM_11_3vs3vs3
        {
            reward = 1685
        },
        new SpaceMissionData // SM_12_End
        {
            reward = 12000
        }
    };

    public class SpaceMissionData
    {
        public int reward;
    }
}
