using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Ability_Button : MonoBehaviour
{
    public int abilityID; // ID of ability link

    public Image abilityImage; // A child gameobject with Image component on it.
}
