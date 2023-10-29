using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RG_InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _prompText;
    [SerializeField] private GameObject _uiPanel;
    public bool _isDisplayed = false;

    public void Start(){
        _uiPanel.SetActive(false);
    }
    public void Setup(string prompText){
        _prompText.text = prompText;
        _uiPanel.SetActive(true);
        _isDisplayed = true;
    }

    public void Close(){
        _isDisplayed = false;
        _uiPanel.SetActive(false);
    }

}
