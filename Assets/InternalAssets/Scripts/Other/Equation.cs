using UnityEngine.UI;
using UnityEngine;

public class Equation : MonoBehaviour
{
    internal string equation;
    internal string task;

    internal bool isRight;
    internal Text taskText;
    internal Text text;
    internal InputField inputField;
    internal Button button;
    internal Text nothingText;
    internal Text rightText;
    internal Text wrongText;

    [SerializeField] internal int FirstOperandRange1 = 10;
    [SerializeField] internal int FirstOperandRange2 = 50;
    [SerializeField] internal NumberSystem FirstOperandNumberSystem = NumberSystem.Binary;
    [System.Serializable]
    internal class Operand
    {
        [SerializeField] internal int Range1 = 10;
        [SerializeField] internal int Range2 = 50;
        [SerializeField] internal NumberSystem OperandNumberSystem = NumberSystem.Binary;
        [SerializeField] internal Operation operation = Operation.Plus;
    }
    [SerializeField] internal Operand[] Operands;

    internal int Answer;
    [SerializeField] internal NumberSystem AnswerNumberSystem = NumberSystem.Decimal;
    internal string AnswerText;

    internal delegate void CheckAnswersDelegate();
    internal CheckAnswersDelegate checkAnswersEvent;

    internal void Initialise(int ID)
    {
        taskText = transform.Find("Text Task").GetComponent<Text>();
        text = transform.Find("Text Equation").GetComponent<Text>();
        inputField = transform.GetComponentInChildren<InputField>();
        button = transform.GetComponentInChildren<Button>();
        nothingText = transform.Find("Text Gray").GetComponent<Text>();
        rightText = transform.Find("Text Right").GetComponent<Text>();
        wrongText = transform.Find("Text Wrong").GetComponent<Text>();

        Answer = NumberSystemManager.RandomDecimal(FirstOperandRange1, FirstOperandRange2);
        task = (ID + 1) + " выражение: (" + NumberSystemManager.NumberSystemToRussian(FirstOperandNumberSystem) + ")";
        equation = NumberSystemManager.DecimalTo(Answer, FirstOperandNumberSystem) + "(" + NumberSystemManager.NumberSystemToInt(FirstOperandNumberSystem) + ")";

        foreach (Operand operand in Operands)
        {
            int newOperand;
            string Number;
            NumberSystemManager.RandomDecimalTo(operand.Range1, operand.Range2, out newOperand, out Number, operand.OperandNumberSystem);
            switch (operand.operation)
            {
                case Operation.Plus:
                    Answer = Answer + newOperand;
                    task += " + ";
                    equation = equation + " + " + Number;
                    break;
                case Operation.Minus:
                    Answer = Answer - newOperand;
                    task += " - ";
                    equation = equation + " - " + Number;
                    break;
                case Operation.Multiplication:
                    Answer = Answer * newOperand;
                    task += " * ";
                    equation = equation + " * " + Number;
                    break;
            }
            equation += "(" + NumberSystemManager.NumberSystemToInt(operand.OperandNumberSystem) + ")";
            task += "(" + NumberSystemManager.NumberSystemToRussian(operand.OperandNumberSystem) + ")";
        }
        AnswerText = NumberSystemManager.DecimalTo(Answer, AnswerNumberSystem).ToUpper();
        taskText.text = task + " = (" + NumberSystemManager.NumberSystemToRussian(AnswerNumberSystem) + ")";
        text.text = equation.ToUpper() + " =";
    }
    public void UICheckAnswer()
    {
        if (inputField.text != "")
        {
            string s = inputField.text;
            int i = 0;
            while (s.Length > i && s[i] == '0')
            {
                i += 1;
            }
            s = s.Remove(0, i);
            if (AnswerText == s)
            {
                s = s + "(" + NumberSystemManager.NumberSystemToInt(AnswerNumberSystem) + ")";

                inputField.interactable = false;
                button.interactable = false;
                isRight = true;
                nothingText.enabled = false;
                wrongText.enabled = false;
                rightText.enabled = true;

                checkAnswersEvent();
            }
            else
            {
                nothingText.enabled = false;
                wrongText.enabled = true;
            }
            inputField.text = s;
        }
    }
    public enum Operation
    {
        Plus,
        Minus,
        Multiplication
    }
}
