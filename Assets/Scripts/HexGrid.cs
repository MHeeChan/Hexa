using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class HexGrid : MonoBehaviour
{
    public static HexGrid Instance { get; private set; }
    public GameObject cellPrefab;
    public int[] colCellCounts = {3, 4, 5, 6, 5, 4, 3};
    public float xStep = 108f;
    public float yStep = 140f;
    public List<Sprite> blockSprites;

    private Dictionary<BlockType, Sprite> blockSpriteDict = new Dictionary<BlockType, Sprite>();
    
    public List<List<HexCell>> hexGrid = new List<List<HexCell>>();
    
    
    void Awake()
    {
        blockSpriteDict.Clear();
        blockSpriteDict.Add(BlockType.Blue, blockSprites[0]);
        blockSpriteDict.Add(BlockType.Yellow, blockSprites[1]);
        blockSpriteDict.Add(BlockType.Red, blockSprites[2]);
        blockSpriteDict.Add(BlockType.Green, blockSprites[3]);
        blockSpriteDict.Add(BlockType.Purple, blockSprites[4]);
        blockSpriteDict.Add(BlockType.Spinner, blockSprites[5]);
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 살아남게
        }
        else
        {
            Destroy(gameObject); // 중복 GameManager 제거
        }
    }
    void Start()
    {
        SpawnGrid();
    }
    
    public Sprite GetBlockImage(BlockType type)
    {
        return blockSpriteDict.ContainsKey(type) ? blockSpriteDict[type] : null;
    }

    void SpawnGrid()
    {
        hexGrid.Clear();
        float totalWidth = (colCellCounts.Length - 1) * xStep;
        float startX = -totalWidth / 2f;

        for (int col = 0; col < colCellCounts.Length; col++)
        {
            GameObject colParent = new GameObject($"Col_{col}");
            colParent.transform.SetParent(this.transform, false);
            List<HexCell> columnList = new List<HexCell>();

            int rowCount = colCellCounts[col];
            float totalHeight = (rowCount - 1) * yStep;
            float baseY = -totalHeight / 2f;

            // 홀수 열이면 Col_? 자체를 y+70만큼 올린다 (이동)
            float colYOffset = (col % 2 == 1) ? yStep / 2f : 0f;
            colParent.transform.localPosition = new Vector3(xStep * (col - (colCellCounts.Length - 1) / 2f), colYOffset, 0);

            for (int row = 0; row < rowCount; row++)
            {
                float y = baseY + row * yStep;
                var go = Instantiate(cellPrefab, colParent.transform);
    
                if(col%2==1)
                    go.transform.localPosition = new Vector3(0, y, 0);
                else
                {
                    go.transform.localPosition = new Vector3(0, y + 70, 0);
                }
                go.name = $"Cell_{col}_{row}";
                HexCell cell = go.GetComponent<HexCell>();
                cell.init(col, row, BlockType.None);
                columnList.Add(cell);
            }
            hexGrid.Add(columnList);
        }
    }
}