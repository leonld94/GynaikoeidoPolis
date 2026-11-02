using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeUIPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject ToolTipBox;

    public void SwitchEscapeUIPanel(bool isEscaped)
    {
        gameObject.SetActive(isEscaped);
    }

    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void DeskTopButtonClicked()
    {
        Application.Quit();
    }

    public void OnToolTipShowClicked()
    {
        ToolTipBox.SetActive(true);
    }
}
