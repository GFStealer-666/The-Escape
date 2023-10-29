using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class RG_GameManager : NetworkBehaviour
{
    // Start is called before the first frame update
    RG_ScoreManager _scoreManagerScript;
    RG_Timer _timerScript;

    RG_StartButton _startButtonScript;

    private NetworkVariable<float> _gamePlayingTime = new NetworkVariable<float>(0f);
    [SerializeField] private float _gamePlayingTimeMax = 90f;

    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _victoryText;
    
    [SerializeField] private GameObject _leaveButton;

    [SerializeField] private GameObject _codePanel;

    [SerializeField] private int _score;

    private RG_LoginManagerScript _loginManagerScript;
    public override void OnNetworkSpawn()
    {
        _gamePlayingTime.Value = _gamePlayingTimeMax;
        _loginManagerScript = FindObjectOfType<RG_LoginManagerScript>();
    }

 
 

    private void Start()
    {
        _scoreManagerScript = FindObjectOfType<RG_ScoreManager>();
        _timerScript = FindObjectOfType<RG_Timer>();
        _startButtonScript = FindObjectOfType<RG_StartButton>();
        _gameOverText.SetActive(false);
        _victoryText.SetActive(false);

        if(!IsHost && _codePanel != null){
            _codePanel.SetActive(false);
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        _score = _scoreManagerScript.GetGameScore();
        // Debug.Log("Score : " + _score);
        if(_score >= 5)
        {
            GameVictoryClientRPc();
        }

        UpdateTimer();
        InactiveButton();
    }

    private void UpdateTimer(bool _startTimer = false)
    {
        _startTimer =  _startButtonScript._pressed.Value;
        if(_startTimer == true)
        {
            _gamePlayingTime.Value -= Time.deltaTime;
            if (_gamePlayingTime.Value < 0f) {
            GameOverClientRpc();
        }
        }
        
    }

    private void InactiveButton(bool _inactive = false)
    {
        _inactive =  _startButtonScript._pressed.Value;
        if(_inactive == true && _codePanel != null)
        {
            _codePanel.gameObject.SetActive(false);
            _leaveButton.gameObject.SetActive(false);
        }
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (_gamePlayingTime.Value / _gamePlayingTimeMax);
    }


    public void reloadTheGame(){
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.SceneManager.LoadScene("FinalGame",LoadSceneMode.Single);
    }

    [ClientRpc]
    public void GameOverClientRpc()
    {
        _gameOverText.SetActive(true);
        Invoke("reloadTheGame",3);
        _gameOverText.SetActive(false);
    }

    [ClientRpc]

    public void GameVictoryClientRPc()
    {
        _victoryText.SetActive(true);
        Invoke("reloadTheGame",3);
        _victoryText.SetActive(false);
    }

    
}
