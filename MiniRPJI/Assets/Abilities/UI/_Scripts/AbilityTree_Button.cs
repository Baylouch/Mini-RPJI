/* AbilityTree_Button.cs
 * 
 * A attacher sur chaque boutons représentant les abilités dans l'arbre d'abilités.
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class AbilityTree_Button : MonoBehaviour
{
    public Ability_Config config;

    [SerializeField] Image abilityIconImage;

    Button buttonComponent;

    private void Start()
    {
        buttonComponent = GetComponent<Button>();

        RefreshButton();
    }

    private void OnEnable()
    {
        if (buttonComponent != null)
            RefreshButton();
    }

    public void RefreshButton()
    {
        if (buttonComponent == null)
            return;

        if (Player_Stats.instance)
        {
            if (config.levelRequired > Player_Stats.instance.GetCurrentLevel())
            {
                // Button greyed and player can't interact with
                abilityIconImage.color = Color.grey;
                buttonComponent.interactable = false;
            }
            else
            {
                // Player has the level required to interact with this button
                abilityIconImage.color = Color.white;
                buttonComponent.interactable = true;
            }
        }
    }
}
