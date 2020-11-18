using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasCredit : MonoBehaviour
{
    [SerializeField] GameObject panelUI;

    [SerializeField] Button backToTownButton;
    [SerializeField] Button skipButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Scenes_Control.instance)
        {
            backToTownButton.onClick.AddListener(() => Scenes_Control.instance.SwitchPlayerToTownFromCredits());
            backToTownButton.onClick.AddListener(() => panelUI.SetActive(false));
            backToTownButton.onClick.AddListener(() => skipButton.gameObject.SetActive(false));

            skipButton.onClick.AddListener(() => Scenes_Control.instance.SwitchPlayerToTownFromCredits());
            skipButton.onClick.AddListener(() => panelUI.SetActive(false));
            skipButton.onClick.AddListener(() => skipButton.gameObject.SetActive(false));
        }

        StartCoroutine(OpenCreditsMenu());

        if (Sound_Manager.instance)
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.fireWorks);
    }

    IEnumerator OpenCreditsMenu()
    {
        yield return new WaitForSeconds(40f);

        panelUI.SetActive(true);
    }
}
