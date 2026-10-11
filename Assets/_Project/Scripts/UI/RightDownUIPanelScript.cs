using UnityEngine;
using static SimulationController;
using TMPro;

public class RightDownUIPanelScript : MonoBehaviour
{
    [Header("Button Elements")]
    public GameObject EnabledPauseButton;
    public GameObject DisabledPauseButton;
    public GameObject EnabledSpeed1Button;
    public GameObject DisabledSpeed1Button;
    public GameObject Speed1ButtonHighlight;
    public GameObject EnabledSpeed2Button;
    public GameObject DisabledSpeed2Button;
    public GameObject Speed2ButtonHighlight;
    public GameObject EnabledSpeed3Button;
    public GameObject DisabledSpeed3Button;
    public GameObject Speed3ButtonHighlight;
    public GameObject EnabledSpeed6Button;
    public GameObject DisabledSpeed6Button;
    public GameObject Speed6ButtonHighlight;

    [Header("Text Elements")]
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI RescueDayText;

    public void Initialize(GameState gameState, SimulationClock clock,
        bool isPaused, TimeSpeed gameSpeed)
    {
        TimeSwitchChanged(isPaused, gameSpeed);
        TimeUIRefresh(clock.PureDay, clock.Hour);
        RescueDayUIRefresh(GameState.RescueArrivalDay - clock.PureDay);
    }

    public void TimeSwitchChanged(bool isPaused, TimeSpeed gameSpeed)
    {
        //Debug.Log("RightDownUIPanelScript.TimeSwitchCahged() Called");

        EnabledPauseButton.SetActive(false);
        EnabledSpeed1Button.SetActive(false);
        EnabledSpeed2Button.SetActive(false);
        EnabledSpeed3Button.SetActive(false);
        EnabledSpeed6Button.SetActive(false);

        DisabledPauseButton.SetActive(true);
        DisabledSpeed1Button.SetActive(true);
        DisabledSpeed2Button.SetActive(true);
        DisabledSpeed3Button.SetActive(true);
        DisabledSpeed6Button.SetActive(true);
        
        Speed1ButtonHighlight.SetActive(false);
        Speed2ButtonHighlight.SetActive(false);
        Speed3ButtonHighlight.SetActive(false);
        Speed6ButtonHighlight.SetActive(false);

        switch (gameSpeed)
        {
            case TimeSpeed.One:
                Speed1ButtonHighlight.SetActive(true);
                break;
            case TimeSpeed.Two:
                Speed2ButtonHighlight.SetActive(true);
                break;
            case TimeSpeed.Three:
                Speed3ButtonHighlight.SetActive(true);
                break;
            case TimeSpeed.Six:
                Speed6ButtonHighlight.SetActive(true);
                break;
        }

        if (isPaused)
        {
            EnabledPauseButton.SetActive(true);
            DisabledPauseButton.SetActive(false);
        }
        else
        {
            switch (gameSpeed)
            {
                case TimeSpeed.One:
                    EnabledSpeed1Button.SetActive(true);
                    DisabledSpeed1Button.SetActive(false);
                    break;
                case TimeSpeed.Two:
                    EnabledSpeed2Button.SetActive(true);
                    DisabledSpeed2Button.SetActive(false);
                    break;
                case TimeSpeed.Three:
                    EnabledSpeed3Button.SetActive(true);
                    DisabledSpeed3Button.SetActive(false);
                    break;
                case TimeSpeed.Six:
                    EnabledSpeed6Button.SetActive(true);
                    DisabledSpeed6Button.SetActive(false);
                    break;
            }
        }
    }

    public void TimeUIRefresh(int day, int hour)
    {
        //Debug.Log("RightDownUIPanelScript.TimeUIRefresh 호출");

        TimeText.text = $"{day}일 {hour}시";
    }

    public void RescueDayUIRefresh(int rescueDay)
    {
        RescueDayText.text = $"D-{rescueDay}";
    }
}
