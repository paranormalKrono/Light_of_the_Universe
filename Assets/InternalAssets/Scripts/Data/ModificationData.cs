

public class ModificationData
{
    public UpgradeData[] UpgradeDatas { get; }
    public float maxGradeModifier { get; }
    public int curGrade { get; private set; }

    private float curGradeModifier;

    public float CurGradeModifier
    {
        get => curGradeModifier;
        private set
        {
            OnGradeModifierChanged?.Invoke(value);
            curGradeModifier = value;
        }
    }

    public delegate void EventHandler(float value);
    public event EventHandler OnGradeModifierChanged;

    public void UpgradeToGrade(int newGrade)
    {
        while (curGrade < newGrade)
        {
            Upgrade();
        }
    }

    public void Upgrade()
    {
        CurGradeModifier += UpgradeDatas[curGrade].UpgradeValue;
        curGrade += 1;
    }

    public void ResetData()
    {
        curGrade = 0;
        CurGradeModifier = 0;
    }

    public UpgradeData GetCurrentUpgradeData() => UpgradeDatas[curGrade];

    public ModificationData(UpgradeData[] upgradeDatas)
    {
        UpgradeDatas = upgradeDatas;
        for (int i = 0; i < UpgradeDatas.Length; ++i)
        {
            maxGradeModifier += UpgradeDatas[i].UpgradeValue;
        }
    }

    public struct UpgradeData
    {

        public float UpgradeValue { get; }
        public int Cost { get; }

        public UpgradeData(float upgradeValue, int cost)
        {
            UpgradeValue = upgradeValue;
            Cost = cost;
        }

    }
}