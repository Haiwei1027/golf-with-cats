using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that fetche and store a refernece for every item
/// </summary>
public class ItemBank : ScriptableObject
{
    public static ItemBank Instance;

    public static Item[] items;

    public static void LoadAllItems()
    {
        items = Resources.LoadAll<Item>("Items");
        Debug.LogAssertion($"Loaded {items.Length} items");
    }

    public static Item GetItem(string name)
    {
        if (items == null)
        {
            LoadAllItems();
        }
        foreach (Item item in items)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }
        return null;
    }
}
