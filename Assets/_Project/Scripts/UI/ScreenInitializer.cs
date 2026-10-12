using UnityEngine;

public class ScreenInitializer : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(
            1920,
            1080,
            FullScreenMode.FullScreenWindow
        );
    }
}
