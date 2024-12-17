using UnityEngine;
using TMPro;

public class GlobalStats : MonoBehaviour
{
    public static GlobalStats Instance;

    public int totalEnergy = 0;
    public int totalMetals = 0;
    public int totalWater = 0;

    // References to the global stats bar UI text elements
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI metalsText;
    public TextMeshProUGUI waterText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures GlobalStats persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResources(int energy, int metals, int water)
    {
        totalEnergy += energy;
        totalMetals += metals;
        totalWater += water;

        Debug.Log($"Global Stats Updated: Energy = {totalEnergy}, Metals = {totalMetals}, Water = {totalWater}");

        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        // Update the text elements if they are assigned
        if (energyText != null) energyText.text = $"Energy: {totalEnergy}";
        if (metalsText != null) metalsText.text = $"Metals: {totalMetals}";
        if (waterText != null) waterText.text = $"Water: {totalWater}";
    }
}