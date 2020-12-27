

public enum StarshipType
{
    PlayerStarship_D,
    PlayerStarship_C,
    Starship_Friend_D,
    Starship_Enemy_D,
    Starship_Friend_C,
    Starship_Enemy_C
}

/// <summary>Содержит данные какого-либо корабля.</summary>
public class StarshipData
{

    private ModificationData[] ModificationDatas = new ModificationData[4];

    public StarshipData(ModificationData[] ModificationDatas)
    {
        this.ModificationDatas = ModificationDatas;
    }

    public ModificationData GetModificationData(ModificationName modificationName)
    {
        switch (modificationName)
        {
            case ModificationName.Health: return ModificationDatas[0];
            case ModificationName.EnginePower: return ModificationDatas[1];
            case ModificationName.GyroPower: return ModificationDatas[2];
            case ModificationName.ShootTime: return ModificationDatas[3];
            default: return ModificationDatas[0];
        }
    }

    public void ResetData()
    {
        for (int i = 0; i < ModificationDatas.Length; ++i)
        {
            ModificationDatas[i].ResetData();
        }
    }

    public int[] GetStarshipDataForSave()
    {
        int[] vs = new int[ModificationDatas.Length];
        for (int i = 0; i < ModificationDatas.Length; ++i)
        {
            vs[i] = ModificationDatas[i].curGrade;
        }
        return vs;
    }

    internal void SetStarshipData(int[] vs)
    {
        for (int i = 0; i < ModificationDatas.Length && i < vs.Length; ++i)
        {
            ModificationDatas[i].UpgradeToGrade(vs[i]);
        }
    }
}