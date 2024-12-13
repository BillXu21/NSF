using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI metalsText;
    public TextMeshProUGUI waterText;

    private float energy = 100f;  // Starting resources
    private float metals = 50f;
    private float water = 30f;

    void Update()
    {
        UpdateUI();
    }

    public void AddResource(string resourceType, float amount)
    {
        switch (resourceType)
        {
            case "Energy":
                energy += amount;
                break;
            case "Metals":
                metals += amount;
                break;
            case "Water":
                water += amount;
                break;
        }
    }

    private void UpdateUI()
    {
        energyText.text = $"Energy: {energy}";
        metalsText.text = $"Metals: {metals}";
        waterText.text = $"Water: {water}";
    }
}