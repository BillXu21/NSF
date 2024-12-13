using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hexPrefab; // Assign HexTile prefab in Inspector
    public int gridWidth = 5;
    public int gridHeight = 5;

    private HexTile[,] hexGrid;

    void Start()
    {
        CreateHexGrid();
    }

    void CreateHexGrid()
    {
        hexGrid = new HexTile[gridWidth, gridHeight];
        float xOffset = 1.0f;
        float yOffset = 0.866f; // Hex height multiplier

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Staggered grid layout
                float xPos = x * xOffset;
                float yPos = y * yOffset;
                if (y % 2 == 1) xPos += xOffset / 2;

                // Create hex and assign properties
                GameObject newHex = Instantiate(hexPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
                HexTile hex = newHex.GetComponent<HexTile>();
                // hex.hexName = $"Hex ({x},{y})";
                // hex.energyOutput = Random.Range(0f, 50f);
                // hex.waterAvailability = Random.Range(0f, 50f);
                // hex.metalRichness = Random.Range(0f, 50f);

                hexGrid[x, y] = hex;
            }
        }
    }
}