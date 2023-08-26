using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;
    public static Inventory Instance { get { return instance; } }

    [SerializeField] private GameObject itemSlotPrefab;

    private int capacity;
    public int Capacity { get { return capacity; } 
        set
        {
            Item[] newItems = new Item[value];
            items.CopyTo(newItems, Mathf.Min(capacity,value));
            items = newItems;
            capacity = value;
            UpdateUI();
        }
    }

    private int selectedItem;
    public Item SelectedItem { get { return items[selectedItem]; } }

    public event Action onSelect;

    private Item[] items;
    private InventorySlot[] slots;

    public void Selected(int id)
    {
        selectedItem = id;
    }

    public void Used(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Equals(item))
            {
                Used(i);
                break;
            }
        }
    }

    public void Used(int id)
    {
        items[id] = null;
        slots[id].SetItem(null);
    }

    public void UpdateUI()
    {
        if (transform.childCount != capacity)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i));
            }
            for (int i = 0; i < capacity; i++)
            {
                slots = new InventorySlot[capacity];
                InventorySlot slot = Instantiate(itemSlotPrefab, transform).GetComponent<InventorySlot>();
                slot.SetId(i);
                slots[i] = slot;
            }
        }
        for (int i = 0; i < capacity; i++)
        {
            slots[i].SetItem(items[i]);
        }
    }

    private void Awake()
    {
        instance = this;
    }
    
}
