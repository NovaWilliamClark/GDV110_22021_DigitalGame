using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PersistentObject))]
public class GenericObject : MonoBehaviour
{
    public UnityEvent<GenericObject> ObjectStateChanged;
    
    public virtual void SetPersistentState()
    {
        gameObject.SetActive(false);
    }
}
