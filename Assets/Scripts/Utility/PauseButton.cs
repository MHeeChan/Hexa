using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : ButtonManager
{
    public static bool isPaused = false;
    
    public static PauseButton Instance { get; private set; }
    [SerializeField] public AudioSource bgm;
    [SerializeField] private GameObject PausePopup;

    void Awake()
    {
        Instance = this;
    }


    public override void OnClickButton()
    {
        if (!isPaused)
        {
            Debug.Log("Pause button clicked" + Time.timeScale);
            Time.timeScale = 0;
            bgmPause();
            PausePopup.SetActive(true);
            isPaused = true;
        }
    }

    public void bgmPause(){
        bgm.Pause();
    }

    public void bgmPlay(){
        bgm.Play();
    }

    public void bgmRestart()
    {
        bgm.Stop();           // 현재 재생 중인 BGM 정지
        bgm.time = 0f;        // 재생 위치를 처음으로 되돌림
        bgm.Play();           // 처음부터 다시 재생
    }
}
