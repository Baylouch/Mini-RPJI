/* Cheats.cs
 * 
 * Contient tout les cheats du jeu.
 * 
 * Si le joueur appuie sur une des touches suivantes : a b c d e f g h i j k l m n o p q r s t u v w x y z 1 2 3 4 5 6 7 8 9 0
 * une chaine de caracteres retiendra les inputs du joueur et définira si la combinaison correspond à un cheat.
 * 
 * Un cheat est une combinaison de 20 caractères max.
 * 
 * Le joueur doit faire la combinaison "ctrl + entrée" afin d'activer l'interface de cheat. Il devra faire "Entrée" pour valider le cheat.
 * 
 * 
 * */

using UnityEngine;

public class Cheats : MonoBehaviour
{
    // TODO Use upper char too ? -> GoLevelUpNoob instead of golevelupnoob

    public const int MaxCharByCheat = 20;

    // Const containing string representing cheat
    public const string LevelUpCheat = "go level up noob"; // Give one level
    public const string GetItemCheat = "pls give item "; // Space at end is very important, because after this space, player put number to get the linked item ID.
    public const string GetAndValideQuest = "pls do quest "; // To get and valid a quest via its ID
    public const string GetMoneyCheat = "give me poney"; // Give 1000 pieces
    public const string GodMode = "i am the matrix"; // Player unvulnerable
    public const string PowerRangerStyle = "i am force red !"; // Player reach level 10 and get rare items level 10
    public const string FinishTheGame = "ok i am done$"; // End the game

    public static Cheats instance;

    string playerInput;

    UI_Cheats cheatsUI;

    bool cheatMode = false;
    public bool PlayerIsInCheatMode()
    {
        return cheatMode;
    }

    bool controlKeyPressed = false;

    public bool godModeActived = false; // To know in Player_Health if player can take damage or not.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerInput = "";

        cheatsUI = FindObjectOfType<UI_Cheats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            controlKeyPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            controlKeyPressed = false;
        }

        if (controlKeyPressed)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!cheatMode)
                {
                    Debug.Log("Enter in cheat mode");
                    cheatMode = true;

                    if (cheatsUI)
                    {
                        cheatsUI.UI.SetActive(true);
                    }
                    else
                    {
                        if (FindObjectOfType<UI_Cheats>())
                        {
                            cheatsUI = FindObjectOfType<UI_Cheats>();
                            cheatsUI.UI.SetActive(true);
                        }
                        else
                        {
                            Debug.Log("No cheats UI to display the cheat.");
                        }
                    }
                    return;
                }
            }
        }

        if (!cheatMode)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Exit cheat mode");
            cheatMode = false;

            if (cheatsUI)
            {
                cheatsUI.UI.SetActive(false);
            }

            CheckIfCheatIsComplete(playerInput);

            return;
        }

        // We must use Input.inputString for some character else unity cant detect that input.
        if (Input.inputString == "!")
        {
            UpdatePlayerInputs('!');
        }
        if (Input.inputString == "?")
        {
            UpdatePlayerInputs('?');
        }
        if (Input.inputString == "$")
        {
            UpdatePlayerInputs('$');
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdatePlayerInputs('a');
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            UpdatePlayerInputs('b');
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdatePlayerInputs('c');
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdatePlayerInputs('d');
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdatePlayerInputs('e');
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            UpdatePlayerInputs('f');
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            UpdatePlayerInputs('g');
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            UpdatePlayerInputs('h');
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdatePlayerInputs('i');
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UpdatePlayerInputs('j');
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdatePlayerInputs('k');
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdatePlayerInputs('l');
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UpdatePlayerInputs('m');
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            UpdatePlayerInputs('n');
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            UpdatePlayerInputs('o');
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdatePlayerInputs('p');
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UpdatePlayerInputs('q');
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdatePlayerInputs('r');
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdatePlayerInputs('s');
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            UpdatePlayerInputs('t');
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdatePlayerInputs('u');
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdatePlayerInputs('v');
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UpdatePlayerInputs('w');
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdatePlayerInputs('x');
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            UpdatePlayerInputs('y');
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpdatePlayerInputs('z');
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdatePlayerInputs('1');
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdatePlayerInputs('2');
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdatePlayerInputs('3');
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdatePlayerInputs('4');
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpdatePlayerInputs('5');
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UpdatePlayerInputs('6');
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UpdatePlayerInputs('7');
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            UpdatePlayerInputs('8');
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            UpdatePlayerInputs('9');
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UpdatePlayerInputs('0');
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdatePlayerInputs(' ');
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) // To delete last char
        {
            UpdatePlayerInputs('@');
        }
    }

    // Method to add a char to playerInputs string
    void UpdatePlayerInputs(char newChar)
    {
        if (newChar == '@')
        {
            if (playerInput.Length > 0)
            {
                string tempString = "";

                for (int i = 0; i < playerInput.Length - 1; i++)
                {
                    tempString += playerInput[i];
                }

                playerInput = tempString;
            }

            cheatsUI.UpdateCheatText(playerInput);

            return;
        }

        if (playerInput.Length < MaxCharByCheat)
        {
            playerInput += newChar;
        }

        cheatsUI.UpdateCheatText(playerInput);
    }

    // Method to check if a cheat is complete
    void CheckIfCheatIsComplete(string cheatCode)
    {
        // Cheat to reach lvl 25
        if (cheatCode == FinishTheGame)
        {
            for (int i = 0; i < 10; i++)
            {
                Player_Inventory.instance.GetNewItem(Player_Inventory.instance.itemDataBase.GetItemById(1004));
                Player_Inventory.instance.GetNewItem(Player_Inventory.instance.itemDataBase.GetItemById(1005));
            }

            while (Player_Stats.instance.GetCurrentLevel() < 25)
            {
                Player_Stats.instance.CheatLevelUp();
            }

            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Helm, 504);
            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Boots, 505);
            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Gloves, 506);
            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Pants, 507);
            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Chest, 508);
            Player_Inventory.instance.SetArmoryIndex((int)ArmoryPart.Bow, 509);

            UI_Player.instance.playerInventoryUI.RefreshArmory();
            Player_Stats.instance.RefreshPlayerStats();

            for (int i = 0; i < UI_Teleporter.teleporterNumber; i++)
            {
                UI_Player.instance.teleporterUI.SetUnlockedTp(i, true);
            }

            for (int i = 0; i < Quests_Control.instance.questDataBase.quests.Length; i++)
            {
                if (Quests_Control.instance.GetQuestAchievement(i) == false)
                {
                    Quests_Control.instance.SetQuestAchievement(i, true);
                }
            }
        }

        // Cheat to give level up to the player
        if (cheatCode == LevelUpCheat)
        {
            Player_Stats.instance.CheatLevelUp();
        }

        // Cheat to give money to the player
        if (cheatCode == GetMoneyCheat)
        {
            Player_Inventory.instance.SetPlayerGold(1000);

            if (Sound_Manager.instance)
            {
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.sell);
            }
        }

        if (cheatCode == PowerRangerStyle)
        {
            for (int i = 0; i < 10; i++)
            {
                Player_Inventory.instance.GetNewItem(Player_Inventory.instance.itemDataBase.GetItemById(1004));
                Player_Inventory.instance.GetNewItem(Player_Inventory.instance.itemDataBase.GetItemById(1005));
            }

            while (Player_Stats.instance.GetCurrentLevel() < 10)
            {
                Player_Stats.instance.CheatLevelUp();
            }

            Player_Inventory.instance.SetArmoryIndex(0, 236);
            Player_Inventory.instance.SetArmoryIndex(1, 237);
            Player_Inventory.instance.SetArmoryIndex(2, 238);
            Player_Inventory.instance.SetArmoryIndex(3, 239);
            Player_Inventory.instance.SetArmoryIndex(4, 240);
            Player_Inventory.instance.SetArmoryIndex(5, 241);

            UI_Player.instance.playerInventoryUI.RefreshArmory();
            Player_Stats.instance.RefreshPlayerStats();
        }

        // Cheat to enter player into GodMode (take no damage)
        if (cheatCode == GodMode)
        {
            if (!godModeActived)
            {
                godModeActived = true;
                Debug.Log("god mode actived");
            }
            else
            {
                godModeActived = false;
                Debug.Log("god mode unactived");
            }
        }

        // Cheat to give item to the player.
        if (cheatCode.Contains(GetItemCheat)) // If the chain contains "pls give item " we must check if we can convert next chars into int to get the linked ID of an item
        {
            int _itemID = GetIDFromPlayerInput();

            if (_itemID == -1)
                return;

            // Check if itemID is linked to an item in the database.
            CheckForItemInDatabase(_itemID);
        }

        // Cheat to get and validate a quest by its ID
        if (cheatCode.Contains(GetAndValideQuest)) // Same concept as the item ID above
        {
            int _questID = GetIDFromPlayerInput();

            if (_questID == -1)
                return;

            // Check if itemID is linked to an item in the database.
            CheckForQuestInDataBase(_questID);
        }

        // After testing all combinaisons, reset playerInputs.
        playerInput = "";
        cheatsUI.UpdateCheatText(playerInput);
    }

    // Method to cheat drop an item via its ID
    void CheckForItemInDatabase(int _itemID)
    {
        if (Player_Inventory.instance)
        {
            if (Player_Inventory.instance.itemDataBase.GetItemById(_itemID) != null)
            {
                // Get the item
                BaseItem itemToCheatDrop = Player_Inventory.instance.itemDataBase.GetItemById(_itemID);

                // Get the parent of all items gameobjects
                Transform parentGameObject = null;

                if (GameObject.Find("Items"))
                {
                    parentGameObject = GameObject.Find("Items").transform;
                }

                // Get a position to spawn item
                Vector3 dropPose = new Vector3(Random.Range(Player_Inventory.instance.transform.position.x - 1f, Player_Inventory.instance.transform.position.x + 1f),
                                               Random.Range(Player_Inventory.instance.transform.position.y - 1f, Player_Inventory.instance.transform.position.y + 1f),
                                               0f);

                // Instantiate the item
                GameObject droppedGO = Instantiate(itemToCheatDrop.itemPrefab, dropPose, Quaternion.identity);

                droppedGO.GetComponent<SpriteRenderer>().sprite = itemToCheatDrop.prefabImage;
                droppedGO.GetComponent<Item>().itemConfig = itemToCheatDrop;

                // Parent it to clean up hierarchy
                if (parentGameObject)
                    droppedGO.transform.parent = parentGameObject;
            }
            else
            {
                Debug.Log("No itemID " + _itemID + " found in the database.");
            }
        }
    }

    // Method to Check if there is a quest linked to an ID. If yes player get and instant validate the quest.
    void CheckForQuestInDataBase(int _questID)
    {
        if (Quests_Control.instance)
        {
            if (Quests_Control.instance.questDataBase.GetQuestByID(_questID))
            {
                if (!Quests_Control.instance.questDataBase.GetQuestByID(_questID).questDone)
                {
                    if (!Quests_Control.instance.GetPlayerQuestByID(_questID))
                    {
                        Quests_Control.instance.GetNewQuest(_questID);
                    }

                    Quests_Control.instance.GetPlayerQuestByID(_questID).currentQuestObjective = Quests_Control.instance.questDataBase.GetQuestByID(_questID).totalQuestObjective;

                    Quests_Control.instance.ValideQuest(_questID, Player_Stats.instance.transform.position);
                }
            }
            else
            {
                Debug.Log("No questID " + _questID + " found in the database.");
            }
        }

    }

    // Method to get an ID(int) from last chars of playerInput
    int GetIDFromPlayerInput()
    {
        // the item's ID must be type after the last " " of playerInput. So we need to get that into a int variable.
        // To proceed, we must know the length of the number (to know if its ID 32 or ID 323 for instance)
        // So we start by unit, then ten, then hundred. When we reach ' ' char, we know it was the last number.

        char tempChar = 'a'; // No matter by what char is initialized, we just must put one.
        int currentPlayerInputIndex = playerInput.Length; // Initialize to the length of the string (it'll be decremented before using)
        int multiplyNumb = 1; // Start to 1, then 10, then 100, then 1000. It'll multiply each time the converted char to get the expected ID number
        int _ID = -1;

        while (true) // We're sure we can go out of this infinite loop because the required string to enter here contains space, and we go out when we meet the first space in the string.
        {
            currentPlayerInputIndex--;
            tempChar = playerInput[currentPlayerInputIndex];

            if (tempChar == ' ')
                break;

            int tempNumb = ConvertCharToInt(tempChar);
            if (tempNumb == -1)
            {
                // The char does not represent a number. Return -1.

                // reset playerInputs.
                playerInput = "";
                cheatsUI.UpdateCheatText(playerInput);

                return -1;
            }

            tempNumb *= multiplyNumb;
            multiplyNumb *= 10;

            _ID += tempNumb;
        }

        // If we reached here, we got a correct _ID, but we must add one to it (because intialized to -1)
        if (_ID >= 0)
            _ID++;
        else // Particular case if player want the _ID 0, it'll still -1. So we must set it to 0.
            _ID = 0;

        return _ID;
    }

    // Method to convert a char into a int
    int ConvertCharToInt(char toConvert)
    {
        switch (toConvert)
        {
            case '1':
                return 1;
            case '2':
                return 2;
            case '3':
                return 3;
            case '4':
                return 4;
            case '5':
                return 5;
            case '6':
                return 6;
            case '7':
                return 7;
            case '8':
                return 8;
            case '9':
                return 9;
            case '0':
                return 0;
            default:
                // Debug.Log("Char non reconnu pour être converti en int.");
                return -1;
        }
    }
}
