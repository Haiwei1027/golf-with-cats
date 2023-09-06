using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Class responsible for holding items the player has and providing interface to access those items for other systems
/// </summary>
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
        SetItem(1, ItemBank.GetItem("Test Item"));
        SetItem(2, ItemBank.GetItem("Test Item"));
        SetItem(3, ItemBank.GetItem("Test Item"));

    }

    private void Update()
    {
/*        for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
        {
            if (i - (int)KeyCode.Alpha1 > Capacity) break;
            if (Input.GetKeyDown((KeyCode)i))
            {
                Selected(i - (int)KeyCode.Alpha1);
                slots[i - (int)KeyCode.Alpha1].GetComponent<Animator>().SetTrigger("Selected");
            }
        }*/
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
        UnSelect();
        selectedItem = items[id];
        onSelect?.Invoke(id);
        Debug.LogAssertion($"Selected {selectedItem}");
    }

    public void UnSelect()
    {
        onUnselect?.Invoke();
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
