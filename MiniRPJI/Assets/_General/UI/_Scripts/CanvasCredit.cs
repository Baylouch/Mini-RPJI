using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasCredit : MonoBehaviour
{
    [SerializeField] GameObject panelUI;

    [SerializeField] Button continueButton;
    [SerializeField] Button saveAndQuitButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Scenes_Control.instance)
        {
            continueButton.onClick.AddListener(() => Scenes_Control.instance.SwitchPlayerToTownFromCredits());
            continueButton.onClick.AddListener(() => panelUI.SetActive(false));
        }

        StartCoroutine(OpenCreditsMenu());
    }

    IEnumerator OpenCreditsMenu()
    {
        yield return new WaitForSeconds(35f);

        panelUI.SetActive(true);
    }
}
