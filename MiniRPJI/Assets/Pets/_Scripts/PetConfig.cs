/* PetConfig.cs
 * 
 * Contient les données d'un animal de compagnie : Nom, description, prix, vitesse, categorie, prefab
 * 
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "PetConfig", menuName = "ScriptableObjects/Pets/PetConfig", order = 1)]
public class PetConfig : ScriptableObject
{
    public int petID;

    public string petName;

    [TextArea] public string petDescription;

    public PetCategory petCategory;

    public PetSex petSex;

    public Sprite petSprite;

    public float petWeight; // kg

    public GameObject petPrefab;

    public int petPrice;
}
