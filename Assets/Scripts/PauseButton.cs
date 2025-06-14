using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : ButtonManager
{
    public static bool isPaused = false;
    
    public static PauseButton Instance { get; private set; }
    [SerializeField] private GameObject PausePopup;

    void Awake()
    {
        
    }
    public override void OnClickButton()
    {
        if (!isPaused)
        {
            Debug.Log("Pause button clicked" + Time.timeScale);
            Time.timeScale = 0;
            AudioListener.pause = true;
            PausePopup.SetActive(true);
            isPaused = true;
        }
    }
}
