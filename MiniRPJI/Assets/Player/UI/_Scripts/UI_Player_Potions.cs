/* UI_Player_Potions.cs :
 * 
 * Permet au joueur de choisir quelles potions il souhaite mettre en raccourci (A ou R).
 * 
 * Contient les input permettant au joueur d'utiliser rapidement les potions définies.
 * 
 * Lorsque le joueur clic sur un des boutons lié, une petite UI s'affiche lui proposant les différentes potions contenues dans son inventaire.
 * Il lui suffit de cliquer sur la potion qu'il veut afin qu'elle soit utilisée quand le joueur utilisera le raccourci.
 * 
 * J'ai décidé pour ne pas partir de rien et de copier coller le script UI_Player_Abilities, que j'ai adapté pour les potions.
 * 
 * */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Player_Potions : MonoBehaviour
{
    public static UI_Player_Potions instance; // To be acces in UI_Player_Abilities to unshow potions panel

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public UsableItem firstPotion;
    public UsableItem secondPotion;

    // To change image relatively of the current potion using.
    public Image potionImage_1;
    public Image potionImage_2;

    [SerializeField] RectTransform PotionsPanel; // The RectTransform who'll be parent of potion's buttons
    // PotionsPanel need to upgrade its width by 30 for each ability button instantiate.

    // potionButtonPrefab are instantiate for each ability's available.
    // /!\ Must have a Ability_Button script on it. /!\ TODO See if still the case with potion ones.
    [SerializeField] GameObject potionButtonPrefab; // Spawn as a child of PotionsPanel, the first is spawn with 15 on x pos, then for all next buttons, increment x pos by 30.

    // Ability description variables
    [SerializeField] GameObject potionDescriptionPanel; // The panel who contain ability's name, description and cost

    GameObject currentDescriptionPanel; // To register an instantiate gameobject and manipulate it.

    Potion_Button currentPotionDescription; // To know the current potion description

    bool displayingPotions = false; // To know when player is displaying potions.

    // Start is called before the first frame update
    void Start()
    {
        ResetPotionsPanel();
    }

    private void Update()
    {
        IsMouseOverPotionButton();

        if (Player_Shortcuts.GetShortCuts() == 0 || Player_Shortcuts.GetShortCuts() == 2) // Same for ZQSD and arrows 
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (firstPotion != null)
                {
                    FastPotionUse(firstPotion);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (firstPotion != null)
                {
                    FastPotionUse(firstPotion);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (secondPotion != null)
            {
                FastPotionUse(secondPotion);
            }
        }

        ResetPotionsImages();
    }

    void ResetPotionsImages()
    {
        if (firstPotion == null)
        {
            potionImage_1.enabled = false;
        }

        if (secondPotion == null)
        {
            potionImage_2.enabled = false;
        }
    }

    // Method to reset potions panel
    public void ResetPotionsPanel()
    {
        // Check if there is children (potions buttons), if there are destroy them.
        if (PotionsPanel.childCount > 0)
        {
            for (int i = 0; i < PotionsPanel.childCount; i++)
            {
                Destroy(PotionsPanel.GetChild(i).gameObject);
            }
        }

        // Reset potionsPanel width (x = 0, y = 33).
        PotionsPanel.sizeDelta = new Vector2(0f, 33f); // Height is always 33f.

        if (PotionsPanel.gameObject.activeSelf)
        {
            PotionsPanel.gameObject.SetActive(false);
        }

        displayingPotions = false;
    }

    // Method to know when mouse is over Potion button to display its description
    void IsMouseOverPotionButton()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Potion_Button>())
            {
                if (currentPotionDescription != raycastResultList[i].gameObject.GetComponent<Potion_Button>())
                {
                    // Display ability's description
                    currentPotionDescription = raycastResultList[i].gameObject.GetComponent<Potion_Button>();

                    DisplayPotionDescription(currentPotionDescription.potionID);
                }
                return;
            }
        }

        // If we're here mouse isn't over Ability_Button so destroy abilityDescriptionPanel if there is one
        if (currentDescriptionPanel)
        {
            Destroy(currentDescriptionPanel);
            currentPotionDescription = null;
        }
    }

    void DisplayPotionDescription(int _potionID)
    {
        BaseItem potionToDisplay = Player_Inventory.instance.itemDataBase.GetItemById(_potionID);

        if (potionToDisplay != null)
        {
            if (currentDescriptionPanel == null)
            {
                currentDescriptionPanel = Instantiate(potionDescriptionPanel);

                currentDescriptionPanel.transform.parent = PotionsPanel.transform;
                currentDescriptionPanel.transform.localScale = Vector3.one;
                currentDescriptionPanel.transform.localPosition = new Vector2(0f, 58f);

                // We know the first child of potionDescriptionPanel is the name
                // second child is the description

                currentDescriptionPanel.transform.GetChild(0).GetComponent<Text>().text = potionToDisplay.itemName;
                currentDescriptionPanel.transform.GetChild(1).GetComponent<Text>().text = potionToDisplay.itemDescription;


            }
            else
            {
                currentDescriptionPanel.transform.GetChild(0).GetComponent<Text>().text = potionToDisplay.itemName;
                currentDescriptionPanel.transform.GetChild(1).GetComponent<Text>().text = potionToDisplay.itemDescription;
            }
        }
        else
        {
            Debug.Log("Wrong _potionID in DisplayPotionDescription() from UI_Player_Potions.");
        }
    }

    // Method to display potions available when player clic on the first or second potion's UI.
    // indexPotion means : (0) first input (A), (1) second input (R).
    public void DisplayPotionsAvailable(int indexPotion)
    {
        if (UI_Player_Abilities.instance)
        {
            UI_Player_Abilities.instance.ResetAbilitiesPanel();
        }

        if (!displayingPotions)
        {
            displayingPotions = true;

            // We need to check if we have acces to player's inventory
            if (Player_Inventory.instance)
            {
                // Now get available potions
                // for this we first create a an array containing all potions available in player inventory. For now no matter what's displayed order.
                int potionsArraySize = 0;

                for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
                {
                    if (Player_Inventory.instance.GetInventoryItem(i) as UsableItem) // For now usable items are only potions in the game.
                    {
                        potionsArraySize++;
                    }                    
                }

                // Then we can create a UsableItem array with size of potionsArraySize and put it with their ID in

                UsableItem[] potionsAvailable = new UsableItem[potionsArraySize];
                int potionsAvailableIndex = 0;

                for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++)
                {
                    if (Player_Inventory.instance.GetInventoryItem(i) as UsableItem) // For now usable items are only potions in the game.
                    {
                        potionsAvailable[potionsAvailableIndex] = Player_Inventory.instance.GetInventoryItem(i) as UsableItem;
                        potionsAvailableIndex++;
                    }
                }

                // Now we got our array containing all potions available. (potionsAvailable[])
                // We can start displaying UI and set interactions.

                // First display PotionsPanel
                if (!PotionsPanel.gameObject.activeSelf)
                {
                    PotionsPanel.gameObject.SetActive(true);
                }

                // Now for each potions available, increase width of PotionsPanel by 30f, instantiate an potion button then set it.
                for (int i = 0; i < potionsAvailable.Length; i++)
                {
                    // /!\ Because of UNITY issue, we must create a new int to store value of i. If not, onClick,AddListener will not WORK /!\
                    int x = i;

                    PotionsPanel.sizeDelta = new Vector2(PotionsPanel.sizeDelta.x + 30f, 33f);

                    // Instantiate and set Parent
                    GameObject currentPotionGO = Instantiate(potionButtonPrefab);
                    currentPotionGO.transform.parent = PotionsPanel.transform;
                    // And reset localScale
                    currentPotionGO.transform.localScale = Vector3.one;

                    // Now set the currentAbilityButton
                    Potion_Button currentPotionButton = currentPotionGO.GetComponent<Potion_Button>();

                    // Set the onClick
                    currentPotionButton.GetComponent<Button>().onClick.AddListener(() => ChangePotion(potionsAvailable[x].itemID, indexPotion));

                    // Set the potions's image
                    currentPotionButton.potionImage.sprite = potionsAvailable[i].inventoryImage;

                    // Set the potion's ID
                    currentPotionButton.potionID = potionsAvailable[i].itemID;
                }
            }
        }
        else
        {
            ResetPotionsPanel();
        }
    }

    // Method used on each Potion_Button from theirs onClick event to change potion 1 input or potion 2 input (indexPotion same of the DisplayPotionsAvailable method)
    public void ChangePotion(int newPotionID, int indexPotion)
    {
        BaseItem newPotion = Player_Inventory.instance.itemDataBase.GetItemById(newPotionID);

        if (indexPotion == 0) // if its the first potion
        {
            potionImage_1.sprite = newPotion.inventoryImage;

            if (!potionImage_1.enabled)
                potionImage_1.enabled = true;

            // Set new potion on the input
            firstPotion = newPotion as UsableItem;

        }
        else if (indexPotion == 1) // if its the second potion
        {
            potionImage_2.sprite = newPotion.inventoryImage;

            if (!potionImage_2.enabled)
                potionImage_2.enabled = true;

            // Set new potion on the input
            secondPotion = newPotion as UsableItem;

        }
        else
        {
            // There is an error
            Debug.Log("ChangePotion indexPotion from UI_Player_Potions isnt right. Please fix.");
        }

        ResetPotionsPanel();
    }

    void UsePotion(UsableItem item)
    {
        if (item == null) // I saw only one time an error when player used its last potion leading me here with a null reference. I suppose this will fix it.
            return;

        if (!item.CanUse())
            return;

        item.Use();

        for (int i = 0; i < Player_Inventory.inventorySlotsNumb; i++) // Loop over inventory slots to find the item used to decrease the right one.
        {
            // Goal with line 2 next lines is to avoid null reference when player use fast potion when there is no more potions available.
            if (Player_Inventory.instance.GetInventoryItem(i) == null)
                continue;

            if (Player_Inventory.instance.GetInventoryItem(i).itemID == item.itemID) // We found it.
            {
                Player_Inventory.instance.inventoryItemsNumb[i]--;

                if (Player_Inventory.instance.inventoryItemsNumb[i] <= 0)
                {
                    Player_Inventory.instance.SetInventoryIndex(i, -1);

                    if (item == firstPotion)
                    {
                        firstPotion = null;
                    }

                    if (item == secondPotion)
                    {
                        secondPotion = null;
                    }

                }

                if (UI_Player.instance.playerInventoryUI)
                {
                    UI_Player.instance.playerInventoryUI.RefreshInventory();
                }

                return;
            }
        }
    }

    public void FastPotionUse(UsableItem potionToUse)
    {
        UsePotion(potionToUse);
    }
}

