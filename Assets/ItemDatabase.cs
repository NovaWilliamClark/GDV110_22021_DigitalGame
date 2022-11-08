using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    
    // these values could be a scriptable object because the world needs more scriptable objects!
    [Header("Type References")] [SerializeField]
    private ItemTypeEnum DefaultType;
    [SerializeField] private ItemTypeEnum SockyType;
    [SerializeField] private ItemTypeEnum BatteryType;
    [SerializeField] private ItemTypeEnum ChipsType;
    [SerializeField] private ItemTypeEnum KeyType;
    [SerializeField] private ItemTypeEnum FuseType;
    [SerializeField] private ItemTypeEnum FlashlightType;
        
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateLinks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    private void CreateLinks()
    {
        foreach (var entry in database)
        {
            var type = entry.item.typeEnum;
            if (type == SockyType)
            {
                entry.refEffect = new SockeyItemEffect(entry.useEvent);
            }
            
            if (type == BatteryType)
            {
                entry.refEffect = new BatteryItemEffect(entry.useEvent);
            }
            if (type == ChipsType)
            {
                Debug.LogWarning("Chips type isn't implemented");
            }
            if (type == KeyType)
            {
                Debug.LogWarning("Key Type effect isn't implemented");   
            }
            if (type == FuseType)
            {
                entry.refEffect = new FuseItemEffect(entry.useEvent);
            }

            if (type == FlashlightType)
            {
                entry.refEffect = new FlashlightItemEffect(entry.useEvent);
            }

            if (type == null || type == DefaultType)
            {
                entry.refEffect = new DefaultItemEffect(entry.useEvent);
            }
        }
        
    }

    public ItemEffect GetItemEffect(ItemData itemRef)
    {
        var effect =
            (from entry in database
                where entry.item == itemRef || itemRef.IsInstanceOf(entry.item)
                select entry.refEffect).FirstOrDefault();
        if (effect == null)
        {
            Debug.LogWarningFormat("{0} effect on {1} doesn't exist - has it been added?",itemRef.typeEnum, itemRef);
        }

        return effect;
    }

    public ItemDatabaseEntry GetEntry(ItemData itemData, bool isInstance = false)
    {
        if (isInstance)
        {
            return database.Where(entry => itemData.IsInstanceOf(entry.item)).Select(entry => entry)
                .FirstOrDefault();
        }

        return database.FirstOrDefault(entry => entry.item == itemData);
    }
    
    public ItemData GetOriginalItem(ItemData instance)
    {
        return database.Where(entry => instance.IsInstanceOf(entry.item)).Select(entry => entry.item).FirstOrDefault();
    }

    public ItemUseEvent GetItemEvent(ItemData itemRef)
    {
        var useEvent =
            (from entry in database
                where entry.item == itemRef || itemRef.IsInstanceOf(entry.item)
                select entry.useEvent).FirstOrDefault();
        return useEvent;
    }

    [SerializeField] private List<ItemDatabaseEntry> database;
}

[Serializable]
public class ItemDatabaseEntry
{
    public ItemData item;
    public ItemEffect refEffect;
    public ItemUseEvent useEvent;
}
