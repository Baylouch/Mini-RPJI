/* UI_Player_Informations.cs
 * 
 * Permet d'afficher un element UI à l'écran un court instant pour donner une indication au joueur.
 * 
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Informations : MonoBehaviour
{
    public static UI_Player_Informations instance;

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

    [SerializeField] GameObject informationsUI;

    GameObject currentInformationsUI;

    // Start is called before the first frame update
    void Start()
    {
        if (informationsUI.activeSelf)
            informationsUI.SetActive(false);
    }

    // Method to call in each methods where player need to be informed
    public void DisplayInformation(string textToDisplay)
    {
        if (currentInformationsUI != null)
            return;

        if (UI_Player.instance)
        {
            // Instantiate informationsUI
            currentInformationsUI = Instantiate(informationsUI, transform); // It'll be a direct child of UI_Player because this script is attach on it
            // Set it unactive
            currentInformationsUI.SetActive(false);

            // Set the text
            currentInformationsUI.GetComponentInChildren<Text>().text = textToDisplay;

            // Set it active
            currentInformationsUI.SetActive(true);

            // Destroy timer
            Destroy(currentInformationsUI, 1.5f);
        }
        else // Must never be reached.
        {
            Debug.Log("No UI_Player instance in the scene to display infos.");
        }
    }
}
