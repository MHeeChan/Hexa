[System.Serializable]
public class BlockTypeRow
{
    public int[] row; // BlockType 대신 int로 선언 (enum은 int로 변환 가능)
}

[System.Serializable]
public class StageData
{
    public int stageNumber;
    public BlockTypeRow[] blockTypes; // BlockTypeRow 배열로 변경
}