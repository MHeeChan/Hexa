using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class HexGrid : MonoBehaviour
{
    public static HexGrid Instance { get; private set; }
    public GameObject cellPrefab;
    //public int[] colCellCounts = {3, 4, 5, 6, 5, 4, 3};
    public int[] colCellCounts = {5, 6, 5, 6, 5, 6, 5};
    public float xStep = 108f;
    public float yStep = 140f;
    public List<Sprite> blockSprites;
    
    public static int movingBlockCount = 0;

    public static int totalCount = 0;
    
    public static int totalScore = 0;
    
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

    #region 블록 낙하 로직
    
    // 순차적으로. 대각선 낙하 연출할때 쓸만할지도
    // 추후 한번에 옮겨지도록 처리 필요
    private IEnumerator DropBlocksInColumn(int col)
    {
        var colList = hexGrid[col];
        float moveDuration = 0.2f;
        bool changed;
        do
        {
            changed = false;
            for (int row = 0; row < colList.Count - 1; row++)
            {
                if (colList[row].blockType == BlockType.None && colList[row + 1].blockType != BlockType.None)
                {

                    yield return StartCoroutine(colList[row + 1].MoveBlockTo(colList[row], moveDuration));
                    colList[row].setBlockType(colList[row + 1].blockType);
                    colList[row + 1].setBlockType(BlockType.None);

                    changed = true;
                }
            }
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
            StartCoroutine(DropBlocksInColumn(col));
        }
    }
    
    #endregion

    #region 블록 파괴 로직
    public bool RemoveLineMatches()
    {
        bool found = false;
        HashSet<HexCell> toRemove = new HashSet<HexCell>();
        int deleteCount = 0;
        for (int col = 0; col < hexGrid.Count; col++)
        {
            var colList = hexGrid[col];
            for (int row = 0; row < colList.Count; row++)
            {
                BlockType type = colList[row].blockType;
                if (type == BlockType.None || type == BlockType.Spinner)
                    continue;

                // ---- 1. 세로(위아래) ----
                List<HexCell> vert = new List<HexCell> { colList[row] };
                int up = row + 1;
                while (up < colList.Count && colList[up].blockType == type)
                {
                    vert.Add(colList[up]);
                    up++;
                }
                int down = row - 1;
                while (down >= 0 && colList[down].blockType == type)
                {
                    vert.Add(colList[down]);
                    down--;
                }

                if (vert.Count >= 3)
                {
                    foreach (var c in vert) toRemove.Add(c);
                    
                    deleteCount += vert.Count;
                }

                // ---- 2. ↘ 방향 (오른쪽 아래/왼쪽 위) ----
                List<HexCell> diag1 = new List<HexCell> { colList[row] };
                // 오른쪽 아래
                int dCol = col;
                int dRow = row;
                while (true)
                {
                    int nextCol = dCol + 1;
                    int nextRow = (dCol % 2 == 1) ? dRow : dRow + 1;
                    if (nextCol >= hexGrid.Count || nextRow < 0 || nextRow >= hexGrid[nextCol].Count)
                        break;
                    if (hexGrid[nextCol][nextRow].blockType == type)
                    {
                        diag1.Add(hexGrid[nextCol][nextRow]);
                        dCol = nextCol;
                        dRow = nextRow;
                    }
                    else break;
                }
                // 왼쪽 위
                dCol = col;
                dRow = row;
                while (true)
                {
                    int nextCol = dCol - 1;
                    int nextRow = (dCol % 2 == 1) ? dRow - 1 : dRow;
                    if (nextCol < 0 || nextRow < 0 || nextRow >= hexGrid[nextCol].Count)
                        break;
                    if (hexGrid[nextCol][nextRow].blockType == type)
                    {
                        diag1.Add(hexGrid[nextCol][nextRow]);
                        dCol = nextCol;
                        dRow = nextRow;
                    }
                    else break;
                }

                if (diag1.Count >= 3)
                {
                    foreach (var c in diag1) toRemove.Add(c);
                    
                    deleteCount += diag1.Count;
                }

                // ---- 3. ↙ 방향 (왼쪽 아래/오른쪽 위) ----
                List<HexCell> diag2 = new List<HexCell> { colList[row] };
                // 왼쪽 아래
                dCol = col;
                dRow = row;
                while (true)
                {
                    int nextCol = dCol - 1;
                    int nextRow = (dCol % 2 == 1) ? dRow : dRow + 1;
                    if (nextCol < 0 || nextRow < 0 || nextRow >= hexGrid[nextCol].Count)
                        break;
                    if (hexGrid[nextCol][nextRow].blockType == type)
                    {
                        diag2.Add(hexGrid[nextCol][nextRow]);
                        dCol = nextCol;
                        dRow = nextRow;
                    }
                    else break;
                }
                // 오른쪽 위
                dCol = col;
                dRow = row;
                while (true)
                {
                    int nextCol = dCol + 1;
                    int nextRow = (dCol % 2 == 1) ? dRow - 1 : dRow;
                    if (nextCol >= hexGrid.Count || nextRow < 0 || nextRow >= hexGrid[nextCol].Count)
                        break;
                    if (hexGrid[nextCol][nextRow].blockType == type)
                    {
                        diag2.Add(hexGrid[nextCol][nextRow]);
                        dCol = nextCol;
                        dRow = nextRow;
                    }
                    else break;
                }

                if (diag2.Count >= 3)
                {
                    foreach (var c in diag2) toRemove.Add(c);
                    
                    deleteCount += diag2.Count;
                }
            }
        }
        
        totalScore += deleteCount;

        foreach (var c in toRemove)
        {
            c.setBlockType(BlockType.None);
            found = true;
        }
        if (found) DropAllColumns();
        return found;
    }

    public IEnumerator RemoveLineMatchesUntilDone(System.Action<bool> onComplete = null)
    {
        yield return StartCoroutine(RemoveLineMatchesRoutine(onComplete));
    }
    public IEnumerator RemoveLineMatchesRoutine(System.Action<bool> onComplete = null)
    {
        bool anyMatched = false;
        bool matched;
        do
        {
            matched = RemoveLineMatches();
            if (matched) anyMatched = true;
            // if (matched)
            //     yield return new WaitForSeconds(2f);
            if (matched)
                yield return new WaitUntil(() => HexGrid.movingBlockCount == 0);
        } while (matched);

        onComplete?.Invoke(anyMatched);
    }



    // private IEnumerator RemoveLineMatchesRoutine()
    // {
    //     // 최초 1회
    //     yield return null;
    //     bool matched;
    //     do
    //     {
    //         // 매칭 & 파괴
    //         matched = RemoveLineMatches();
    //         // 드랍 애니메이션이 있다면 이 부분에서 "모든 이동이 끝났는지" 체크 필요
    //         // 예: yield return new WaitUntil(() => 모든 블록 이동 완료);
    //         // 또는 충분한 시간 기다리기(간단히 연출용)
    //         if (matched)
    //             yield return new WaitForSeconds(0.3f); // 드랍 연출 기다림
    //     } while (matched);
    // }


    
    #endregion
}