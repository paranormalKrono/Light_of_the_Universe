

public enum ModificationName
{
    Health,
    EnginePower,
    GyroPower,
    ShootTime
}

/// <summary>Содержит данные для всех возможных модификаций кораблей.</summary>
public static class StarshipsModificatonsData
{

    private static StarshipData[] datas = new StarshipData[2]
    {
        new StarshipData( new ModificationData[4]
        {
    new ModificationData
        (new ModificationData.UpgradeData[3] // Health
        {
            new ModificationData.UpgradeData(15, 40),
            new ModificationData.UpgradeData(20, 90),
            new ModificationData.UpgradeData(45, 140)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // EnginePower
        {
            new ModificationData.UpgradeData(30, 60),
            new ModificationData.UpgradeData(70, 110),
            new ModificationData.UpgradeData(120, 160)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // GyroPower
        {
            new ModificationData.UpgradeData(300, 30),
            new ModificationData.UpgradeData(500, 80),
            new ModificationData.UpgradeData(700, 130)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // ShootTime
        {
            new ModificationData.UpgradeData(0.15f, 70),
            new ModificationData.UpgradeData(0.25f, 130),
            new ModificationData.UpgradeData(0.4f, 180)
        })
        }
       ),

        new StarshipData( new ModificationData[4]
        {
    new ModificationData
        (new ModificationData.UpgradeData[3] // Health
        {
            new ModificationData.UpgradeData(30, 320),
            new ModificationData.UpgradeData(50, 510),
            new ModificationData.UpgradeData(70, 800)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // EnginePower
        {
            new ModificationData.UpgradeData(40, 470),
            new ModificationData.UpgradeData(80, 600),
            new ModificationData.UpgradeData(130, 750)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // GyroPower
        {
            new ModificationData.UpgradeData(400, 270),
            new ModificationData.UpgradeData(700, 380),
            new ModificationData.UpgradeData(1100, 450)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3] // ShootTime
        {
            new ModificationData.UpgradeData(0.25f, 700),
            new ModificationData.UpgradeData(0.5f, 850),
            new ModificationData.UpgradeData(0.8f, 1200)
        })
        }
       )
    };

    public static StarshipData GetStarshipData(StarshipType starship)
    {
        switch (starship)
        {
            case StarshipType.PlayerStarship_D: return datas[0];
            case StarshipType.PlayerStarship_C: return datas[1];
            default: return datas[0];
        }
    }

    public static void ResetStarshipDatas()
    {
        for (int i = 0; i < datas.Length; ++i)
        {
            datas[i].ResetData();
        }
    }

    public static int[][] GetStarshipsDataForSave()
    {
        int[][] vs = new int[datas.Length][];
        for (int i = 0; i < datas.Length; ++i)
        {
            vs[i] = datas[i].GetStarshipDataForSave();
        }

        return vs;
    }

    internal static void SetStarshipsData(int[][] currentStarshipsModificationsGrade)
    {
        for (int i = 0; i < datas.Length && i < currentStarshipsModificationsGrade.Length; ++i)
        {
            datas[i].SetStarshipData(currentStarshipsModificationsGrade[i]);
        }
    }
}
