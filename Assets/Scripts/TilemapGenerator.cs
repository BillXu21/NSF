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
    public TMPro.TextMeshProUGUI globalEnergyText;

    private Vector3Int selectedTilePosition;

    // Turn and resource management
    private int totalMoney = 500; // Lower starting money
    private int totalWater = 0;
    private int totalEnergy = 0; // Colony starts with no energy
    private int currentTurn = 1;

    private int lastNetResult = 0;
    private int lastWaterChange = 0;
    private int lastEnergyChange = 0;

    // Constant drain values per turn
    private const int baseEnergyDrain = 20;
    private const int baseWaterDrain = 10;

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
                    isMiningHelium = false,
                    isMiningIce = false,
                    solarPanelLevel = 0 // Start with no solar panels
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
            string status = "Unexplored";
            if (data.solarPanelLevel > 0)
            {
                status = $"Solar Panel Level {data.solarPanelLevel}";
            }
            else if (data.isMiningHelium && data.isMiningIce)
            {
                status = "Mining Helium & Ice";
            }
            else if (data.isMiningHelium)
            {
                status = "Mining Helium";
            }
            else if (data.isMiningIce)
            {
                status = "Mining Ice";
            }
            else if (data.isInvestigated)
            {
                status = "Investigated";
            }

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
            lastNetResult -= 10; // Update net money immediately
            Debug.Log("Tile investigated. Cost: 10 money.");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
    }

    public void MineHelium()
    {
        if (!tileDataDictionary.ContainsKey(selectedTilePosition))
        {
            Debug.Log("No tile selected for mining helium.");
            return;
        }

        HexTileData tile = tileDataDictionary[selectedTilePosition];
        if (!tile.isMiningHelium && totalWater >= tile.helium3Concentration)
        {
            tile.isMiningHelium = true;
            totalWater -= tile.helium3Concentration; // Water cost
            lastWaterChange -= tile.helium3Concentration; // Update immediate water change
            lastEnergyChange -= (5 + Mathf.CeilToInt(tile.helium3Concentration * 0.5f)); // Update immediate energy drain
            Debug.Log($"Helium mining started on tile {selectedTilePosition}. Helium Concentration: {tile.helium3Concentration}");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
        else
        {
            Debug.Log("Not enough water or already mining helium on this tile.");
        }
    }

    public void MineIce()
    {
        if (!tileDataDictionary.ContainsKey(selectedTilePosition))
        {
            Debug.Log("No tile selected for mining ice.");
            return;
        }

        HexTileData tile = tileDataDictionary[selectedTilePosition];
        if (!tile.isMiningIce)
        {
            tile.isMiningIce = true;
            lastEnergyChange -= tile.iceDepth; // Energy drain
            lastWaterChange += 10; // Water gain
            Debug.Log($"Ice mining started on tile {selectedTilePosition}. Ice Depth: {tile.iceDepth}");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
        else
        {
            Debug.Log("Already mining ice on this tile.");
        }
    }

    public void BuildSolarPanel()
    {
        HexTileData tile = tileDataDictionary[selectedTilePosition];
        if (tile.solarPanelLevel == 0 && totalMoney >= 50)
        {
            totalMoney -= 50;
            tile.solarPanelLevel = 1;
            lastEnergyChange += 15; // Energy gain
            Debug.Log("Solar Panel built. Energy increased by 15 per turn.");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
        else if (tile.solarPanelLevel > 0 && totalMoney >= 100)
        {
            totalMoney -= 100;
            tile.solarPanelLevel++;
            lastEnergyChange += 10 * tile.solarPanelLevel; // Incremental gain
            Debug.Log($"Solar Panel upgraded to Level {tile.solarPanelLevel}. Energy increased by {10 * tile.solarPanelLevel} per turn.");
            UpdateGlobalStatsUI();
            ShowPopup();
        }
        else
        {
            Debug.Log("Not enough money to build or upgrade the solar panel.");
        }
    }

    public void EndTurn()
    {
        currentTurn++;
        int turnProfit = 0;
        int turnCost = 0;
        int waterGained = 0;
        int energyChange = -baseEnergyDrain; // Base energy drain

        foreach (var tile in tileDataDictionary.Values)
        {
            if (tile.isMiningHelium)
            {
                turnProfit += tile.helium3Concentration * 10;
                turnCost += 10;
                energyChange -= (5 + Mathf.CeilToInt(tile.helium3Concentration * 0.5f));
            }

            if (tile.isMiningIce)
            {
                turnCost += 10; // Ice mining cost
                waterGained += 10; // Water gain
                energyChange -= tile.iceDepth;
            }

            if (tile.solarPanelLevel > 0)
            {
                energyChange += 15 + (10 * (tile.solarPanelLevel - 1));
            }
        }

        turnCost += baseWaterDrain;
        totalWater -= baseWaterDrain;

        lastNetResult = turnProfit - turnCost;
        totalMoney += lastNetResult;
        totalWater += waterGained;
        totalEnergy += energyChange;

        lastWaterChange = waterGained - baseWaterDrain;
        lastEnergyChange = energyChange;

        Debug.Log($"Turn {currentTurn} ended. Profit: ${turnProfit}, Costs: ${turnCost}, Net: ${lastNetResult}, Water Gained: {waterGained}, Energy Change: {energyChange}");
        UpdateGlobalStatsUI();
    }

    void UpdateGlobalStatsUI()
    {
        if (globalMoneyText != null) 
            globalMoneyText.text = $"Money: ${totalMoney}";
    
        if (globalWaterText != null) 
            globalWaterText.text = $"Water: {totalWater} ({lastWaterChange:+#;-#;0})";
    
        if (globalEnergyText != null) 
            globalEnergyText.text = $"Energy: {totalEnergy} ({lastEnergyChange:+#;-#;0})";
    
        if (turnText != null) 
            turnText.text = $"Turn: {currentTurn}";
    }
}

public class HexTileData
{
    public string tileType;
    public int helium3Concentration;
    public int iceDepth;
    public bool isInvestigated;
    public bool isMiningHelium;
    public bool isMiningIce;
    public int solarPanelLevel;
}
