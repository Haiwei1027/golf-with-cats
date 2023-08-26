using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image itemImage;

    private Item item;
    private int id;

    public void SetItem(Item item)
    {
        this.item = item;

        UpdateVisual();
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void Select()
    {
        Inventory.Instance.Selected(id);
    }

    public void UpdateVisual()
    {
        itemImage.enabled = false;
        itemImage.sprite = null;
        if (item != null)
        {
            itemImage.enabled = true;
            itemImage.sprite = item.itemSprite;
        }
    }
}
