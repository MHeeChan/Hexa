using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public enum BlockType
{
    None,
    Blue,
    Yellow,
    Red,
    Green,
    Purple,
    Spinner, // 장애물
    Disable
}

public class HexCell : MonoBehaviour
{
    public int col;
    public int row;
    public BlockType blockType;
    [SerializeField] Image blockImage;
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
    
    public void OnClickCell()
    {
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
                HexGrid.Instance.plusCount();
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
    
    public bool IsAdjacent(HexCell other)
    {
        Vector2 myPos = ((RectTransform)this.transform).position;
        Vector2 otherPos = ((RectTransform)other.transform).position;
        float dist = Vector2.Distance(myPos, otherPos);
        
        return dist <= 140f; 
    }

    void Update()
    {
        if (tested) {
            setBlockType(BlockType.None);
        }
    }
    
    
    // public IEnumerator MoveBlockTo(HexCell toCell, float duration = 0.25f)
    // {
    //     var image = blockImage.transform;
    //     Vector3 startPos = image.position;
    //     Vector3 endPos = toCell.blockImage.transform.position;
    //     Debug.LogError(startPos + " ; " + endPos);
    //     Sprite tempSprite = blockImage.sprite;
    //     BlockType tempType = blockType;
    //
    //     float elapsed = 0;
    //     while (elapsed < duration)
    //     {
    //         //image.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }
    //     image.position = endPos;
    //
    //     // 도착 후에만 교체
    //     toCell.setBlockType(tempType);
    //     toCell.setImage(tempSprite);
    //     this.setBlockType(BlockType.None);
    // }
    
    public IEnumerator MoveBlockTo(HexCell toCell, float duration = 0.25f)
    {
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

      a.setBlockType(typeA);
        b.setBlockType(typeB);

        tempARect.position = bStartPos;
        tempBRect.position = aStartPos;
    
        SwapBlock(a, b);
        
		//if (!HexGrid.Instance.RemoveVerticalMatches())
        if (!HexGrid.Instance.RemoveLineMatches())
        {
            Debug.LogError("스왑실패");
			SwapBlock(a, b);
        }
    
        Destroy(tempA);
        Destroy(tempB);
    }
   
   public void SwapWithAnim(HexCell cellA, HexCell cellB)
   {
       StartCoroutine(HexCell.SwapWithAnim(cellA, cellB));
   }

}