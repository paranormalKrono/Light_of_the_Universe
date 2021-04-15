using UnityEngine;

public class System_Equation : MonoBehaviour
{
    internal Equation[] Equations;

    private void Awake()
    {
        Equations = GetComponentsInChildren<Equation>();
        float dk = EducationStatistics.statistics.GeneralDifficultyCoefficient();
        for (int i = 0; i < Equations.Length; ++i)
        {
            Equations[i].Initialise(i, dk);
        }
    }
    internal void CheckRight()
    {
        foreach (Equation Eq in Equations)
        {
            if (Eq.isRight == false && Eq.isWrong == false)
            {
                EducationStatistics.statistics.ThemeTasksDatas[(int)Eq.theoryTheme].WrongAnswers += 1;
            }
        }
        EducationStatistics.SaveStatistics();
    }
}
