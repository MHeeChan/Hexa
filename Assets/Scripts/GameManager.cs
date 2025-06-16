using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private StageData currentStage;
    public static int moveCount = 20;
    public static int missionCount = 10;
    [SerializeField] TextMeshProUGUI missionText;
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject ClearPopup;
    [SerializeField] GameObject FailPopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 스테이지 데이터 관리
    public void LoadStageFromJson(string stageFileName)
    {
        // Resources 폴더에서 JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>($"Stages/{stageFileName}");
        if (jsonFile != null)
        {
            currentStage = JsonUtility.FromJson<StageData>(jsonFile.text);
            Debug.Log("스테이지 데이터 로드 완료: " + currentStage.stageNumber);
            ApplyStageData();
        }
        else
        {
            Debug.LogError($"스테이지 파일을 찾을 수 없습니다: {stageFileName}");
        }
    }

    private void ApplyStageData()
    {
        if (currentStage == null || currentStage.blockTypes == null)
        {
            Debug.LogError(currentStage);
            Debug.LogError(currentStage.blockTypes);
            Debug.LogError("스테이지 데이터가 없습니다!");
            return;
        }

        for (int i = 0; i < HexGrid.Instance.hexGrid.Count; i++)
        {
            if (i >= currentStage.blockTypes.Length)
                continue;

            int[] rowData = currentStage.blockTypes[i].row; // 변경된 부분

            for (int j = 0; j < HexGrid.Instance.hexGrid[i].Count; j++)
            {
                if (j >= rowData.Length)
                    continue;

                // 필요하다면 Disable 처리 등 추가
                BlockType blockType = (BlockType)rowData[j];
                HexGrid.Instance.hexGrid[i][j].setBlockType(blockType);
            }
        }
    }
    #endregion

    // public void ReStart()
    // {
    //     Debug.Log("testStart");
    //     UpdateCount();
    //     UpdateScore();
    //     UpdateMission();

    //     int idxI = 0;
    //     foreach (var i in HexGrid.Instance.hexGrid )
    //     {
    //         int idxJ = 0;
    //         foreach (var j in i)
    //         {
    //             if(idxJ == 0 && (idxI == 0 || idxI == 1 || idxI == 5 || idxI == 6)){
    //                 j.setBlockType(BlockType.Disable);
    //             }
    //             else{
    //                 BlockType randomType = (BlockType)Random.Range((int)BlockType.Blue, (int)BlockType.Purple + 1);
    //                 j.setBlockType(randomType);
    //             }
    //             idxJ++;
    //         }
    //         idxI++;
    //     }
    //     // 추후 랜덤이 아니라 스테이지 초기로 수정 필요
    // }

    public void ReStart()
    {
        Debug.Log("testStart");
        UpdateCount();
        UpdateScore();
        UpdateMission();

        // 기존 랜덤 생성 대신 스테이지 데이터 적용
        if (currentStage != null)
        {
            ApplyStageData();
        }
        else
        {
            Debug.LogError("스테이지 데이터가 로드되지 않았습니다!");
        }
    }

    public void UpdateMission()
    {
        missionText.text = $"x {missionCount - HexGrid.totalMission}";
    }

    public void UpdateScore()
    {
        scoreText.text = $"{HexGrid.totalScore}";
    }

    public void UpdateCount()
    {
        moveText.text = $"{moveCount - HexGrid.totalCount}";
    }

    public void StageClear(){
        ClearPopup.SetActive(true);
    }

    public void StageFail(){
        FailPopup.SetActive(true);
    }
}