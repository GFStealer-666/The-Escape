using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class RG_LightFlickering : NetworkBehaviour
{
    [SerializeField] private Light _flickeringLight;
    [SerializeField] private AudioSource _flickeringSound;

    public NetworkVariable<float> _minLightOutTimer = new NetworkVariable<float>
    (20,NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> _maxLightOutTimer = new NetworkVariable<float>
    (80,NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> _lightOutTimer = new NetworkVariable<float>
    (80,NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> _waitTimer = new NetworkVariable<float>
    (0,NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);
    public float _waitMaxTimer = 60f;

    public float _lightRangeMax;
    public float _lightIntensityMax;

    void Start(){
        _flickeringLight = GetComponent<Light>();
        _lightOutTimer.Value = UnityEngine.Random.Range(_minLightOutTimer.Value , _minLightOutTimer.Value);
        _lightRangeMax = _flickeringLight.range;
        _lightIntensityMax = _flickeringLight.intensity;
    }

    void Update(){
        LightFlickering();
    }

    private void LightFlickering(){
        if(_lightOutTimer.Value > 0){
            _waitTimer.Value = _waitMaxTimer;
            _lightOutTimer.Value -= Time.deltaTime;
            _flickeringLight.range = 0;
            _flickeringLight.intensity = 0;
        }
        else if(_lightOutTimer.Value <= 0){
            _flickeringLight.range = _lightRangeMax;
            _flickeringLight.intensity = _lightIntensityMax;
            _waitTimer.Value -= Time.deltaTime;
            if(_waitTimer.Value <= 0 ){
                _lightOutTimer.Value = UnityEngine.Random.Range(_minLightOutTimer.Value , _minLightOutTimer.Value);
                // _flickeringSound.Play();
            }
        }

    }

}
