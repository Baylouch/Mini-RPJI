﻿using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    private void Awake()
    {
        if (control == null)
        {
            control = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ShowPath();
    }

    public void ShowPath()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void SavePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/PlayerData.max");

        PlayerData data = new PlayerData();
    
        // Player Stats
        data.playerStats = new PlayerStatsData();

        data.playerStats.level = Player_Stats.stats_instance.getCurrentLevel();
        data.playerStats.totalLevelExp = Player_Stats.stats_instance.getTotalLevelExp();
        data.playerStats.currentExp = Player_Stats.stats_instance.getCurrentExp();
        data.playerStats.strenght = Player_Stats.stats_instance.GetBaseStatsByType(StatsType.STRENGTH);
        data.playerStats.agility = Player_Stats.stats_instance.GetBaseStatsByType(StatsType.AGILITY);
        data.playerStats.vitality = Player_Stats.stats_instance.GetBaseStatsByType(StatsType.VITALITY);
        data.playerStats.intellect = Player_Stats.stats_instance.GetBaseStatsByType(StatsType.INTELLECT);
        data.playerStats.currentStatsPoints = Player_Stats.stats_instance.getCurrentStatsPoints();

        // Player Inventory
        data.playerInventory = new PlayerInventoryData(); // Create new instance of PlayerInventoryData

        data.playerInventory.inventoryItems = new int[Player_Inventory.inventorySlotsNumb]; // Create new int array to contain item's id
        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++) // Reset all number to -1 (usefull for load part)
        {
            data.playerInventory.inventoryItems[i] = -1;
        }
        // Then if there is an item in the current slot, get his ID into inventoryItems data.
        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++) 
        {
            if (Player_Inventory.inventory_instance.GetInventorySlotItem(i) != null)
            {
                data.playerInventory.inventoryItems[i] = Player_Inventory.inventory_instance.GetInventorySlotItem(i).itemID;
            }
        }

        // Exactly same as inventoryItems
        data.playerInventory.armoryItems = new int[Player_Inventory.armorySlotsNumb];
        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            data.playerInventory.armoryItems[i] = -1;
        }

        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            if (Player_Inventory.inventory_instance.GetArmorySlotItem(i) != null)
            {
                data.playerInventory.armoryItems[i] = Player_Inventory.inventory_instance.GetArmorySlotItem(i).itemID;
            }
        }

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadPlayerData()
    {      
        if (File.Exists(Application.persistentDataPath + "/PlayerData.max"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.max", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            // Player Stats
            Player_Stats.stats_instance.SetCurrentLevel(data.playerStats.level);
            Player_Stats.stats_instance.SetTotalLevelExp(data.playerStats.totalLevelExp);
            Player_Stats.stats_instance.SetCurrentLevelExp(data.playerStats.currentExp);
            Player_Stats.stats_instance.SetBaseStatsByType(StatsType.STRENGTH, data.playerStats.strenght);
            Player_Stats.stats_instance.SetBaseStatsByType(StatsType.AGILITY, data.playerStats.agility);
            Player_Stats.stats_instance.SetBaseStatsByType(StatsType.VITALITY, data.playerStats.vitality);
            Player_Stats.stats_instance.SetBaseStatsByType(StatsType.INTELLECT, data.playerStats.intellect);
            Player_Stats.stats_instance.SetCurrentStatsPoints(data.playerStats.currentStatsPoints);

            // Player Inventory
            // Remove all item in inventory to not have unsaved item.
            for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
            {
                Player_Inventory.inventory_instance.RemoveItem(i, false);
            }
            // Same for armory slots
            for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
            {
                Player_Inventory.inventory_instance.RemoveArmoryItem(i, false);
            }

            // And now set registered item into their slots.
            for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
            {
                if (data.playerInventory.inventoryItems[i] != -1) // If we set this, there is an item ID in
                {
                    Player_Inventory.inventory_instance.SetInventorySlots(i, data.playerInventory.inventoryItems[i]);
                }
            }
            
            for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
            {
                if (data.playerInventory.armoryItems[i] != -1) // If we set this, there is an item ID in
                {
                    Player_Inventory.inventory_instance.SetArmorySlots(i, data.playerInventory.armoryItems[i]);
                }
            }

            // Then refresh all for the right display
            Player_Inventory.inventory_instance.RefreshInventory();
            Player_Inventory.inventory_instance.RefreshArmory();

            Player_Stats.stats_instance.RefreshPlayerStats();
        }       
    }
}

[Serializable]
class PlayerData
{
    public PlayerStatsData playerStats;
    public PlayerInventoryData playerInventory;
}

[Serializable]
class PlayerStatsData
{
    public int level;
    public int totalLevelExp;
    public int currentExp;

    public int strenght;
    public int agility;
    public int vitality;
    public int intellect;

    public int currentStatsPoints;
}

[Serializable]
class PlayerInventoryData
{
    // We get the unique item ID of items in inventory and in armory
    public int[] inventoryItems;
    public int[] armoryItems;
}
