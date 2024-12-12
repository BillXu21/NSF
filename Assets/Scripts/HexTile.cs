using UnityEngine;

public class HexTile : MonoBehaviour
{
    public enum HexType { Empty, Resource, Building, Infrastructure, Special }
    
    // Tile properties
    public HexType type;
    public string hexName;
    public bool investigated = false;

    // Resource stats (placeholders)
    public float energyOutput;
    public float waterAvailability;
    public float metalRichness;

    // Investigate the tile
    public void Investigate()
    {
        if (!investigated)
        {
            investigated = true;
            Debug.Log($"Investigated {hexName}: Energy={energyOutput}, Water={waterAvailability}, Metal={metalRichness}");
        }
        else
        {
            Debug.Log($"{hexName} already investigated.");
        }
    }
}