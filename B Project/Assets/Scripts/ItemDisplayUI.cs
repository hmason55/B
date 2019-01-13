using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayUI : MonoBehaviour
{
    // Collection of UI icons
    public Image[] ItemIcons; 

    void Awake()
    {
        // Turn all icons and text off
        for (int i = 0; i < ItemIcons.Length; i++)
        {
            ItemIcons[i].enabled = false;
            Text itemText = ItemIcons[i].GetComponentInChildren<Text>();
            itemText.enabled = false;
        }
    }

    public void UpdateItems( List<BaseItem> items)
    {
        for (int i = 0; i < ItemIcons.Length; i++)
        {
            ItemIcons[i].enabled = (i < items.Count);
            Text itemText = ItemIcons[i].GetComponentInChildren<Text>();
            itemText.enabled = false;
            if (i < items.Count)
            {
                ItemIcons[i].sprite = items[i].Icon;
                itemText.text = items[i].GetDescription();
            }

        }
    }

}
