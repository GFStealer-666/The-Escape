using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class RG_Timer : NetworkBehaviour
{
    RG_ScoreManager _scoreManagerScript;
    RG_GameManager _gameManagerScript;

    [SerializeField] private Image _timer;

    private int _checkInt;

    private void Start()
    {
        _gameManagerScript = FindObjectOfType<RG_GameManager>();
        _scoreManagerScript =FindObjectOfType<RG_ScoreManager>();
    }

    private void Update()
    {
        _checkInt = _scoreManagerScript.GetGameScore();
        if(_checkInt < 5){
            _timer.fillAmount = _gameManagerScript.GetGamePlayingTimerNormalized();
        }
        
    }
}
