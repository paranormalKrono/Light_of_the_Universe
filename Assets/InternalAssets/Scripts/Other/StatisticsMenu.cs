using UnityEngine;
using UnityEngine.UI;

public class StatisticsMenu : MonoBehaviour
{
    [SerializeField] private Text Expirience;
    [SerializeField] private Text GeneralAssessment;
    [SerializeField] private Text GeneralSolvabilityPercentage;
    [SerializeField] private Text GeneralCorrectAnswers;
    [SerializeField] private Text GeneralWrongAnswers;
    [SerializeField] private Text[] SolvabilityPercentages;
    [SerializeField] private Text[] Assessments;
    [SerializeField] private Text[] CorrectAnswers;
    [SerializeField] private Text[] WrongAnswers;


    private void Awake()
    {
        if (!StaticSettings.isEquations)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        EducationStatistics.Statistics statistics = EducationStatistics.statistics;

        Expirience.text = statistics.Experience.ToString();
        int assessment = statistics.GeneralAssessment();
        GeneralAssessment.text = assessment.ToString(); 
        if (assessment == 2)
        {
            GeneralAssessment.color = Color.red;
        }
        else if (assessment == 3)
        {
            GeneralAssessment.color = Color.yellow;
        }
        else if (assessment == 4)
        {
            GeneralAssessment.color = Color.green;
        }
        else
        {
            GeneralAssessment.color = Color.grey;
        }
        GeneralSolvabilityPercentage.text = ((int)statistics.GeneralSolvabilityPercentage()).ToString() + "%"; 
        
        GeneralCorrectAnswers.text = statistics.GeneralCorrectAnswers().ToString();
        GeneralWrongAnswers.text = statistics.GeneralWrongAnswers().ToString();

        EducationStatistics.Statistics.ThemeTasksData tasksData;
        for (int i = 0; i < 3; ++i)
        {
            tasksData = statistics.ThemeTasksDatas[i];
            SolvabilityPercentages[i].text = ((int)tasksData.SolvabilityPercentage()).ToString() + "%";
            
            assessment = tasksData.Assessment();
            Assessments[i].text = assessment.ToString();
            if (assessment == 2)
            {
                Assessments[i].color = Color.red;
            }
            else if (assessment == 3)
            {
                Assessments[i].color = Color.yellow;
            }
            else if (assessment == 4)
            {
                Assessments[i].color = Color.green;
            }
            else
            {
                Assessments[i].color = Color.grey;
            }

            CorrectAnswers[i].text = tasksData.CorrectAnswers.ToString();
            WrongAnswers[i].text = tasksData.WrongAnswers.ToString();
        }
    }
}
