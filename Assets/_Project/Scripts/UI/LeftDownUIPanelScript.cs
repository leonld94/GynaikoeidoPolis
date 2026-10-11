using UnityEngine;
using TMPro;

public class LeftDownUIPanelScript : MonoBehaviour
{
    [Header("PopTap")]
    public TextMeshProUGUI WorkingGynaikoeideisValueText;
    public TextMeshProUGUI ActiveGynaikoeideisValueText;
    public TextMeshProUGUI WorkingAnthropoiValueText;
    public TextMeshProUGUI LivingAnthropoiValueText;
    public TextMeshProUGUI RequiredWorkingMaterialsValueText;

    [Header("GynaikoeideisTap")]
    public TextMeshProUGUI ActiveGynaikoeideisValueText2;
    public TextMeshProUGUI AllGynaikoeideisValueText;
    public TextMeshProUGUI RequiredGynaikoeidiesMaterialsValueText;

    [Header("AnthropoiTap")]
    public TextMeshProUGUI LivingAnthropoiValueText2;
    public TextMeshProUGUI AnthropoiThresholdValueText;
    public TextMeshProUGUI RequiredAnthropoiFoodValueText;
    public TextMeshProUGUI RequiredAnthropoiMaterialsValueText;

    [Header("ConsumeMultipleTap")]
    public TextMeshProUGUI ConsumeMultiplerValueText;

    public GameState GameState { get; set; }

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
        AnthropoiThresholdValueText.text = GameState.AnthropoiDeathLimit.ToString();

        PopUIRefresh();
        HazardLevelUIRefresh();
    }

    public void PopUIRefresh()
    {
        //Pop
        WorkingGynaikoeideisValueText.text = GameState.WorkingGynaikoeideisCount.ToString();
        ActiveGynaikoeideisValueText.text = GameState.ActiveGynaikoeideisCount.ToString();
        WorkingAnthropoiValueText.text = GameState.WorkingAnthropoiCount.ToString();
        LivingAnthropoiValueText.text = GameState.LivingAnthropoiCount.ToString();
        RequiredWorkingMaterialsValueText.text = GameState.RequiredWorkingMaterials.ToString();

        // Gynaik
        GynaikoeideisUIRefresh();

        // Anthrop
        LivingAnthropoiValueText2.text = GameState.LivingAnthropoiCount.ToString();
        RequiredAnthropoiFoodValueText.text = (GameState.LivingAnthropoiCount
            * GameState.FoodConsumedPerAnthroposPerHour * GameState.ConsumeMultiplier).ToString();
        RequiredAnthropoiMaterialsValueText.text = (GameState.LivingAnthropoiCount
            * GameState.MaterialsConsumedPerAnthroposPerHour * GameState.ConsumeMultiplier).ToString();
    }

    public void GynaikoeideisUIRefresh()
    {
        ActiveGynaikoeideisValueText2.text = GameState.ActiveGynaikoeideisCount.ToString();
        AllGynaikoeideisValueText.text = (GameState.InactiveGynaikoeideisCount
            + GameState.ActiveGynaikoeideisCount).ToString();
        RequiredGynaikoeidiesMaterialsValueText.text = (GameState.ActiveGynaikoeideisCount
            * GameState.MaterialsConsumedPerGynaikoeidesPerHour * GameState.ConsumeMultiplier).ToString();
    }

    public void HazardLevelUIRefresh()
    {
        ConsumeMultiplerValueText.text = GameState.ConsumeMultiplier.ToString();
    }
}
