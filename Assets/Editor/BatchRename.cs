using UnityEditor;
using UnityEngine;

public class BatchRename : ScriptableWizard
{
    //Базовое имя
    public string BaseName = "MyObject_";

    // Начальный номер
    public int StartNumber = 0;

    // Шаг
    public int Increment = 1;

    [MenuItem("Edit/Batch Rename...")]
    static void CreateWizard()
    {
        DisplayWizard("Batch Rename", typeof(BatchRename), "Rename");
    }
    //Вызывается, когда изменяется область выбора в сцене
    private void OnEnable()
    {
        UpdateSelectionHelper();
    }

    //Изменяет счётчик выбранных объектов
    void UpdateSelectionHelper()
    {
        helpString = "";

        if (Selection.objects != null)
        {
            helpString = "Number of objects selected: " + Selection.objects.Length;
        }
    }
    //Переименование
    private void OnWizardCreate()
    {
        // Если ничего не выбрано, выйти
        if (Selection.objects == null)
        {
            return;
        }
        // Текущий шаг
        int PostFix = StartNumber;

        // Цикл переименования
        foreach(Object O in Selection.objects)
        {
            O.name = BaseName + PostFix;
            PostFix += Increment;
        }
    }
}
