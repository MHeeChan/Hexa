using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPopup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject BackGround;
    [SerializeField] private AudioSource bgm;

    // Update is called once per frame
    void OnEnable()
    {
        if (!BackGround.activeSelf)
        {
            BackGround.SetActive(true);   
        }
        //Time.timeScale = 0;
        bgm.ignoreListenerPause = true;
        PauseButton.Instance.bgmPause();
        bgm.Play();
        PauseButton.isPaused = true;
    }

    void OnDisable()
    {
        if (BackGround.activeSelf)
        {
            BackGround.SetActive(false);
        }
    }

    public void OnClickRetry()
    {
        HexGrid.totalCount = 0;
        HexGrid.totalScore = 0;
        HexGrid.totalMission = 0;
        Time.timeScale = 1;
        PauseButton.Instance.bgmRestart();
        PauseButton.isPaused = false;
        this.gameObject.SetActive(false);

        GameManager.Instance.ReStart();
    }

}
