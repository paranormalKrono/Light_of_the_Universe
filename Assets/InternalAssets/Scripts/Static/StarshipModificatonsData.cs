
public static class StarshipModificatonsData
{

    private static StarshipData[] datas = new StarshipData[2]
    {
        new StarshipData( new ModificationData[4]
        {
    new ModificationData
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(15, 40),
            new ModificationData.UpgradeData(20, 90),
            new ModificationData.UpgradeData(45, 140)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(30, 60),
            new ModificationData.UpgradeData(70, 110),
            new ModificationData.UpgradeData(120, 160)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(40, 30),
            new ModificationData.UpgradeData(60, 80),
            new ModificationData.UpgradeData(100, 130)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
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
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(30, 320),
            new ModificationData.UpgradeData(50, 510),
            new ModificationData.UpgradeData(70, 800)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(40, 470),
            new ModificationData.UpgradeData(80, 600),
            new ModificationData.UpgradeData(130, 750)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
        {
            new ModificationData.UpgradeData(40, 270),
            new ModificationData.UpgradeData(80, 380),
            new ModificationData.UpgradeData(130, 450)
        }),
    new ModificationData
        (new ModificationData.UpgradeData[3]
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
}
