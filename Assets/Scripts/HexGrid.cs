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

    public int totalCount = 0;
    
    public bool canSwap = false;

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
        blockSpriteDict.Add(BlockType.None, blockSprites[6]);
        
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

    public void plusCount()
    {
        totalCount++;
        Debug.LogError(totalCount);
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
    
    // public List<HexCell> FindMatch3(HexCell cell)
    // {
    //     List<HexCell> totalMatch = new List<HexCell>();
    //     BlockType type = cell.blockType;
    //     if (type == BlockType.None || type == BlockType.Spinner)
    //         return totalMatch;
    //
    //     // 3개 연속 탐색 (6방향 각각)
    //     int[][] directions = new int[][]
    //     {
    //         new int[]{0,1}, new int[]{0,-1}, new int[]{1,0}, new int[]{-1,0}
    //         //new int[]{1,1}, new int[]{-1,1},  new int[]{1,-1}, new int[]{-1,-1}
    //     };
    //     // 현재 자기가 속한 열의 원소 갯수를 기준으로 {1,1}, {1,-1}, {-1,1}, {-1,-1} 중에 뭘 넣을지 판단해야함
    //
    //     for (int d = 0; d < directions.Length; d++)
    //     {
    //         List<HexCell> line = new List<HexCell>();
    //         line.Add(cell);
    //         
    //         for (int sign = -1; sign <= 1; sign += 2) // forward/backward
    //         {
    //             int curCol = cell.col;
    //             int curGlobalY = cell.row + HexGrid.Instance.colRowStartY[cell.col];
    //
    //             while (true)
    //             {
    //                 int nCol = curCol + directions[d][0] * sign;
    //                 int nGlobalY = curGlobalY + directions[d][1] * sign;
    //
    //                 if (nCol < 0 || nCol >= HexGrid.Instance.colCellCounts.Length) break;
    //                 int nRow = nGlobalY - HexGrid.Instance.colRowStartY[nCol];
    //                 if (nRow < 0 || nRow >= HexGrid.Instance.colCellCounts[nCol]) break;
    //
    //                 HexCell next = HexGrid.Instance.hexGrid[nCol][nRow];
    //                 if (next.blockType == type)
    //                 {
    //                     line.Add(next);
    //                     curCol = nCol;
    //                     curGlobalY = nGlobalY;
    //                 }
    //                 else break;
    //             }
    //         }
    //         if (line.Count >= 3)
    //             totalMatch = totalMatch.Union(line).ToList();
    //     }
    //     return totalMatch;
    // }
    //
    // public void DestroyMatched(List<HexCell> match)
    // {
    //     Debug.LogError("DestroyMatched");
    //     // foreach (var cell in match)
    //     //     cell.SetBlockType(BlockType.None, null); // 이미지 없애기 등
    // }

    #region 블록 낙하 로직
    
    public void DropBlocksInColumn(int col)
    {
        var colList = hexGrid[col];
        bool changed;
        do
        {
            changed = false;
            for (int row = 0; row < colList.Count - 1; row++)
            {
                // 아래쪽이 None이고 위에 블록이 있으면 내려주기
                if (colList[row].blockType == BlockType.None && colList[row + 1].blockType != BlockType.None)
                {
                    Debug.LogError(row + " : " + col);
                    colList[row].setBlockType(colList[row + 1].blockType);
                    colList[row + 1].setBlockType(BlockType.None);
                    changed = true;
                }
            }
            // 맨 위칸이 None이면 랜덤 블록 생성
            if (colList[colList.Count - 1].blockType == BlockType.None)
            {
                colList[colList.Count - 1].setBlockRandomType();
                changed = true;
            }
        } while (changed);
    }

    
    public void DropAllColumns()
    {
        Debug.LogError("DropAllColumns");
        for (int col = 0; col < hexGrid.Count; col++)
        {
            DropBlocksInColumn(col);
        }
    }
    
    // public void DropUntilFull()
    // {
    //     Debug.LogError("DropUntilFull");
    //     bool changed;
    //     do
    //     {
    //         changed = false;
    //         for (int col = 0; col < hexGrid.Count; col++)
    //         {
    //             var colList = hexGrid[col];
    //             for (int row = 0; row < colList.Count - 1; row++)
    //             {
    //                 if (colList[row].blockType == BlockType.None && colList[row + 1].blockType != BlockType.None)
    //                 {
    //                     colList[row].setBlockType(colList[row + 1].blockType);
    //                     colList[row + 1].setBlockType(BlockType.None);
    //                     changed = true;
    //                 }
    //             }
    //             // 맨 위칸이 None이면 새 블록 생성
    //             if (colList[colList.Count - 1].blockType == BlockType.None)
    //             {
    //                 BlockType randomType = (BlockType)Random.Range(1, 6);
    //                 colList[colList.Count - 1].setBlockRandomType();
    //                 changed = true;
    //             }
    //         }
    //     } while (changed);
    // }
    
    #endregion

    #region 블록 파괴 로직
    
    public bool RemoveVerticalMatches()
    {
        canSwap = false;
        for (int col = 0; col < hexGrid.Count; col++)
        {
            var colList = hexGrid[col];
            int row = 0;
            while (row < colList.Count)
            {
                BlockType type = colList[row].blockType;
                if (type == BlockType.None || type == BlockType.Spinner)
                {
                    row++;
                    continue;
                }

                // 연속 구간 찾기
                int matchCount = 1;
                int next = row + 1;
                while (next < colList.Count && colList[next].blockType == type)
                {
                    matchCount++;
                    next++;
                }

                if (matchCount >= 3)
                {
                    Debug.LogError("붐");
                    canSwap = true;
                    // 3개 이상 연속 시 제거
                    for (int i = row; i < row + matchCount; i++)
                    {
                        
                        colList[i].setBlockType(BlockType.None);
                    }
                }
                row = next; // 다음 구간으로
            }
        }
        DropAllColumns();
        return canSwap;
    }
    
    #endregion
}