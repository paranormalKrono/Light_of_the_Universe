using UnityEngine;

public static class GameCursor
{
    public static bool lockCursor { get; private set; }

    public static void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //Debug.Log(GetCursorState());
    }

    public static string GetCursorState() => $"CursorState: Lock - {Cursor.lockState.ToString()}  Visible - {Cursor.visible.ToString()}";
}
