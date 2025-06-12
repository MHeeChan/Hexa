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
            // 선택 효과(테두리 등) 주기
        }
        else if (selectedCell != this)
        {
            SwapBlock(selectedCell, this);
            selectedCell = null;
            // 선택 효과 제거
        }
        else
        {
            // 같은 셀 두 번 클릭 시 선택 취소
            selectedCell = null;
            // 선택 효과 제거
        }
    }
    
    public static void SwapBlock(HexCell a, HexCell b)
    {
        BlockType tempType = a.blockType;
        a.setBlockType(b.blockType);
        b.setBlockType(tempType);
    }
    
}