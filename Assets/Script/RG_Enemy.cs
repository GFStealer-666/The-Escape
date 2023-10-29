// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class RG_Enemy : NetworkBehaviour
// {
    
//     private EnemyState _state;
//     [SerializeField] public GameObject _target;
//     private CharacterController _controller;
//     private float _gravity = -9.81f;
//     private float _walkSpeed = 2f;
//     private float _chaseSpeed = 8f;
//     [SerializeField]private float _detectionRange = 30f;
//     private float _attackRange = 15f;
//     private float _patrolDuration = 3f;
//     private float _idleDuration = 1f;

//     [SerializeField] private Transform _shootOrigin;

//     private bool _iswalkRoutineRunning;
//     private float yVelocity = 0;

//     public RG_EnemySpawner _enemySpawner ;

//     private Animator _enemyAnimator;

//     private  NetworkObject _player;
//     public enum EnemyState
//     {
//         idle,
//         walk,
//         chase,
//         attack
//     }
//     void Start()
//     {
//         _state = EnemyState.idle;
//         _enemyAnimator = GetComponent<Animator>();
//         _gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
//         _walkSpeed *= Time.fixedDeltaTime;
//         _chaseSpeed *= Time.fixedDeltaTime;
//     }

//     // Update is called once per frame
//     private void FixedUpdate() 
//     {
//         if(!IsOwnedByServer) return;
//         _player = NetworkManager.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject;
//         switch(_state)
//         {
//             case EnemyState.idle:
//                 LookForPlayer();
//                 break;
//             // case EnemyState.walk:
//             //     if (!LookForPlayer())
//             //     {
//             //         Patrol();
//             //     }
//             //     break;
//             // case EnemyState.chase:
//             //     Chase();
//             //     break;
//             // case EnemyState.attack:
//             //     Attack();
//             //     break;
//             // default:
//             //     break;
//         }
//     }

//     private bool LookForPlayer()
//     {
//         if(_player != null)
//         {
//             Vector3 _disEnemyToPlayer = _player.transform.position - transform.position;
//             if(_disEnemyToPlayer.magnitude <= _detectionRange)
//             {
//                 if(Physics.Raycast(_shootOrigin.position , _disEnemyToPlayer , out RaycastHit _hit ,_detectionRange))
//                 {
//                     if(_hit.collider.CompareTag("Player"))
//                     {
//                         _target = _hit.collider.GetComponent<GameObject>();
//                         _enemyAnimator.SetBool("isChasing" , true);
//                     }
//                 }
//             }
//         }
//         return false;
//     }

   
// }
