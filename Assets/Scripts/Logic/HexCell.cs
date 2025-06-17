using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public enum BlockType
{
    None = 0,
    Blue = 1,
    Yellow = 2,
    Red = 3,
    Green = 4,
    Orange = 5,
    Purple = 6,
    Spinner = 7,
    SpinnerHitted = 8,
    Disable = 9
}

public class HexCell : MonoBehaviour
{
    public int col;
    public int row;
    public BlockType blockType;
    [SerializeField] Image blockImage;
    [SerializeField] ParticleSystem blockParticle;
    [SerializeField] Sprite spinnerImage;
    [SerializeField] private GameObject selectEffect;
    
    public static HexCell selectedCell = null;

    public bool tested = false;
    public void init(int c, int r, BlockType _blockType)
    {
        col = c;
        row = r;
        blockType = _blockType;
    }

    public void setBlockType(BlockType _blockType)
    {
        blockType = _blockType;
        if (blockType == BlockType.None)
        {
            blockImage.enabled = false; // 완전 숨기기
        }else if (blockType == BlockType.Disable)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            blockImage.enabled = true; // 다시 켜기
            blockImage.sprite = HexGrid.Instance.GetBlockImage(_blockType);
        }
    }
    
    public void setBlockRandomType()
    {
        BlockType randomType = (BlockType)Random.Range((int)BlockType.Blue, (int)BlockType.Purple + 1);
        setBlockType(randomType);
    }

    public void setImage(Sprite _image)
    {
        blockImage.sprite = _image;
    }
    
    public Image getImage()
    {
        return blockImage;
    }

    public void PlayParticleEffect()
    {
        blockParticle.Play();
    }
    
    public void OnClickCell()
    {
        if (blockType == BlockType.Disable)
            return; 
        
        if (selectedCell == null)
        {
            selectedCell = this;
            SetSelected(true);
        }
        else if (selectedCell != this)
        {
            // 인접한 셀만 스왑 허용!
            if (selectedCell.IsAdjacent(this))
            {
                
            	SwapWithAnim(selectedCell, this);
                //SwapBlock(selectedCell, this);
                //HexGrid.Instance.DropAllColumnsWithAnimation();
                //HexGrid.Instance.DropUntilFull();
                
                //if (!HexGrid.Instance.RemoveVerticalMatches())
                //{
                //    Debug.LogError("스왑실패");
				//	SwapBlock(this, selectedCell);
                //    //SwapWithAnim(selectedCell, this);
                //}

            //HexGrid.Instance.DropAllColumns();

            }
            selectedCell.SetSelected(false);
            SetSelected(false);
            selectedCell = null;
        }
        else
        {
            SetSelected(false);
            selectedCell = null;
        }

		//HexGrid.Instance.DropAllColumns();
    }
    
    public static void SwapBlock(HexCell a, HexCell b)
    {
        BlockType tempType = a.blockType;
        a.setBlockType(b.blockType);
        b.setBlockType(tempType);
    }
    
    public void SetSelected(bool isSelected)
    {
        if(selectEffect != null)
            selectEffect.SetActive(isSelected);
    }
    
    // public bool IsAdjacent(HexCell other)
    // {
    //     Vector2 myPos = ((RectTransform)this.transform).position;
    //     Vector2 otherPos = ((RectTransform)other.transform).position;
    //     float dist = Vector2.Distance(myPos, otherPos);
    //     Debug.LogError(dist);
    //     return dist <= 0.5f; 
    // }
    public bool IsAdjacent(HexCell cell)
    {
        int col = this.col;
        int row = this.row;
        int[][] evenOffsets = new int[][] { new int[]{-1,0}, new int[]{-1,1}, new int[]{0,1}, new int[]{0,-1}, new int[]{1,0}, new int[]{1,1} };
        int[][] oddOffsets  = new int[][] { new int[]{-1,0}, new int[]{-1,-1}, new int[]{0,1}, new int[]{0,-1}, new int[]{1,0}, new int[]{1,-1} };
        int[][] offsets = (col % 2 == 0) ? evenOffsets : oddOffsets;
        for (int i = 0; i < 6; i++)
        {
            int nCol = col + offsets[i][0];
            int nRow = row + offsets[i][1];
            if (nCol == cell.col && nRow == cell.row)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (tested) {
            setBlockType(BlockType.None);
        }
    }
    
    public IEnumerator MoveBlockTo(HexCell toCell, float duration = 0.25f)
    {
        HexGrid.movingBlockCount++;
        GameObject tempImg = Instantiate(blockImage.gameObject, blockImage.transform.parent);
        RectTransform tempRect = tempImg.GetComponent<RectTransform>();
        Vector3 startPos = blockImage.transform.position;
        Vector3 endPos = toCell.blockImage.transform.position;
        tempRect.position = startPos;

        BlockType tempBlockType = this.blockType;

        float elapsed = 0;
        while (elapsed < duration)
        {
            tempRect.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        tempRect.position = endPos;


        Destroy(tempImg);
        HexGrid.movingBlockCount--;
    }

   public static IEnumerator SwapWithAnim(HexCell a, HexCell b, float duration = 0.25f)
    {
        // 임시 이미지 생성 및 애니메이션(위 코드 참고)
        GameObject tempA = Instantiate(a.blockImage.gameObject, a.blockImage.transform.parent);
        GameObject tempB = Instantiate(b.blockImage.gameObject, b.blockImage.transform.parent);

        BlockType typeA = a.blockType;
        BlockType typeB = b.blockType;

        RectTransform tempARect = tempA.GetComponent<RectTransform>();
        RectTransform tempBRect = tempB.GetComponent<RectTransform>();
        Vector3 aStartPos = a.blockImage.transform.position;
        Vector3 bStartPos = b.blockImage.transform.position;
        tempARect.position = aStartPos;
        tempBRect.position = bStartPos;
    
        a.setBlockType(BlockType.None);
        b.setBlockType(BlockType.None);
    
        float elapsed = 0f;

        while (elapsed < duration)
        {
            tempARect.position = Vector3.Lerp(aStartPos, bStartPos, elapsed / duration);
            tempBRect.position = Vector3.Lerp(bStartPos, aStartPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(tempA);
        Destroy(tempB);

        a.setBlockType(typeA);
        b.setBlockType(typeB);

        tempARect.position = bStartPos;
        tempBRect.position = aStartPos;
    
        SwapBlock(a, b);
        bool isMatched = false;
        yield return HexGrid.Instance.StartCoroutine(
            HexGrid.Instance.RemoveLineMatchesUntilDone((matched) => isMatched = matched)
        );

        if (!isMatched)
        {
            Debug.LogError("스왑실패, 원상복구");
            SwapBlock(a, b);
        }
        else
        {
            HexGrid.Instance.plusCount();
            GameManager.Instance.UpdateCount();
            GameManager.Instance.UpdateScore();
            GameManager.Instance.UpdateMission();

            if(GameManager.missionCount <= HexGrid.totalMission){
                GameManager.Instance.StageClear();   
            }
            else if(GameManager.moveCount - HexGrid.totalCount <= 0){
                GameManager.Instance.StageFail();   
            }
        }
    
        
    }
   
    public void SwapWithAnim(HexCell cellA, HexCell cellB)
    {
        StartCoroutine(HexCell.SwapWithAnim(cellA, cellB));
    }

#region 장애물 블록 구현
    public void HitSpinner()
    {
        if (blockType != BlockType.Spinner && blockType != BlockType.SpinnerHitted) return;
        //Debug.LogError("HitSpinner");

        if (blockType == BlockType.Spinner)
        {
            // 포장 벗겨짐
            setBlockType(BlockType.SpinnerHitted);
        }
        else if (blockType == BlockType.SpinnerHitted)
        {
            // 완전 파괴
            HexGrid.totalMission++;
            PlayParticleEffect();
            setBlockType(BlockType.None);
        }
    }
#endregion
}