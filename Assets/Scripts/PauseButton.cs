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
}
