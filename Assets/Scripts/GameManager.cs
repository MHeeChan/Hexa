using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public static int moveCount = 20;
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI scoreText;
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
    
    public void ReStart()
    {
        Debug.Log("testStart");
        UpdateCount();
        UpdateScore();
        foreach (var i in HexGrid.Instance.hexGrid )
        {
            foreach (var j in i)
            {
                BlockType randomType = (BlockType)Random.Range((int)BlockType.Blue, (int)BlockType.Purple + 1);
                j.setBlockType(randomType);
            }
        }
        // 추후 랜덤이 아니라 스테이지 초기로 수정 필요
    }

    public void UpdateScore()
    {
        scoreText.text = $"{HexGrid.totalScore}";
    }

    public void UpdateCount()
    {
        moveText.text = $"{moveCount - HexGrid.totalCount}";
    }

}