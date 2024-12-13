using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            HandleTileClick();
        }
    }

    void HandleTileClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HexTile clickedTile = hit.collider.GetComponent<HexTile>();
            if (clickedTile != null)
            {
                // clickedTile.Investigate();
            }
        }
    }
}