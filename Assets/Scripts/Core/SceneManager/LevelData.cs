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
    [SerializeField] private bool initialized = false;
    public bool Initialized => initialized;
    public bool levelCutscenePlayed = false;

    private void OnEnable()
    {
        initialized = false;
        interactions.Clear();
    }

    public void Setup()
    {
        initialized = true;
    }

    public void AddInteraction(PersistentObject po)
    {
        if (String.IsNullOrEmpty(po.Id) || po.Id == "" || String.IsNullOrWhiteSpace(po.Id))
        {
            Debug.LogWarningFormat("{0} PersistantObject ID is invalid", po.gameObject.name);
        }
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