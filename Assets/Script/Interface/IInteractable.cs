using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable{

    public string InteractionPrompt {get ;}
    public bool Interact(RG_Interactor interactor);
    
}
