/* TransitionInformationSetter.cs
 * 
 * Permet de faire le lien entre la données TransitionInformation et la scene de transition.
 * 
 * Permet d'afficher une information aléatoire.
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class TransitionInformationSetter : MonoBehaviour
{
    [SerializeField] TransitionInformation_Config[] infoConfigs;

    [SerializeField] GameObject infoPanel;

    [SerializeField] Image infoImage;
    [SerializeField] Text infoText;

    public void DisplayRandomInfoConfig()
    {
        int index = Random.Range(0, infoConfigs.Length);

        infoImage.sprite = infoConfigs[index].informationSprite;
        infoText.text = infoConfigs[index].informationText;
    }
}
