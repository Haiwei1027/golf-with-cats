using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemBank
{
    public static List<Item> items;

    public static void LoadAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:Item");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            items.Add(AssetDatabase.LoadAssetAtPath<Item>(path));
        }
    }

    public static Item GetItem(string name)
    {
        if (items == null)
        {
            items = new List<Item>();
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
