using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private bool isMouseLocked = true;

    private void Start()
    {
        // Lock the cursor to the center of the screen
        LockCursor();
    }

    private void Update()
    {
        // Toggle mouse lock/unlock when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseLocked = !isMouseLocked;
            if (isMouseLocked)
                LockCursor();
            else
                UnlockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
