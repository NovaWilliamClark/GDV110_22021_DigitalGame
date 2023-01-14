using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData_SO : ScriptableObject
{
    public string sceneName;
    public LevelData persistentObjectData;
    [SerializeField] private bool initialized = false;
    public bool Initialized => initialized;
    public bool levelCutscenePlayed = false;

    [FormerlySerializedAs("CreatedAt")] public float createdAt;
    private void OnEnable()
    {

    }

    public void Init()
    {
        initialized = false;
        levelCutscenePlayed = false;
        persistentObjectData = new LevelData();
        createdAt = Time.time;
    }

    public LevelData_SO CreateCopy()
    {
        var copy = Instantiate(this);
        copy.persistentObjectData = persistentObjectData.Clone();
        copy.createdAt = Time.time;
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
                switch (state.State)
                {
                    case InteractionState interactionState:
                        entry.SetState(interactionState.Clone() as InteractionState);
                        break;
                    case GenericState genericState:
                        entry.SetState(genericState.Clone() as GenericState);
                        break;
                    case ItemContainerState containerState:
                        entry.SetState(containerState.Clone() as ItemContainerState);
                        break;
                    case EnemyLevelState enemyState:
                        entry.SetState(enemyState.Clone() as EnemyLevelState);
                        break;
                    default:
                        entry.SetState(state.Clone() as PersistentObjectState);
                        break;
                }
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
        return new InteractionState(this);
    }
}

[Serializable]
public class GenericState : PersistentObjectState
{
    public bool active = true;

    public GenericState(string id) : base(id) {}

    public GenericState(GenericState original) : base(original)
    {
        active = original.active;
        id = original.id;
    }
    public override object Clone()
    {
        return new GenericState(this);
    }
}

[Serializable]
public class ItemContainerState : PersistentObjectState
{
    public List<ItemData> items = new();

    public ItemContainerState(string id) : base(id) {}

    public ItemContainerState(ItemContainerState original) : base(original)
    {
        foreach (var item in original.items)
        {
            items.Add(item);
            id = original.id;
        }
    }

    public override object Clone()
    {
        return new ItemContainerState(this);
    }
}

[Serializable]
public class EnemyLevelState : PersistentObjectState
{
    //public 
    public bool active = true;
    public bool behaviourEnabled = true;

    public EnemyLevelState(string id) : base(id) { }

    public EnemyLevelState(EnemyLevelState original) : base(original)
    {
        active = original.active;
        behaviourEnabled = original.behaviourEnabled;
        id = original.id;
    }
    public override object Clone()
    {
        return new EnemyLevelState(this);
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

    public BreakerState(BreakerState original) : base(original)
    {
        id = original.id;
        opened = original.opened;
        usedFuseCircle = original.usedFuseCircle;
        usedFuseSquare = original.usedFuseSquare;
        usedFuseTriangle = original.usedFuseTriangle;
    }

    public override object Clone()
    {
        return new BreakerState(this);
    }
}

[Serializable]
public class MoveableState : InteractionState
{
    public Vector3 position;

    public MoveableState(string id) : base(id) {}

    public MoveableState(MoveableState original) : base(original)
    {
        id = original.id;
        position.x = original.position.x;
        position.y = original.position.y;
        position.z = original.position.z;
    }

    public override object Clone()
    {
        return new MoveableState(this);
    }
}