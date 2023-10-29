using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class RG_ScoreManager : NetworkBehaviour
{   
    [SerializeField] private TMP_Text _scoreText;
    private NetworkVariable<int> _playerScore = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, 
    NetworkVariableWritePermission.Server);

    void Start()
    {
        
    }
    void Update()
    {
        UpdateScoreText();
        // Debug.Log("Value" + _playerScore.Value);
    }

    private void UpdateScoreText()
    {
        _scoreText.text = $"{_playerScore.Value}";
    }

    public void IncreaseTheScore()
    {
        _playerScore.Value ++;
    }

    public int GetGameScore()
    {
        int _scoreToPass = _playerScore.Value;
        return _scoreToPass;
    }
}
