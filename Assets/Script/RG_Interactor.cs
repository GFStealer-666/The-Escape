using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class RG_Interactor : NetworkBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius = 0.5f;

    [SerializeField] private LayerMask _interactionMask;

    [SerializeField] private RG_InteractionPromptUI interactionPromptUIScript;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _objectNumFound;
    
    private IInteractable _interactable;
    private void Update()
    {
        if(!IsLocalPlayer) return;
       _objectNumFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position , _interactionRadius
        , _colliders , _interactionMask); 

        if(_objectNumFound > 0)
        {
            _interactable = _colliders[0].GetComponent<IInteractable>();

            if(_interactable == null ) return;
                if(!interactionPromptUIScript._isDisplayed) interactionPromptUIScript.Setup(_interactable.InteractionPrompt);
                if(Keyboard.current.eKey.wasPressedThisFrame) _interactable.Interact(this);             
            
        }
        else{
            if(_interactable != null) _interactable = null;
            if(interactionPromptUIScript._isDisplayed) interactionPromptUIScript.Close();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_interactionPoint.position , _interactionRadius);
    }

    
}
