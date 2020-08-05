using UnityEngine;

public class System_Equation : MonoBehaviour
{
    internal delegate void EquationsRightDelegate();
    internal EquationsRightDelegate RightEvent;
    internal Equation[] Equations;

    void Awake()
    {
        Equations = GetComponentsInChildren<Equation>();
        for (int i = 0; i < Equations.Length; ++i)
        {
            Equations[i].checkAnswersEvent = CheckRight;
            Equations[i].Initialise(i);
        }
    }
    internal void CheckRight()
    {
        bool b = true;
        foreach (Equation Eq in Equations)
        {
            if (!Eq.isRight)
            {
                b = false;
                break;
            }
        }
        if (b)
        {
            RightEvent();
        }
    }
}
