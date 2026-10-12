using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{

    [Header("Text Elements")]
    public TextMeshProUGUI CompleteConditionText;
    public TextMeshProUGUI EndingScriptText;

    public void ShowVictoryPanel(int anthropoiNumber, int gynaikoeideisNumber)
    {
        CompleteConditionText.text = "Mission Complete";
        EndingScriptText.text = $"당신의 집단은 구조대가 도착할 때까지 무사히 살아남았습니다.\r\n" +
            $"총 {anthropoiNumber}명의 사람이 고향으로 돌아가는 구조선에 탑승했으며\r\n" +
            $"총 {gynaikoeideisNumber}개의 인형이 보존한 자산으로써 화물칸에 적재되었습니다.\r\n\r\n" +
            $"분명 당신은 최선의 지도자였습니다.";

        gameObject.SetActive(true);
    }

    public void ShowFailurePanel(int liveDay, int DDay)
    {
        CompleteConditionText.text = "Mission Failed";
        EndingScriptText.text = $"당신의 지도 아래 집단은 {liveDay}일을 버텨냈지만\r\n" +
            $"계속되는 사망으로 구조까지 {DDay}일을 남기고 결국 무너지고 말았습니다.\r\n\r\n" +
            $"그러나 당신은 분명 최선을 다했습니다.";
        gameObject.SetActive(true);
    }

    public void RestartButtonClicked()
    {
        SceneManager.LoadScene("PrototypeGameplayScene");
    }
}
