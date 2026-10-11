using UnityEngine;
using TMPro;

public class TopUIBarScript : MonoBehaviour
{
    [Header("FoodTap")]
    public TextMeshProUGUI StoredFoodValueText;
    public TextMeshProUGUI FoodStorageValueText;

    [Header("GynaikoeideisTap")]
    public TextMeshProUGUI InactiveGynaikoeideisValue;
    public TextMeshProUGUI InactiveGynaikoeideisStorageValue;

    [Header("MaterialsTap")]
    public TextMeshProUGUI StoredMaterialsValueText;
    public TextMeshProUGUI MaterialsStorageValueText;

    public GameState GameState { get; set; }

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        HourUIRefresh();
        GynaikoeideisUIRefresh();
    }

    public void HourUIRefresh()
    {
        // 식량
        StoredFoodValueText.text = GameState.StoredFood.ToString();
        FoodStorageValueText.text = GameState.FoodStorageCapacity.ToString();

        // 인형
        InactiveGynaikoeideisValue.text = GameState.InactiveGynaikoeideisCount.ToString();
        InactiveGynaikoeideisStorageValue.text = GameState.InactiveGynaikoeideisStorageCapacity.ToString();

        // 자재
        StoredMaterialsValueText.text = GameState.StoredMaterials.ToString();
        MaterialsStorageValueText.text = GameState.MaterialsStorageCapacity.ToString();

    }

    public void GynaikoeideisUIRefresh()
    {
        InactiveGynaikoeideisValue.text = GameState.InactiveGynaikoeideisCount.ToString();
        InactiveGynaikoeideisStorageValue.text = GameState.InactiveGynaikoeideisStorageCapacity.ToString();
    }
}
