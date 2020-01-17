using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour 
{
    public static SelectionManager Instance;

    public Color SelectedColor; //{ get; private set; }
    public List<Tile> SelectedTiles /*{ get; private set; }*/ = new List<Tile>();
    public bool IsSelecting { get; private set; }

	private void Start() 
	{
        Instance = this;
	}
	
	private void Update() 
	{
        if (Input.GetMouseButtonUp(0))
        {
            OnRelease();
        }
	}

    public void OnFirstSelected(Tile pTile)
    {
        SelectedColor = pTile.TileData.Color;
        BoardManager.Instance.HighlightSelectedColor(SelectedColor);
        IsSelecting = true;
    }

    public bool CanBeLinked(Tile pTile)
    {
        var newTileData = pTile.TileData;

        if (newTileData.Color != SelectedColor)
        {
            return false;
        }

        var lastSelectedTileData = SelectedTiles[SelectedTiles.Count - 1].TileData;

        if (lastSelectedTileData.Xpos == newTileData.Xpos && Mathf.Abs(lastSelectedTileData.Ypos - newTileData.Ypos) == 1)
        {
            return true;
        }
        else if (lastSelectedTileData.Ypos == newTileData.Ypos && Mathf.Abs(lastSelectedTileData.Xpos - newTileData.Xpos) == 1)
        {
            return true;
        }
        else if (Mathf.Abs(lastSelectedTileData.Ypos - newTileData.Ypos) == 1 && Mathf.Abs(lastSelectedTileData.Xpos - newTileData.Xpos) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddSelectedTile(Tile pTile)
    {
        SelectedTiles.Add(pTile);
    }

    public void RemoveSelectedTile( Tile pTile)
    {
        SelectedTiles.Remove(pTile);
    }

    public void OnRelease()
    {
        IsSelecting = false;
        BoardManager.Instance.RemoveHighlight();

        if (SelectedTiles.Count >= 3)
        {
            foreach (var tile in SelectedTiles)
            {
                tile.DestroyTile();
            }

            StartCoroutine(BoardManager.Instance.ShiftBoardAndSpawnTiles(SelectedTiles));
            SelectedTiles.Clear();
        }
        else
        {
            //deselect tiles and clear list
            for (int i = 0; i < SelectedTiles.Count; i++)
            {
                SelectedTiles[i].SetSelected(false);
            }
            SelectedTiles.Clear();
        }

        SelectedColor = Color.black;
    }

    public void DeselectTile()
    {
        var tileToDeselect = SelectedTiles[SelectedTiles.Count - 1];
        tileToDeselect.SetSelected(false);
        RemoveSelectedTile(tileToDeselect);
    }
}
