using System;
using Unity.Collections;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    [SerializeField] private string uniqueId;
    public string Id => uniqueId;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID () => uniqueId = Guid.NewGuid().ToString();
}