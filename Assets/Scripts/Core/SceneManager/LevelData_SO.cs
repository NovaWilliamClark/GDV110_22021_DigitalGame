using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData_SO : ScriptableObject
{
    public string sceneName;
    public LevelData levelInteractions = new();
    public LevelData levelGenericObjects = new();
    public LevelData levelContainers = new();
    public LevelData levelEnemies = new();
    [SerializeField] private bool initialized = false;
    public bool Initialized => initialized;
    public bool levelCutscenePlayed = false;

    private void OnEnable()
    {
        initialized = false;
        levelCutscenePlayed = false;
        levelInteractions = new LevelData();
        levelGenericObjects = new LevelData();
        levelContainers = new LevelData();
        levelEnemies = new LevelData();
    }

    public void Setup()
    {
        initialized = true;
    }
}

[Serializable]
public class LevelData
{
    public List<PersistentObjectState> persistentObjects = new();

    public virtual void AddObjectState(PersistentObject po)
    {
        if (!IsIdValid(po)) return;
        persistentObjects.Add(new PersistentObjectState(po.Id));
    }

    public virtual void SetObjectState(PersistentObject po, PersistentObjectState state)
    {
        if (!IsIdValid(po)) return;
        var entry = persistentObjects.FirstOrDefault(e => e.Id == po.Id);
        if (entry != null)
        {
            entry.SetState(state);
        }
    }

    public virtual PersistentObjectState GetObjectState(PersistentObject po)
    {
        var poState = persistentObjects.FirstOrDefault(e => e.Id == po.Id);
        if (poState == null)
        {
            Debug.LogErrorFormat("Object {0} not found in persistent object list", po.Id);
        }
        return poState;
    }

    public virtual bool IsIdValid(PersistentObject po)
    {
        if (String.IsNullOrEmpty(po.Id) || po.Id == "" || String.IsNullOrWhiteSpace(po.Id))
        {
            Debug.LogWarningFormat("{0} PersistantObject ID is invalid", po.gameObject.name);
            return false;
        }

        return true;
    }
}

[Serializable]
public class PersistentObjectState
{
    protected string id;

    public PersistentObjectState(string id)
    {
        this.id = id;
    }

    public string Id => id;
    private PersistentObjectState state;

    public PersistentObjectState State => state;
    
    public void SetState(PersistentObjectState newState)
    {
        state = null;
        state = newState;
    }
}

[Serializable]
public class InteractionState : PersistentObjectState
{
    public bool interacted = false;

    public InteractionState(string id) : base(id) {}
}

[Serializable]
public class GenericState : PersistentObjectState
{
    public bool active = true;

    public GenericState(string id) : base(id) {}
}

[Serializable]
public class ItemContainerState : PersistentObjectState
{
    public List<ItemData> items = new List<ItemData>();

    public ItemContainerState(string id) : base(id) {}
}

[Serializable]
public class EnemyLevelState : PersistentObjectState
{
    //public 
    public bool active = true;

    public EnemyLevelState(string id) : base(id) { }
}

[Serializable]
public class BreakerState : InteractionState
{
    public bool opened = false;
    public bool usedFuseSquare = false;
    public bool usedFuseCircle = false;
    public bool usedFuseTriangle = false;

    public BreakerState(string id) : base(id)
    {
    }
}