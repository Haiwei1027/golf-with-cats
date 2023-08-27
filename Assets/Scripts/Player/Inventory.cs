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
            if (items != null)
            {
                Item[] newItems = new Item[value];
                items.CopyTo(newItems, Mathf.Min(capacity, value));
                items = newItems;
            }
            else
            {
                items = new Item[value];
            }
            capacity = value;
            UpdateUI();
        }
    }

    private Item selectedItem;
    public Item SelectedItem { get { return selectedItem; } }

    public event Action<int> onSelect;
    public event Action onUnselect;
    public event Action<Item> onUse;

    private Item[] items;
    private InventorySlot[] slots;

    private void Start()
    {

        Capacity = 4;
        SetItem(0, ItemBank.GetItem("Test Item"));

    }

    public Item BorrowItem(int index)
    {
        if (index < 0 || index >= items.Length) { return null; }
        slots[index].Item = null;
        return items[index];
    }

    public void ReturnItem(int index)
    {
        if (index < 0 || index >= items.Length) { return; }
        if (items[index] != null && slots[index].Item == null)
        {
            slots[index].Item = items[index];
        }
    }

    public void SetItem(int index, Item item)
    {
        if (index < 0 || index >= items.Length) { return; }
        items[index] = item;
        slots[index].Item = item;
    }

    public void Selected(int id)
    {
        if (id < 0 || id > items.Length) { return; }
        selectedItem = items[id];
        onSelect?.Invoke(id);
        Debug.LogAssertion($"Selected {selectedItem}");
    }

    public void UnSelect(int id)
    {
        if (id < 0 || id > items.Length) { return; }
        if (selectedItem == items[id])
        {
            Debug.LogAssertion($"Unselected {selectedItem}");
            selectedItem = null;

            onUnselect?.Invoke();
            
        }
    }

    public int GetIndex(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    public void Used(Item item)
    {
        Used(GetIndex(item));
    }

    public void Used(int id)
    {
        if (id < 0 || id > items.Length) { return; }
        Debug.LogAssertion($"Used {id}");
        onUse?.Invoke(items[id]);

        items[id] = null;
        slots[id].Item = null;
    }

    public void UpdateUI()
    {
        if (transform.childCount != capacity)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            slots = new InventorySlot[capacity];
            for (int i = 0; i < capacity; i++)
            {
                InventorySlot slot = Instantiate(itemSlotPrefab, transform).GetComponent<InventorySlot>();
                slot.Id = i;
                slot.Item = items[i];
                slots[i] = slot;
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }
    
}
