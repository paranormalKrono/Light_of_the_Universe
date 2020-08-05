using System;

public enum NumberSystem
{
    Binary,
    Octal,
    Hexadecimal,
    Decimal
}
public static class NumberSystemManager
{
    static public string NumberSystemToRussian(NumberSystem numberSystem)
    {
        switch (numberSystem)
        {
            case NumberSystem.Binary:
                return "Двоичная";
            case NumberSystem.Decimal:
                return "Десятичная";
            case NumberSystem.Octal:
                return "Восьмеричная";
            case NumberSystem.Hexadecimal:
                return "Шестнадцатеричная";
            default:
                return "bad def";
        }
    }
    static public string NumberSystemToInt(NumberSystem numberSystem)
    {
        switch (numberSystem)
        {
            case NumberSystem.Binary:
                return "2";
            case NumberSystem.Decimal:
                return "10";
            case NumberSystem.Octal:
                return "8";
            case NumberSystem.Hexadecimal:
                return "16";
            default:
                return "bad def";
        }
    }
    static private string Binary(int dec) => Convert.ToString(dec, 2);
    static private string Octal(int dec) => Convert.ToString(dec, 8);
    static private string Hexadecimal(int dec) => Convert.ToString(dec, 16);

    static public string DecimalTo(int Dec, NumberSystem NST) // Перевод из 10 в другую систему счисления
    {
        switch (NST)
        {
            case NumberSystem.Binary: return Binary(Dec);
            case NumberSystem.Octal: return Octal(Dec);
            case NumberSystem.Hexadecimal: return Hexadecimal(Dec);
            case NumberSystem.Decimal: return Dec.ToString();
            default: return null;
        }
    }
    static public void RandomDecimalTo(int DecRange1, int DecRange2, out int Dec, out string Num, NumberSystem NST)
    {
        int d = RandomDecimal(DecRange1, DecRange2);
        Num = DecimalTo(d, NST);
        Dec = d;
    }

    static public int RandomDecimal(int Range1, int Range2) => UnityEngine.Random.Range(Range1, Range2);
}
