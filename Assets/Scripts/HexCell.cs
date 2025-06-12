using UnityEngine;
using UnityEngine.UI;
public enum BlockType
{
    None,
    Blue,
    Yellow,
    Red,
    Green,
    Purple,
    Spinner // 장애물
}

public class HexCell : MonoBehaviour
{
    public int col;
    public int row;
    public BlockType blockType;
    [SerializeField] Image blockImage;
    [SerializeField] private GameObject selectEffect;
    
    public static HexCell selectedCell = null;
    
    public void init(int c, int r, BlockType _blockType)
    {
        col = c;
        row = r;
        blockType = _blockType;
    }

    public void setBlockType(BlockType _blockType)
    {
        blockType = _blockType;
        setImage(HexGrid.Instance.GetBlockImage(_blockType));
    }

    public void setImage(Sprite _image)
    {
        blockImage.sprite = _image;
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
                SwapBlock(selectedCell, this);
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


    
}