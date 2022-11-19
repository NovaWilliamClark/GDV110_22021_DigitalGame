using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData_SO : ScriptableObject
{
    public string sceneName;
    public LevelData persistentObjectData;
    [SerializeField] private bool initialized = false;
    public bool Initialized => initialized;
    public bool levelCutscenePlayed = false;

    private void OnEnable()
    {

    }

    public void Init()
    {
        initialized = false;
        levelCutscenePlayed = false;
        persistentObjectData = new LevelData();
    }

    public LevelData_SO CreateCopy(LevelData_SO original)
    {
        var copy = Instantiate(original);
        copy.persistentObjectData = persistentObjectData.Clone();
        return copy;
    }
    
    public void CompleteInitialization()
    {
        initialized = true;
    }

    public void MainMenu()
    {
        initialized = false;
        levelCutscenePlayed = false;
        persistentObjectData = new LevelData();
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

    public LevelData Clone()
    {
        var copy = new LevelData();
        foreach (var state in persistentObjects)
        {
            var entry = new PersistentObjectState(state.Id);
            if (state.State != null)
            {
                PersistentObjectState newState;
                switch (state.State)
                {
                    case InteractionState interactionState:
                        newState = (PersistentObjectState) interactionState.Clone();
                        break;
                    case GenericState genericState:
                        newState = (PersistentObjectState) genericState.Clone();
                        break;
                    case ItemContainerState containerState:
                        newState = (PersistentObjectState) containerState.Clone();
                        break;
                    case EnemyLevelState enemyState:
                        newState = (PersistentObjectState) enemyState.Clone();
                        break;
                    default:
                        newState = (PersistentObjectState) state.Clone();
                        break;
                }
                entry.SetState(newState);
            }
            copy.persistentObjects.Add(entry);
        }

        return copy;
    }
}

[Serializable]
public class PersistentObjectState : ICloneable
{
    protected string id;

    public PersistentObjectState(string id)
    {
        this.id = id;
    }

    public PersistentObjectState(PersistentObjectState copy)
    {
        id = copy.id;
    }

    public string Id => id;
    protected PersistentObjectState state;

    public PersistentObjectState State => state;
    
    public void SetState(PersistentObjectState newState)
    {
        state = null;
        state = newState;
    }

    public virtual object Clone()
    {
        return new PersistentObjectState(this);
    }
}

[Serializable]
public class InteractionState : PersistentObjectState
{
    public bool interacted = false;

    public InteractionState(string id) : base(id) {}

    public InteractionState(InteractionState copy) : base(copy)
    {
        interacted = copy.interacted;
        id = copy.id;
    }
    public override object Clone()
    {
        var itstate = new InteractionState(this);
        return itstate;
    }
}

[Serializable]
public class GenericState : PersistentObjectState
{
    public bool active = true;

    public GenericState(string id) : base(id) {}
    public override object Clone()
    {
        return new GenericState(id) {active = active};
    }
}

[Serializable]
public class ItemContainerState : PersistentObjectState
{
    public List<ItemData> items = new List<ItemData>();

    public ItemContainerState(string id) : base(id) {}

    public override object Clone()
    {
        return new ItemContainerState(id) {items = items.ToList()};
    }
}

[Serializable]
public class EnemyLevelState : PersistentObjectState
{
    //public 
    public bool active = true;

    public EnemyLevelState(string id) : base(id) { }
    public override object Clone()
    {
        return new EnemyLevelState(id) {active = active};
    }
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

    public override object Clone()
    {
        return new BreakerState(id)
        {
            opened = opened, 
            usedFuseCircle = usedFuseCircle, 
            usedFuseSquare = usedFuseSquare,
            usedFuseTriangle = usedFuseTriangle
        };
    }
}