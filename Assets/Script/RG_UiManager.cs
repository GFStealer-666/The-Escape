using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RG_UiManager : MonoBehaviour
{
   [SerializeField] private GameObject scoreUI;
   [SerializeField] private GameObject timeUI;
    private void OnEnable() 
    {
        RG_StartButton.OnStartButtonPress += OnGameStart;
    }
    private void OnDisable() 
    {
        RG_StartButton.OnStartButtonPress -= OnGameStart;
    }

    private void OnGameStart(bool _bool)
    {
        scoreUI.SetActive(true);
        timeUI.SetActive(true);
    }
}
