using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnScript : NetworkBehaviour
{
    // Start is called before the first frame update

    // MainPlayerMovement mainPlayer;

    public Behaviour[] scripts;
    Renderer[] renderers;
    void Start()
    {
        // mainPlayer = gameObject.GetComponent<MainPlayerMovement>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void SetPlayerState(bool state){
        foreach(var scripts in scripts){
            scripts.enabled = state;
        }
        foreach  (var render in renderers){
            render.enabled = state;
        }
    }

    // Update is called once per frame

    private Vector3 GetRandPOs(){
        Vector3 randPos = new Vector3(Random.Range(-3f,3f), 1 , Random.Range(-3f,3f));
        Debug.Log("rand pos = " + randPos.ToString());
        return randPos;
    }

    public void Respawn(){
        RespawnServerRpc();
    }

    [ServerRpc]
    private void RespawnServerRpc(){
        RespawnClientRpc(GetRandPOs());
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 spawnPos){
        StartCoroutine(RespawnCoroutine(spawnPos));
    }

    IEnumerator RespawnCoroutine(Vector3 spawnPos){
        SetPlayerState(false);
        transform.position = spawnPos;
        yield return new WaitForSeconds(3);

        SetPlayerState(true);
    }
}
