using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class VirtualPad_Actions : MonoBehaviour
{
    public const int virtualAbilitiesNumb = 5; // The number of abilities buttons
    public const int virtualPotionsNumb = 2; // The number of potions buttons

    [SerializeField] VirtualPad_Ability_Button primaryAbilityButton;
    [SerializeField] VirtualPad_Ability_Button secondaryAbilityButton;
    [SerializeField] VirtualPad_Ability_Button thirdAbilityButton;
    [SerializeField] VirtualPad_Ability_Button fourthAbilityButton;
    [SerializeField] VirtualPad_Ability_Button fifthAbilityButton;

    [SerializeField] VirtualPad_Potion_Button primaryPotionButton;
    [SerializeField] VirtualPad_Potion_Button secondaryPotionButton;

    VirtualPad_Ability_Button currentAbilityButton;
    VirtualPad_Potion_Button currentPotionButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UseAbility(Ability_Config _ability)
    {
        
    }

    void UsePotion(UsableItem _potion)
    {

    }

    // Method to know when mouse is over an action button
    // and set the current ability / potion button
    bool IsMouseOverMoveTrigger()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            // If player has the mouse on an ability button
            if (raycastResultList[i].gameObject.GetComponent<VirtualPad_Ability_Button>())
            {
                if (currentPotionButton != null)
                    currentPotionButton = null;

                currentAbilityButton = raycastResultList[i].gameObject.GetComponent<VirtualPad_Ability_Button>();

                return true;
            }
            else if (raycastResultList[i].gameObject.GetComponent<VirtualPad_Potion_Button>())
            {
                if (currentAbilityButton != null)
                    currentAbilityButton = null;

                currentPotionButton = raycastResultList[i].gameObject.GetComponent<VirtualPad_Potion_Button>();

                return true;
            }
        }

        if (currentAbilityButton != null)
            currentAbilityButton = null;

        if (currentPotionButton != null)
            currentPotionButton = null;

        return false;
    }

    public void SetAbility(int _index, Ability_Config _newAbility)
    {

    }

    public void SetPotion(int _index, UsableItem _newPotion)
    {

    }

    public Ability_Config GetAbilityBind(int _index)
    {
        return null;
    }

    public UsableItem GetPotionBind(int _index)
    {
        return null;
    }
}
