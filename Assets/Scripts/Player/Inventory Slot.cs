using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for updating the UI of an inventory slot
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image itemImage;

    private Button button;

    private Item item;
    public Item Item { get { return item; } set { item = value; UpdateVisual(); } }
    private int id;
    public int Id { get { return id; } set { id = value; name = name + " (" + id + ")"; } }

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
    }

    public void Select()
    {
        Inventory.Instance.Selected(id);
    }

    private void Update()
    {
        /*if (selected)
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
            {
                Inventory.Instance.UnSelect(id);
            }
        }*/
    }

    public void UpdateVisual()
    {
        itemImage.enabled = false;
        itemImage.sprite = null;
        button.interactable = false;
        if (item != null)
        {
            button.interactable = true;
            itemImage.enabled = true;
            itemImage.sprite = item.itemSprite;
        }
    }
}
