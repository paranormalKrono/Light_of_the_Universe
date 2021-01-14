using UnityEngine;

public static class GameCursor
{

    public static void SetCursorLock(bool lockCursor, CursorLockMode cursorLockMode)
    {
        Cursor.visible = !lockCursor;
        Cursor.lockState = cursorLockMode;
    }
}
