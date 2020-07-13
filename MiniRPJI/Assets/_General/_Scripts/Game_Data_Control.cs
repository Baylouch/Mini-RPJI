﻿using UnityEngine;
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
        FileStream file = File.Create(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji");

        PlayerData data = new PlayerData();

        // ************************************************************************************************
        // ********************************* PLAYER_STATS *************************************************
        // ************************************************************************************************

        data.playerStats = new PlayerStatsData();

        data.playerStats.level = Player_Stats.instance.GetCurrentLevel();
        data.playerStats.totalLevelExp = Player_Stats.instance.GetTotalLevelExp();
        data.playerStats.currentExp = Player_Stats.instance.GetCurrentExp();
        data.playerStats.strenght = Player_Stats.instance.GetBaseStatsByType(StatsType.STRENGTH);
        data.playerStats.agility = Player_Stats.instance.GetBaseStatsByType(StatsType.AGILITY);
        data.playerStats.vitality = Player_Stats.instance.GetBaseStatsByType(StatsType.VITALITY);
        data.playerStats.intellect = Player_Stats.instance.GetBaseStatsByType(StatsType.ENERGY);
        data.playerStats.currentStatsPoints = Player_Stats.instance.GetCurrentStatsPoints();

        // ************************************************************************************************
        // ********************************* PLAYER_INVENTORY *********************************************
        // ************************************************************************************************

        data.playerInventory = new PlayerInventoryData(); // Create new instance of PlayerInventoryData

        data.playerInventory.inventoryItems = new int[Player_Inventory.inventorySlotsNumb]; // Create new int array to contain item's id
        data.playerInventory.inventoryItemsNumber = new int[Player_Inventory.inventorySlotsNumb]; // Create new int array to contain item's number (linked to inventoryItems -> itemsNumber[i]'s item = inventoryItems[i]'s item.)

        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++) // Reset all number to -1 (usefull for load part)
        {
            data.playerInventory.inventoryItems[i] = -1;
            data.playerInventory.inventoryItemsNumber[i] = 0; // Initialize itemsNumber to 0 (As every items that are not stackable)
        }
        // Then if there is an item in the current slot, get his ID into inventoryItems data.
        // And his number if its a stackable item.
        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++) 
        {
            if (Player_Inventory.instance.GetInventoryItem(i) != null)
            {
                data.playerInventory.inventoryItems[i] = Player_Inventory.instance.GetInventoryItem(i).itemID;

                // If current item is a stackable one
                if (Player_Inventory.instance.GetInventoryItem(i).stackableItem)
                {
                    // Get acces to the inventory slot by UI_Player_Inventory
                    if (UI_Player.instance && UI_Player.instance.playerInventoryUI)
                    {
                        if (UI_Player.instance.playerInventoryUI.GetInventorySlotByIndex(i) != null)
                        {
                            data.playerInventory.inventoryItemsNumber[i] = UI_Player.instance.playerInventoryUI.GetInventorySlotByIndex(i).itemNumb;
                        }
                    }
                }
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

        // Then same for the bank
        data.playerInventory.bankItems = new int[UI_Player_Bank.bankSlotsNumb];
        data.playerInventory.bankItemsNumber = new int[UI_Player_Bank.bankSlotsNumb];

        for (int i = 0; i < UI_Player_Bank.bankSlotsNumb; i++)
        {
            data.playerInventory.bankItems[i] = -1;
            data.playerInventory.bankItemsNumber[i] = 0;
        }

        for (int i = 0; i < UI_Player_Bank.bankSlotsNumb; i++)
        {
            if (UI_Player.instance.playerBankUI.GetBankItem(i) != null)
            {
                data.playerInventory.bankItems[i] = UI_Player.instance.playerBankUI.GetBankItem(i).itemID;

                // If current item is a stackable one
                if (UI_Player.instance.playerBankUI.GetBankItem(i).stackableItem)
                {
                    data.playerInventory.bankItemsNumber[i] = UI_Player.instance.playerBankUI.GetBankSlotByIndex(i).itemNumb;
                }
            }
        }

        // Get player's gold
        data.playerInventory.gold = Player_Inventory.instance.GetPlayerGold();

        // ************************************************************************************************
        // ********************************* PLAYER_QUEST *************************************************
        // ************************************************************************************************

        // TODO upgrade if there are more than one Act, to get all quests by act.
        data.playerQuest = new PlayerQuestData();
        data.playerQuest.questsID = new int[200];
        data.playerQuest.questsCurrentObjective = new int[200];

        // First set data quests ID to -1 to know what index of quest is set (index set != -1)
        for (int i = 0; i < 200; i++)
        {
            data.playerQuest.questsID[i] = -1;
        }

        // Now save which quest player has and currentQuestObjective linked.
        for (int i = 0; i < 200; i++)
        {
            if (Quests_Control.instance.GetPlayerQuestByIndex(i))
            {
                data.playerQuest.questsID[i] = Quests_Control.instance.GetPlayerQuestByIndex(i).questConfig.questID;
                data.playerQuest.questsCurrentObjective[i] = Quests_Control.instance.GetPlayerQuestByIndex(i).currentQuestObjective;
            }
        }

        // Then save the quests done by player : loop trough all quests to set it.
        int totalGameQuests = Quests_Control.instance.questDataBase.quests.Length;
        data.playerQuest.questsDone = new bool[totalGameQuests];

        for (int i = 0; i < totalGameQuests; i++)
        {
            if (Quests_Control.instance.questDataBase.quests[i].questDone)
            {
                data.playerQuest.questsDone[i] = true;
            }
            else
            {
                data.playerQuest.questsDone[i] = false;
            }
        }

        // ************************************************************************************************
        // ********************************* PLAYER_ABILITIES *********************************************
        // ************************************************************************************************

        data.playerAbilities = new PlayerAbilitiesData();

        // Start by get numbers of available's abilities
        int abilitiesAvailable = 0;
        for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
        {
            if (Player_Abilities.instance.GetUnlockAbility(i) == true)
            {
                abilitiesAvailable++;
            }
        }

        // Create data abilities array with the right size.
        data.playerAbilities.abilitiesID = new int[abilitiesAvailable];
        int indexDataAbilitiesArray = 0; // To loop trough array using other variable than the loop one (i)

        // now we can put available abilities ID in the data array. i represent abilit'y ID
        for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
        {
            if (Player_Abilities.instance.GetUnlockAbility(i) == true)
            {
                data.playerAbilities.abilitiesID[indexDataAbilitiesArray] = i;
                indexDataAbilitiesArray++;
            }
        }

        // Get primary and secondary ability
        data.playerAbilities.primaryAbilityID = Player_Abilities.instance.GetPrimaryAbility().abilityID;
        data.playerAbilities.secondaryAbilityID = Player_Abilities.instance.GetSecondaryAbility().abilityID;

        // ************************************************************************************************
        // ********************************* PLAYER_PETS **************************************************
        // ************************************************************************************************

        data.playerPets = new PlayerPetsData();

        data.playerPets.petsUnlocked = Player_Pets.instance.GetPetsUnlocked();

        // If pets are unlocked, get player pets ID
        if (data.playerPets.petsUnlocked)
        {
            int playerPetsNumb = 0;

            for (int i = 0; i < Player_Pets.playerPetsLength; i++)
            {
                if (Player_Pets.instance.GetPlayerPetByIndex(i) != null)
                {
                    playerPetsNumb++;
                }
            }

            data.playerPets.petsID = new int[playerPetsNumb];

            if (playerPetsNumb > 0)
            {
                int currentPlayerPetsIDIndex = 0;

                for (int i = 0; i < Player_Pets.playerPetsLength; i++)
                {
                    if (Player_Pets.instance.GetPlayerPetByIndex(i) != null)
                    {
                        data.playerPets.petsID[currentPlayerPetsIDIndex] = Player_Pets.instance.GetPlayerPetByIndex(i).petID;
                        currentPlayerPetsIDIndex++;
                    }
                }
            }

            if (Player_Pets.instance.currentPlayerPet != null)
            {
                data.playerPets.playerCurrentPetID = Player_Pets.instance.currentPlayerPet.petID;
            }
            else
            {
                data.playerPets.playerCurrentPetID = -1;
            }
        }

        // ************************************************************************************************
        // ********************************* TELEPORTERS **************************************************
        // ************************************************************************************************

        data.playerTps = new PlayerTeleportersData();

        data.playerTps.tpsUnlocked = new bool[UI_Teleporter.teleporterNumber];

        for (int i = 0; i < UI_Teleporter.teleporterNumber; i++)
        {
            if (UI_Player.instance.teleporterUI.GetUnlockedTp(i))
            {
                data.playerTps.tpsUnlocked[i] = true;
            }
            else
            {
                data.playerTps.tpsUnlocked[i] = false;
            }
        }

        // ************************************************************************************************
        // ************************************************************************************************
        // ************************************************************************************************

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadPlayerData(int saveIndex)
    {      
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            // ************************************************************************************************
            // ********************************* PLAYER_STATS *************************************************
            // ************************************************************************************************

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

            // ************************************************************************************************
            // ********************************* PLAYER_INVENTORY *********************************************
            // ************************************************************************************************

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
            // Same for bank slots
            for (int i = 0; i < UI_Player_Bank.bankSlotsNumb; i++)
            {
                UI_Player.instance.playerBankUI.SetBankItemSlot(i, -1);
            }

            // And now set registered item into their slots.
            for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
            {
                if (data.playerInventory.inventoryItems[i] != -1) // If we had set this, there is an item ID in
                {
                    Player_Inventory.instance.SetInventoryIndex(i, data.playerInventory.inventoryItems[i], data.playerInventory.inventoryItemsNumber[i]);
                }
            }
            
            for (int i = 0; i < Player_Inventory.armorySlotsNumb; i++)
            {
                if (data.playerInventory.armoryItems[i] != -1) // If we set this, there is an item ID in
                {
                    Player_Inventory.instance.SetArmoryIndex(i, data.playerInventory.armoryItems[i]);
                }
            }

            for (int i = 0; i < UI_Player_Bank.bankSlotsNumb; i++)
            {
                if (data.playerInventory.bankItems[i] != -1)
                {
                    UI_Player.instance.playerBankUI.SetBankItemSlot(i, data.playerInventory.bankItems[i], data.playerInventory.bankItemsNumber[i]);
                }
            }

            Player_Inventory.instance.SetPlayerGold(0);
            Player_Inventory.instance.SetPlayerGold(data.playerInventory.gold);

            // Then refresh all for the right display
            UI_Player.instance.playerInventoryUI.RefreshInventory();
            UI_Player.instance.playerInventoryUI.RefreshArmory();

            Player_Stats.instance.RefreshPlayerStats();

            // ************************************************************************************************
            // ********************************* PLAYER_QUESTS ************************************************
            // ************************************************************************************************

            // First of all remove current quests and reset all quests achievement.
            for (int i = 0; i < 200; i++)
            {
                Quests_Control.instance.RemoveQuestByIndex(i);
            }

            Quests_Control.instance.ResetAllQuestsAchievement();

            // Get the total quests in the game to set the ones player already done.
            int totalGameQuests = Quests_Control.instance.questDataBase.quests.Length;

            for (int i = 0; i < totalGameQuests; i++)
            {
                if (data.playerQuest.questsDone[i])
                {
                    int currentQuestID = Quests_Control.instance.questDataBase.quests[i].questID;
                    Quests_Control.instance.SetQuestAchievement(currentQuestID, true);
                }
            }

            // Then set player's current quests.
            for (int i = 0; i < 200; i++)
            {
                if (data.playerQuest.questsID[i] != -1)
                {
                    Quests_Control.instance.GetNewQuest(data.playerQuest.questsID[i]);
                    Quests_Control.instance.GetPlayerQuestByIndex(i).currentQuestObjective = data.playerQuest.questsCurrentObjective[i];
                }
            }

            // ************************************************************************************************
            // ********************************* PLAYER_ABILITIES *********************************************
            // ************************************************************************************************

            // First lock all abilities
            for (int i = 0; i < Player_Abilities.instance.abilitiesDatabase.abilities.Length; i++)
            {
                Player_Abilities.instance.SetUnlockAbility(i, false);
            }

            // Simply loop trough our data array and unlock abilities ID linked
            for (int i = 0; i < data.playerAbilities.abilitiesID.Length; i++)
            {
                Player_Abilities.instance.SetUnlockAbility(data.playerAbilities.abilitiesID[i], true);
            }

            // Set primary and secondary abilities
            UI_Player_Abilities.instance.ChangeAbility(data.playerAbilities.primaryAbilityID, 0);
            UI_Player_Abilities.instance.ChangeAbility(data.playerAbilities.secondaryAbilityID, 1);

            // ************************************************************************************************
            // ********************************* PLAYER_PETS **************************************************
            // ************************************************************************************************

            Player_Pets.instance.SetPetsUnlocked(data.playerPets.petsUnlocked);

            if (data.playerPets.petsUnlocked)
            {
                for (int i = 0; i < data.playerPets.petsID.Length; i++)
                {
                    Player_Pets.instance.GetNewPet(Player_Pets.instance.petsDataBase.GetPetByID(data.playerPets.petsID[0]));
                }

                if (data.playerPets.playerCurrentPetID != -1)
                {
                    Player_Pets.instance.InvokePet(Player_Pets.instance.petsDataBase.GetPetByID(data.playerPets.playerCurrentPetID));
                }
            }

            // ************************************************************************************************
            // ********************************* TELEPORTERS **************************************************
            // ************************************************************************************************

            for (int i = 0; i < UI_Teleporter.teleporterNumber; i++)
            {
                if (data.playerTps.tpsUnlocked[i])
                {
                    UI_Player.instance.teleporterUI.SetUnlockedTp(i, true);
                }
                else
                {
                    UI_Player.instance.teleporterUI.SetUnlockedTp(i, false);
                }
            }

            // ************************************************************************************************
            // ************************************************************************************************
            // ************************************************************************************************
        }
    }

    public PlayerData GetLoadData(int saveIndex)
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            return data;
        }

        return null;
    }

    public void RemoveData(int saveIndex)
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji"))
        {
            File.Delete(Application.persistentDataPath + "/PlayerData" + saveIndex.ToString() + ".mrpji");
        }
    }
}

[Serializable]
public class PlayerData
{
    public PlayerStatsData playerStats;
    public PlayerInventoryData playerInventory;
    public PlayerQuestData playerQuest;
    public PlayerAbilitiesData playerAbilities;
    public PlayerPetsData playerPets;
    public PlayerTeleportersData playerTps;
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
    // We get the unique item ID of items in inventory, armory and bank
    public int[] inventoryItems;
    public int[] inventoryItemsNumber; // To know, if its a stackable item, how much player has (0 = not a stackable item)
    public int[] armoryItems;
    public int[] bankItems;
    public int[] bankItemsNumber; // Same as inventoryItemsNumber for the bank this time.

    public int gold;
}

[Serializable]
public class PlayerQuestData
{
    public int[] questsID; // Contain current player quest. We got them by index into Player quest control playerQuests
    public int[] questsCurrentObjective; // Contain the quest progress. Use same index as questsID
    public bool[] questsDone; // To know which quests are already done for this save. Index array is used as quest ID. We loop trough all quests in the game.
}

[Serializable]
public class PlayerAbilitiesData
{
    public int[] abilitiesID; // Contain all ID's of available abilities for this save.

    public int primaryAbilityID; // To know what's the current primary ability
    public int secondaryAbilityID;
}

[Serializable]
public class PlayerPetsData
{
    public bool petsUnlocked; // To know if player already unlock pets or not

    public int[] petsID; // All pet's ID own by the player
    public int playerCurrentPetID; // The current pet ID of the player
}

[Serializable]
public class PlayerTeleportersData
{
    public bool[] tpsUnlocked; // Represent unlockedTps in UI_Teleporter.cs
}
