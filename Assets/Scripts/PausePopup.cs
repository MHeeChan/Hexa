using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject BackGround;

    // Update is called once per frame
    void OnEnable()
    {
        if (!BackGround.activeSelf)
        {
            BackGround.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (BackGround.activeSelf)
        {
            BackGround.SetActive(false);
        }
    }

    public void OnClickResume()
    {
        ResumeGame();
    }

    public void OnClickRetry()
    {
        HexGrid.totalCount = 0;
        HexGrid.totalScore = 0;
        HexGrid.totalMission = 0;
        Time.timeScale = 1;
        PauseButton.Instance.bgmPlay();
        PauseButton.isPaused = false;
        this.gameObject.SetActive(false);

        GameManager.Instance.ReStart();
    }

    void ResumeGame()
    {
        Debug.Log("Resume button clicked");
        Time.timeScale = 1;
        PauseButton.Instance.bgmPlay();
        PauseButton.isPaused = false;
        this.gameObject.SetActive(false);
    }
}
