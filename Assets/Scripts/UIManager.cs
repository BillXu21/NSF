using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text resourceText; // Assign a UI Text object in Inspector
    public GameManager gameManager; // Reference the GameManager

    void Update()
    {
        // Display a placeholder for now (expand this logic later)
        resourceText.text = "Resources: \nEnergy, Water, Metals, etc.";
    }
}