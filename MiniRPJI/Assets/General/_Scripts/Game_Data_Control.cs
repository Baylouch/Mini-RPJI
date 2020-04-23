using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Game_Data_Control : MonoBehaviour
{
    public static Game_Data_Control data_instance;

    private void Awake()
    {
        if (data_instance == null)
        {
            data_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //ShowPath();
    }

    public void ShowPath()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void SavePlayerData(int saveIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max");

        PlayerData data = new PlayerData();
    
        // Player Stats
        data.playerStats = new PlayerStatsData();

        data.playerStats.level = Player_Stats.instance.GetCurrentLevel();
        data.playerStats.totalLevelExp = Player_Stats.instance.GetTotalLevelExp();
        data.playerStats.currentExp = Player_Stats.instance.GetCurrentExp();
        data.playerStats.strenght = Player_Stats.instance.GetBaseStatsByType(StatsType.STRENGTH);
        data.playerStats.agility = Player_Stats.instance.GetBaseStatsByType(StatsType.AGILITY);
        data.playerStats.vitality = Player_Stats.instance.GetBaseStatsByType(StatsType.VITALITY);
        data.playerStats.intellect = Player_Stats.instance.GetBaseStatsByType(StatsType.ENERGY);
        data.playerStats.currentStatsPoints = Player_Stats.instance.GetCurrentStatsPoints();

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
            if (Player_Inventory.instance.GetInventoryItem(i) != null)
            {
                data.playerInventory.inventoryItems[i] = Player_Inventory.instance.GetInventoryItem(i).itemID;
            }
        }

        // Exactly same as inventoryItems, for armory.
        data.playerInventory.armoryItems = new int[Player_Inventory.armorySlotsNumb];
        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            data.playerInventory.armoryItems[i] = -1;
        }

        for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
        {
            if (Player_Inventory.instance.GetArmoryItem(i) != null)
            {
                data.playerInventory.armoryItems[i] = Player_Inventory.instance.GetArmoryItem(i).itemID;
            }
        }

        data.playerInventory.gold = Player_Inventory.instance.GetPlayerGold();

        // Player quests
        // TODO upgrade if there are more than one Act, to get all quests by act.
        data.playerQuest = new PlayerQuestData();
        data.playerQuest.questsID = new int[Player_Quest_Control.questSlotsNumb];
        data.playerQuest.questsCurrentObjective = new int[Player_Quest_Control.questSlotsNumb];

        // First set data quests ID to -1 to know what index of quest is set (index set != -1)
        for (int i = 0; i < Player_Quest_Control.questSlotsNumb; i++)
        {
            data.playerQuest.questsID[i] = -1;
        }

        // Now save which quest player has and currentQuestObjective linked.
        for (int i = 0; i < Player_Quest_Control.questSlotsNumb; i++)
        {
            if (Player_Quest_Control.instance.GetPlayerQuestByIndex(i))
            {
                data.playerQuest.questsID[i] = Player_Quest_Control.instance.GetPlayerQuestByIndex(i).questConfig.questID;
                data.playerQuest.questsCurrentObjective[i] = Player_Quest_Control.instance.GetPlayerQuestByIndex(i).currentQuestObjective;
            }
        }

        // Then save the quests done by player : loop trough all quests to set it.
        int totalGameQuests = Player_Quest_Control.instance.questDataBase.quests.Length;
        data.playerQuest.questsDone = new bool[totalGameQuests];

        for (int i = 0; i < totalGameQuests; i++)
        {
            if (Player_Quest_Control.instance.questDataBase.quests[i].questDone)
            {
                data.playerQuest.questsDone[i] = true;
            }
            else
            {
                data.playerQuest.questsDone[i] = false;
            }
        }

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadPlayerData(int saveIndex)
    {      
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            // Player Stats
            Player_Stats.instance.SetCurrentLevel(data.playerStats.level);
            Player_Stats.instance.SetTotalLevelExp(data.playerStats.totalLevelExp);
            Player_Stats.instance.SetCurrentLevelExp(data.playerStats.currentExp);
            Player_Stats.instance.SetBaseStatsByType(StatsType.STRENGTH, data.playerStats.strenght);
            Player_Stats.instance.SetBaseStatsByType(StatsType.AGILITY, data.playerStats.agility);
            Player_Stats.instance.SetBaseStatsByType(StatsType.VITALITY, data.playerStats.vitality);
            Player_Stats.instance.SetBaseStatsByType(StatsType.ENERGY, data.playerStats.intellect);
            Player_Stats.instance.SetCurrentStatsPoints(data.playerStats.currentStatsPoints);

            Player_Stats.instance.RefreshPlayerStats();

            Player_Stats.instance.playerHealth.SetCurrentHealthPoints(Player_Stats.instance.playerHealth.GetTotalHealthPoints());

            // Player Inventory
            // Remove all item in inventory to not have unsaved item.
            for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
            {
                Player_Inventory.instance.SetInventoryIndex(i, -1);
            }
            // Same for armory slots
            for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
            {
                Player_Inventory.instance.SetArmoryIndex(i, -1);
            }

            // And now set registered item into their slots.
            for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
            {
                if (data.playerInventory.inventoryItems[i] != -1) // If we had set this, there is an item ID in
                {
                    Player_Inventory.instance.SetInventoryIndex(i, data.playerInventory.inventoryItems[i]);
                }
            }
            
            for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
            {
                if (data.playerInventory.armoryItems[i] != -1) // If we set this, there is an item ID in
                {
                    Player_Inventory.instance.SetArmoryIndex(i, data.playerInventory.armoryItems[i]);
                }
            }

            Player_Inventory.instance.SetPlayerGold(0);
            Player_Inventory.instance.SetPlayerGold(data.playerInventory.gold);

            // Then refresh all for the right display
            UI_Player.instance.playerInventoryUI.RefreshInventory();
            UI_Player.instance.playerInventoryUI.RefreshArmory();

            // Player quests
            // First of all remove current quests and reset all quests achievement.
            for (int i = 0; i < Player_Quest_Control.questSlotsNumb; i++)
            {
                Player_Quest_Control.instance.RemoveQuestByIndex(i);                
            }

            Player_Quest_Control.instance.ResetAllQuestsAchievement();

            // Get the total quests in the game to set the ones player already done.
            int totalGameQuests = Player_Quest_Control.instance.questDataBase.quests.Length;

            for (int i = 0; i < totalGameQuests; i++)
            {
                if (data.playerQuest.questsDone[i])
                {
                    int currentQuestID = Player_Quest_Control.instance.questDataBase.quests[i].questID;
                    Player_Quest_Control.instance.SetQuestAchievement(currentQuestID, true);
                }
            }

            // Then set player's current quests.
            for (int i = 0; i < Player_Quest_Control.questSlotsNumb; i++)
            {
                if (data.playerQuest.questsID[i] != -1)
                {
                    Player_Quest_Control.instance.GetNewQuest(data.playerQuest.questsID[i]);
                    Player_Quest_Control.instance.GetPlayerQuestByIndex(i).currentQuestObjective = data.playerQuest.questsCurrentObjective[i];
                }
            }           
        }       
    }

    public PlayerData GetLoadData(int saveIndex)
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            return data;
        }

        return null;
    }

    public void RemoveData(int saveIndex)
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max"))
        {
            File.Delete(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".max");
        }
    }
}

[Serializable]
public class PlayerData
{
    public PlayerStatsData playerStats;
    public PlayerInventoryData playerInventory;
    public PlayerQuestData playerQuest;
}

[Serializable]
public class PlayerStatsData
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
public class PlayerInventoryData
{
    // We get the unique item ID of items in inventory and in armory
    public int[] inventoryItems;
    public int[] armoryItems;

    public int gold;
}

[Serializable]
public class PlayerQuestData
{
    public int[] questsID; // Contain current player quest. We got them by index into Player quest control playerQuests
    public int[] questsCurrentObjective; // Contain the quest progress. Use same index as questsID
    public bool[] questsDone; // To know which quests are already done for this save. Index array is used as quest ID.
}
