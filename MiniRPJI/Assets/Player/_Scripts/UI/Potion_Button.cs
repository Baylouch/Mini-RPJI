using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Potion_Button : MonoBehaviour
{
    public int potionID; // ID of the potion link

    public Image potionImage; // A child gameobject with Image component on it.
}
