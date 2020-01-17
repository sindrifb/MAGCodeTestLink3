using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour 
{
    [SerializeField]
    private GameObject ShadeObj;
    public TileData TileData; //{ get; private set; }
    public bool IsSelected = false;
    private Animator Animator;
    private SpriteRenderer Renderer;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        Renderer = GetComponent<SpriteRenderer>();
    }


    public void Initilize(int pX, int pY, Color pColor)
    {
        TileData = new TileData(pColor, pX, pY);

        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }
        
        if (Renderer == null)
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        Renderer.color = pColor;
        SetShade(false);
        Renderer.enabled = true;
    }

    public void ReActivate(Color pColor)
    {
        TileData.SetNewColor(pColor);
        Renderer.color = pColor;
        Renderer.enabled = true;
    }

    public void SetShade(bool pValue)
    {
        if (ShadeObj.activeInHierarchy == pValue)
        {
            return;
        }

        ShadeObj.SetActive(pValue);
    }

    private void OnMouseDown()
    {
        if (BoardManager.Instance.IsShifting)
        {
            return;
        }

        SelectionManager.Instance.OnFirstSelected(this);
        SetSelected(true);
        SelectionManager.Instance.AddSelectedTile(this);
    }


    private void OnMouseEnter()
    {
        if (!SelectionManager.Instance.IsSelecting || TileData.Color != SelectionManager.Instance.SelectedColor || this == SelectionManager.Instance.SelectedTiles[0] && SelectionManager.Instance.SelectedTiles.Count < 2)
        {
            return;
        }

        var selectedTiles = SelectionManager.Instance.SelectedTiles;

        if (IsSelected && selectedTiles[selectedTiles.Count - 2] == this)
        {
            //SetSelected(false);
            SelectionManager.Instance.DeselectTile();
        }
        else if (!IsSelected && SelectionManager.Instance.CanBeLinked(this))
        {
            SetSelected(true);
            SelectionManager.Instance.AddSelectedTile(this);
        }
    }

    public void SetSelected(bool pValue)
    {
        IsSelected = pValue;

        //if (pValue)
        //{
        //    SelectionManager.Instance.AddSelectedTile(this);
        //}
        //else
        //{
        //    SelectionManager.Instance.RemoveSelectedTile(this);
        //}

        Animator.SetBool("Selected", pValue);
    }

    public void DestroyTile()
    {
        Renderer.enabled = false;
        IsSelected = false;
        Animator.SetBool("Selected", false);
        //destroy effect and sound?
    }
}

public struct TileData
{
    public Color Color;
    public int Xpos;
    public int Ypos;

    public TileData(Color pColor, int pXpos, int pYpos)
    {
        Color = pColor;
        Xpos = pXpos;
        Ypos = pYpos;
    }

    public void SetNewColor(Color pNewcolor)
    {
        Color = pNewcolor;
    }
}
