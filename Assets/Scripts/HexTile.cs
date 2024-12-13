using UnityEngine;
using TMPro;  // Add for text handling

public class HexTile : MonoBehaviour
{
    public string tileType = "Unexplored";  // Current state of the tile

    public TextMeshProUGUI statsPopup;  // Assign in the Inspector

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set default color based on state
        UpdateTileColor();

        // Hide stats popup initially
        if (statsPopup != null)
        {
            statsPopup.gameObject.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        // Highlight tile when hovered
        spriteRenderer.color = Color.yellow;
    }

    void OnMouseExit()
    {
        // Reset tile color
        UpdateTileColor();
        HideStatsPopup();
    }

    void OnMouseDown()
    {
        Debug.Log($"Tile clicked! Current state: {tileType}");
        ShowStatsPopup();
    }

    private void UpdateTileColor()
    {
        // Change color based on tile state
        switch (tileType)
        {
            case "Unexplored":
                spriteRenderer.color = Color.gray; // Default color
                break;
            case "Investigated":
                spriteRenderer.color = Color.blue; // Investigated color
                break;
            case "Developed":
                spriteRenderer.color = Color.green; // Developed color
                break;
        }
    }

    private void ShowStatsPopup()
    {
        if (statsPopup != null)
        {
            statsPopup.gameObject.SetActive(true);
            statsPopup.text = $"Tile State: {tileType}\nEnergy: 0\nMetals: 0\nWater: 0";
        }
    }

    private void HideStatsPopup()
    {
        if (statsPopup != null)
        {
            statsPopup.gameObject.SetActive(false);
        }
    }
}