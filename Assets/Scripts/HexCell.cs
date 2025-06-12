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

    public void init(int c, int r, BlockType _blockType)
    {
        col = c;
        row = r;
        blockType = _blockType;
    }

    public void setImage(Image _image)
    {
        blockImage = _image;
    }
    // 나중에 블록 오브젝트(이미지, 이펙트 등) 참조도 추가 가능
}