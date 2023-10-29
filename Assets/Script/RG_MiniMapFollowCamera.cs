using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RG_MiniMapFollowCamera : NetworkBehaviour
{
    [SerializeField] private Transform _targetToFollow;
    [SerializeField] private float _cameraHeight;
    private bool _isGameStart = false;
    public bool _rotateWithTheTarget;

    private void OnEnable() 
    {
        RG_StartButton.OnStartButtonPress += OnGameStart;
    }
    private void OnDisable() 
    {
        RG_StartButton.OnStartButtonPress -= OnGameStart;
    }
    private void Awake(){
        _cameraHeight = transform.position.y;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsOwner)
        {
            this.gameObject.SetActive(false);
            // Debug.Log(_button[0].ToString());
        }
    }
    private void OnGameStart(bool _bool)
    {
        _isGameStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(!IsOwner && _isGameStart ==false) return;
        Vector3 _targetPosition = _targetToFollow.transform.position;
        transform.position = new Vector3(_targetPosition.x , _targetPosition.y + _cameraHeight , _targetPosition.z);
        if(_rotateWithTheTarget){
            Quaternion _targerRotaion = _targetToFollow.transform.rotation;
            transform.rotation = Quaternion.Euler(90,_targerRotaion.eulerAngles.y,0);
        }
    }
}
