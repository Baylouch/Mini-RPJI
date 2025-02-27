﻿using UnityEngine;
using UnityEngine.UI;

public class UI_DisplayItemStats : MonoBehaviour
{
    // To display item's stats
    [SerializeField] GameObject statsItemPanel; // This gameobject prefab must be set 0 width and 0 height on his rectTransform 
    [SerializeField] GameObject itemName; // Automaticaly scaled by Vertical Layout group on statsItemPanel
    [SerializeField] GameObject itemDescription; // For item's description
    [SerializeField] GameObject statsNameAndPoints; // Same as itemName
    // statsItemPanel will contains all others UI elements who display item's stats. statsBackgroundPanelRect is rect of current statsItemPanel display
    // so just destroy gameobject attach to statsBackgroundPanelRect to remove stats display
    RectTransform statsPanelRect; // To set hierarchy and item's stats stuff

    private void OnLevelWasLoaded(int level)
    {
        HideAndReset();
    }

    // Method to create new Text element in DisplayItemStats(ItemConfig)
    void CreateItemStatsText(string statsName, float statsPoints)
    {
        GameObject _statsText = Instantiate(statsNameAndPoints); // Instatiate text element prefab
        // Check if statsPoints is > or < to 0 for set green or red text
        if (statsPoints > 0)
        {
            _statsText.GetComponent<Text>().text = statsName + ": " + "<color=green>" + statsPoints + "</color>"; // Set text with greend pts
        }
        else
        {
            _statsText.GetComponent<Text>().text = statsName + ": " + "<color=red>" + statsPoints + "</color>"; // Set text with red pts
        }

        _statsText.transform.SetParent(statsPanelRect.transform); // Set hierarchy
        _statsText.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale (idk why but without scale do weird things)
        // Add 30 to the height of the panel for each new element
        statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
    }

    // Method to display equipment item stats on UI (used on buttons configurations methods)
    public void DisplayItemStats(EquipmentItem item)
    {
        // First of all, instantiate statsItemPanel
        GameObject statsBackgroundPanel = Instantiate(statsItemPanel); // Create background panel
        statsPanelRect = statsBackgroundPanel.GetComponent<RectTransform>(); // Get current rectransform
        statsBackgroundPanel.transform.SetParent(transform); // Set background panel hierarchy
        statsBackgroundPanel.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale

        // Second add item name text and set it
        GameObject itemName = Instantiate(this.itemName); // Create item name text
        Text itemNameText = itemName.GetComponent<Text>();
        // Switch on every rarety to set correct color on item name
        switch (item.rarety)
        {
            case ItemRarety.Common:
                itemNameText.color = Color.white;
                break;
            case ItemRarety.Uncommon:
                itemNameText.color = Color.cyan;
                break;
            case ItemRarety.Rare:
                itemNameText.color = Color.yellow;
                break;
            case ItemRarety.Epic:
                itemNameText.color = Color.magenta;
                break;
            case ItemRarety.Legendary:
                itemNameText.color = Color.red; // TODO modify
                break;
        }
        itemName.GetComponent<Text>().text = item.itemName; // Set item name text
        itemName.transform.SetParent(statsBackgroundPanel.transform); // Set item name text hierarchy
        itemName.GetComponent<RectTransform>().localScale = Vector3.one;
        // Add 30 to panel's height
        statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);

        // Stats are tested in order we want to display them

        if (item.damageMin != 0) // We only check damageMin and rangedDamageMin. If min is set max must be set /!!!!!\ 
        {
            GameObject _damageText = Instantiate(statsNameAndPoints);
            if (item.damageMin > 0)
            {
                _damageText.GetComponent<Text>().text = "Damage: <color=green>" + item.damageMin + "</color> - <color=green>" + item.damageMax + "</color>";
            }
            else
            {
                _damageText.GetComponent<Text>().text = "Damage: <color=red>" + item.damageMin + "</color> - <color=red>" + item.damageMax + "</color>";
            }
            _damageText.transform.SetParent(statsPanelRect.transform);
            _damageText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
        if (item.rangedDamageMin != 0)
        {
            GameObject _rangedDamageText = Instantiate(statsNameAndPoints);
            if (item.rangedDamageMin > 0)
            {
                _rangedDamageText.GetComponent<Text>().text = "Ranged dmg: <color=green>" + item.rangedDamageMin + "</color> - <color=green>" + item.rangedDamageMax + "</color>";
            }
            else
            {
                _rangedDamageText.GetComponent<Text>().text = "Ranged dmg: <color=red>" + item.rangedDamageMin + "</color> - <color=red>" + item.rangedDamageMax + "</color>";
            }
            _rangedDamageText.transform.SetParent(statsPanelRect.transform);
            _rangedDamageText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
        if (item.healthpoints != 0)
        {
            CreateItemStatsText("Health", item.healthpoints);
        }
        if (item.armor != 0)
        {
            CreateItemStatsText("Armor", item.armor);
        }
        if (item.strength != 0)
        {
            CreateItemStatsText("Strength", item.strength);

        }
        if (item.agility != 0)
        {
            CreateItemStatsText("Agility", item.agility);
        }
        if (item.vitality != 0)
        {
            CreateItemStatsText("Vitality", item.vitality);
        }
        if (item.energy != 0)
        {
            CreateItemStatsText("Energy", item.energy);
        }
        if (item.criticalRate != 0)
        {
            GameObject _criticalRateText = Instantiate(statsNameAndPoints);
            if (item.criticalRate > 0)
            {
                _criticalRateText.GetComponent<Text>().text = "Crit: <color=green>" + item.criticalRate + "</color>%";
            }
            else
            {
                _criticalRateText.GetComponent<Text>().text = "Crit: <color=red>" + item.criticalRate + "</color>%";
            }
            _criticalRateText.transform.SetParent(statsPanelRect.transform);
            _criticalRateText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
        if (item.rangedCriticalRate != 0)
        {
            GameObject _rangedCriticalRateText = Instantiate(statsNameAndPoints);
            if (item.rangedCriticalRate > 0)
            {
                _rangedCriticalRateText.GetComponent<Text>().text = "Ranged crit: <color=green>" + item.rangedCriticalRate + "</color>%";
            }
            else
            {
                _rangedCriticalRateText.GetComponent<Text>().text = "Ranged crit: <color=red>" + item.rangedCriticalRate + "</color>%";
            }
            _rangedCriticalRateText.transform.SetParent(statsPanelRect.transform);
            _rangedCriticalRateText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
        if (item.levelRequired != 0)
        {
            GameObject _statsText = Instantiate(statsNameAndPoints); // Instatiate text element prefab
                                                                     // Check if statsPoints is > or < to 0 for set green or red text
            if (Player_Stats.instance.GetCurrentLevel() >= item.levelRequired)
            {
                _statsText.GetComponent<Text>().text = "Level required : <color=green>" + item.levelRequired + "</color>"; // Set text with greend pts
            }
            else
            {
                _statsText.GetComponent<Text>().text = "Level required : <color=red>" + item.levelRequired + "</color>"; // Set text with red pts
            }

            _statsText.transform.SetParent(statsPanelRect.transform); // Set hierarchy
            _statsText.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale (idk why but without scale do weird things)
                                                                               // Add 30 to the height of the panel for each new element
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }

        if (item.itemDescription != string.Empty)
        {
            GameObject descriptionText = Instantiate(itemDescription);

            descriptionText.GetComponent<Text>().text = "\"" + item.itemDescription + "\"";
            descriptionText.transform.SetParent(statsPanelRect.transform);
            descriptionText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }

        if (item.itemSellPrice != 0)
        {
            GameObject descriptionText = Instantiate(itemDescription);

            descriptionText.GetComponent<Text>().text = "Sell price : " + item.itemSellPrice.ToString();
            descriptionText.transform.SetParent(statsPanelRect.transform);
            descriptionText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
    }

    // Method to display item stats on UI (used on buttons configurations methods)
    public void DisplayItemStats(UsableItem item)
    {
        // First of all, instantiate statsItemPanel
        GameObject statsBackgroundPanel = Instantiate(statsItemPanel); // Create background panel
        statsPanelRect = statsBackgroundPanel.GetComponent<RectTransform>(); // Get current rectransform
        statsBackgroundPanel.transform.SetParent(transform); // Set background panel hierarchy
        statsBackgroundPanel.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale

        // Second add item name text and set it
        GameObject itemName = Instantiate(this.itemName); // Create item name text
        Text itemNameText = itemName.GetComponent<Text>();

        itemName.GetComponent<Text>().text = item.itemName; // Set item name text
        itemName.transform.SetParent(statsBackgroundPanel.transform); // Set item name text hierarchy
        itemName.GetComponent<RectTransform>().localScale = Vector3.one;

        // Add 30 to panel's height
        statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);

        // TODO Show description item

        if (item.healthRegenerationPoints != 0)
        {
            CreateItemStatsText("Health", item.healthRegenerationPoints);
        }
        if (item.energyRegenerationPoints != 0)
        {
            CreateItemStatsText("Energy", item.energyRegenerationPoints);
        }

        if (item.itemDescription != string.Empty)
        {
            GameObject descriptionText = Instantiate(itemDescription);

            descriptionText.GetComponent<Text>().text = "\"" + item.itemDescription + "\"";
            descriptionText.transform.SetParent(statsPanelRect.transform);
            descriptionText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }

        if (item.itemSellPrice != 0)
        {
            GameObject descriptionText = Instantiate(itemDescription);

            descriptionText.GetComponent<Text>().text = "Sell price : " + item.itemSellPrice.ToString();
            descriptionText.transform.SetParent(statsPanelRect.transform);
            descriptionText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);
        }
    }

    public void DisplayItemStats(QuestItem item)
    {
        // First of all, instantiate statsItemPanel
        GameObject statsBackgroundPanel = Instantiate(statsItemPanel); // Create background panel
        statsPanelRect = statsBackgroundPanel.GetComponent<RectTransform>(); // Get current rectransform
        statsBackgroundPanel.transform.SetParent(transform); // Set background panel hierarchy
        statsBackgroundPanel.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale

        // Second add item name text and set it
        GameObject itemName = Instantiate(this.itemName); // Create item name text
        Text itemNameText = itemName.GetComponent<Text>();

        itemName.GetComponent<Text>().text = item.itemName; // Set item name text
        itemName.transform.SetParent(statsBackgroundPanel.transform); // Set item name text hierarchy
        itemName.GetComponent<RectTransform>().localScale = Vector3.one;

        // Add 30 to panel's height
        statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 30);

        if (item.itemDescription != string.Empty)
        {
            GameObject descriptionText = Instantiate(itemDescription);

            descriptionText.GetComponent<Text>().text = "\"" + item.itemDescription + "\"";
            descriptionText.transform.SetParent(statsPanelRect.transform);
            descriptionText.GetComponent<RectTransform>().localScale = Vector3.one;
            statsPanelRect.sizeDelta = new Vector2(statsPanelRect.sizeDelta.x, statsPanelRect.sizeDelta.y + 50);
        }
    }

    public void HideAndReset()
    {
        // Destroy all child
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
