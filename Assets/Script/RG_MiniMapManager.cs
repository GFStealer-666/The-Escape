using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RG_MiniMapManager : MonoBehaviour
{
    [SerializeField] private GameObject minimapCamera;
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
        minimapCamera.SetActive(true);
    }


}
