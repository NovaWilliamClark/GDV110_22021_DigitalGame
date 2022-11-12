using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PersistentObject))]
public class GenericObject : MonoBehaviour
{
    public UnityEvent<GenericObject, GenericState> ObjectStateChanged;
    
    public virtual void SetPersistentState(GenericState state)
    {
        gameObject.SetActive(state.active);
    }
}
