using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase hexTilePrefab;
    public int gridWidth = 7;
    public int gridHeight = 5;

    public Dictionary<Vector3Int, HexTileData> tileDataDictionary = new Dictionary<Vector3Int, HexTileData>();

    public GameObject popupPanel;
    public TMPro.TextMeshProUGUI statsText;

    public TMPro.TextMeshProUGUI globalMoneyText;
    public TMPro.TextMeshProUGUI turnText;
    public TMPro.TextMeshProUGUI globalWaterText;

    private Vector3Int selectedTilePosition;

    // Turn and resource management
    private int totalMoney = 1000;
    private int totalWater = 0;
    private int currentTurn = 1;

    void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
        if (tilemap == null) Debug.LogError("Tilemap component is not assigned!");
        UpdateGlobalStatsUI();
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = -gridWidth; x < gridWidth; x++)
        {
            for (int y = -gridHeight; y < gridHeight; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y - (x / 2), 0);
                tilemap.SetTile(gridPosition, hexTilePrefab);

                HexTileData tileData = new HexTileData
                {
                    tileType = "Unexplored",
                    helium3Concentration = Random.Range(1, 10),
                    iceDepth = Random.Range(5, 20),
                    isInvestigated = false,
                    isDeveloped = false,
                    isMiningHelium = false,
                    isMiningIce = false
                };

                tileDataDictionary.Add(gridPosition, tileData);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int clickedPosition = tilemap.WorldToCell(mouseWorldPos);

            // Check if click is on UI, prevent closing popup
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            if (popupPanel.activeSelf)
            {
                ClosePopup();
            }
            else if (tileDataDictionary.ContainsKey(clickedPosition))
            {
                selectedTilePosition = clickedPosition;
                ShowPopup();
            }
        }
    }

    void ShowPopup()
    {
        HexTileData data = tileDataDictionary[selectedTilePosition];
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            string status = data.isDeveloped ? "Developed" : (data.isInvestigated ? "Investigated" : "Unexplored");
            string helium3Display = data.isInvestigated ? data.helium3Concentration.ToString() : "??";
            string iceDepthDisplay = data.isInvestigated ? data.iceDepth.ToString() : "??";

            statsText.text = $"Type: {status}\n" +
                             $"Helium-3: {helium3Display}\n" +
                             $"Ice Depth: {iceDepthDisplay}";
        }
    }

    void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void InvestigateTile()
    {
        if (!tileDataDictionary[selectedTilePosition].isInvestigated && totalMoney >= 10)
        {
            tileDataDictionary[selectedTilePosition].isInvestigated = true;
            totalMoney -= 10;
            UpdateGlobalStatsUI();
            ShowPopup();
        }
    }

    public void DevelopTile()
    {
        if (tileDataDictionary[selectedTilePosition].isInvestigated &&
            !tileDataDictionary[selectedTilePosition].isDeveloped && totalMoney >= 100)
        {
            tileDataDictionary[selectedTilePosition].isDeveloped = true;
            totalMoney -= 100;
            UpdateGlobalStatsUI();
            ShowPopup();
        }
    }

    public void MineHelium()
    {
        HexTileData tile = tileDataDictionary[selectedTilePosition];
        if (tile.isDeveloped && !tile.isMiningHelium)
        {
            tile.isMiningHelium = true;
            int immediateProfit = tile.helium3Concentration * 10;
            totalMoney += immediateProfit; // Immediate profit
            Debug.Log($"Helium mining started! Immediate profit: ${immediateProfit}");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
    }

    public void MineIce()
    {
        HexTileData tile = tileDataDictionary[selectedTilePosition];
        if (tile.isDeveloped && !tile.isMiningIce)
        {
            tile.isMiningIce = true;
            int immediateWater = tile.iceDepth * 2; // Immediate water gain
            totalWater += immediateWater;
            int immediateCost = 15 + tile.iceDepth; // Immediate cost
            totalMoney -= immediateCost;

            Debug.Log($"Ice mining started! Immediate water: {immediateWater}, Immediate cost: ${immediateCost}");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
    }


    public void EndTurn()
    {
        currentTurn++;
        int turnProfit = 0;
        int turnCost = 0;
        int waterGained = 0;

        foreach (var tile in tileDataDictionary.Values)
        {
            if (tile.isMiningHelium)
            {
                turnProfit += tile.helium3Concentration * 10;
                turnCost += 10;
            }

            if (tile.isMiningIce)
            {
                turnProfit += tile.iceDepth * 5;
                turnCost += 15 + tile.iceDepth;
                waterGained += tile.iceDepth; // Gain water based on depth
            }
        }

        totalMoney += turnProfit - turnCost;
        totalWater += waterGained;

        Debug.Log($"Turn {currentTurn} ended. Profit: ${turnProfit}, Costs: ${turnCost}, Net: ${turnProfit - turnCost}, Water Gained: {waterGained}");
        UpdateGlobalStatsUI();
    }

    void UpdateGlobalStatsUI()
    {
        if (globalMoneyText != null) globalMoneyText.text = $"Money: ${totalMoney}";
        if (turnText != null) turnText.text = $"Turn: {currentTurn}";
        if (globalWaterText != null) globalWaterText.text = $"Water: {totalWater}";
    }
}

public class HexTileData
{
    public string tileType;
    public int helium3Concentration;
    public int iceDepth;
    public bool isInvestigated;
    public bool isDeveloped;
    public bool isMiningHelium;
    public bool isMiningIce;
}
