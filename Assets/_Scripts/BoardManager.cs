using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour 
{
    public static BoardManager Instance;
    [SerializeField]
    private GameObject TilePrefab;
    [SerializeField]
    private List<Color> TileColors;
    public bool IsShifting = false;

    //public List<Tile> Board { get; private set; } = new List<Tile>();
    private Tile[,] Board;
    public List<Tile> AllTiles { get; private set; } = new List<Tile>();

    private void Start()
    {
        Instance = this;

        CreateBoard(7, 9);
    }

    private void CreateBoard(int pXSize, int pYSize)
    {
        Board = new Tile[pXSize, pYSize];

        Vector2 offset = TilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        float startX = -((pXSize / 2) * offset.x);
        float startY = -(pYSize / 2) * offset.y;

        for (int x = 0; x < pXSize; x++)
        {
            for (int y = 0; y < pYSize; y++)
            {
                GameObject newTileObj = Instantiate(TilePrefab, new Vector3(startX + (offset.x * x), startY + (offset.y * y)), TilePrefab.transform.rotation);
                Tile newTile = newTileObj.GetComponent<Tile>();

                Color tileColor = TileColors[Random.Range(0, TileColors.Count)];
                newTile.Initilize(x, y, tileColor);

                //Board.Add(newTile);
                Board[x, y] = newTile;
            }
        }

        AllTiles = Board.Cast<Tile>().ToList();
    }

    public void HighlightSelectedColor(Color pSelectedColor)
    {
        List<Tile> tilesToBeShaded = AllTiles.Where(a => a.TileData.Color != pSelectedColor).ToList();

        tilesToBeShaded.ForEach(a => a.SetShade(true));
    }

    public void RemoveHighlight()
    {
        AllTiles.ForEach(a => a.SetShade(false));
    }

    public IEnumerator ShiftBoardAndSpawnTiles(List<Tile> pDestroyedTiles)
    {
        IsShifting = true;
        var sorted = pDestroyedTiles.OrderByDescending(a => a.TileData.Ypos);

        foreach (var destroyed in sorted)
        {
            int x = destroyed.TileData.Xpos;
            int startY = destroyed.TileData.Ypos + 1;

            for (int i = startY; i < Board.GetLength(1); i++)
            {
                yield return new WaitForSeconds(0.1f);
                Board[x, i].DestroyTile();
                Board[x, i - 1].ReActivate(Board[x, i].TileData.Color);
            }

            //set random color of the topmost tile in the column and initialize
            Color newColor = TileColors[Random.Range(0, TileColors.Count - 1)];
            Board[x, Board.GetLength(1) - 1].ReActivate(newColor);
        }

        IsShifting = false;
    }
}
