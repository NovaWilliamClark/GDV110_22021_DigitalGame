using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelData", fileName = "LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public List<GameObject> levelEnemies;
    public List<LevelInteractionData> interactions;
    public List<GenericPersistentObjectData> persistentObjects;
    [SerializeField] private bool initialized = false;
    public bool Initialized => initialized;
    public bool levelCutscenePlayed = false;

    private void OnEnable()
    {
        initialized = false;
        levelCutscenePlayed = false;
        interactions.Clear();
        persistentObjects.Clear();
    }

    public void Setup()
    {
        initialized = true;
    }

    public void AddInteraction(PersistentObject po)
    {
        PersistentIdValid(po);
        interactions.Add(new LevelInteractionData {id = po.Id, interacted = false});
    }

    public bool HasInteracted(string id)
    {
        return interactions.Any(interaction => interaction.id == id && interaction.interacted);
    }

    public void SetInteraction(string id, bool value = true)
    {
        var interact = interactions.First(s => s.id == id);
        interact.interacted = value;
    }

    public void AddPersistentObject(PersistentObject po)
    {
        PersistentIdValid(po);
        persistentObjects.Add(new GenericPersistentObjectData {id = po.Id, satisfied = false});
    }

    public bool PersistentObjectActive(string id)
    {
        return persistentObjects.Any(po => po.id == id && po.satisfied);
    }

    public void SetPersistentObject(string id, bool value = true)
    {
        var po = persistentObjects.First(s => s.id == id);
        po.satisfied = value;
    }

    public bool PersistentIdValid(PersistentObject po)
    {
        if (String.IsNullOrEmpty(po.Id) || po.Id == "" || String.IsNullOrWhiteSpace(po.Id))
        {
            Debug.LogWarningFormat("{0} PersistantObject ID is invalid", po.gameObject.name);
            return false;
        }

        return true;
    }

    public bool InteractionIdExists(string id)
    {
        foreach (var interactionData in interactions)
        {
            if (interactionData.id == id)
            {
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class GenericPersistentObjectData
{
    // generic object persistence
    public string id;
    public bool satisfied = true;
    //public
}

[Serializable]
public class LevelItemData
{
    public string itemId;
    public bool pickedUp = false;
}

[Serializable]
public class LevelInteractionData
{
    public string id;
    public bool interacted = false;
}