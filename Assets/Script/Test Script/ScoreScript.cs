using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreScript : NetworkBehaviour
{

    TMP_Text p1Text;
    TMP_Text p2Text;
    MainPlayerMovement mainPlayerMovement;
    // Start is called before the first frame update

    public NetworkVariable<int> scoreP1 = new NetworkVariable<int>(5,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> scoreP2 = new NetworkVariable<int>(5,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    void Start()
    {
        p1Text = GameObject.Find("ScorePlayer1").GetComponent<TMP_Text>();
        p2Text = GameObject.Find("ScorePlayer2").GetComponent<TMP_Text>();
        mainPlayerMovement = GetComponent<MainPlayerMovement>();
    }

    private void updateScore()
    {
        if (IsOwnedByServer)
        {
            { p1Text.text = $"{mainPlayerMovement.playerNameA.Value} : {scoreP1.Value}"; }
        }
        else
        {
            { p2Text.text = $"{mainPlayerMovement.playerNameB.Value} : {scoreP2.Value}"; }
        }
          
    }

    // Update is called once per frame
    void Update()
    {
        updateScore();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) return;
        if (collision.gameObject.tag == "DeathZone")
        {
            if (IsOwnedByServer) { scoreP1.Value--; }
            else { scoreP2.Value--; }
            gameObject.GetComponent<PlayerSpawnScript>().Respawn();
        }
        
    }
}
