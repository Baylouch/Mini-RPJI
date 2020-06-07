using UnityEngine;

[CreateAssetMenu(fileName = "PetsDataBase", menuName = "ScriptableObjects/Pets/DataBase", order = 0)]
public class PetsDataBase : ScriptableObject
{
    public PetConfig[] pets;

    public PetConfig GetPetByID(int _petID)
    {
        for (int i = 0; i < pets.Length; i++)
        {
            if (pets[i] != null)
            {
                if (pets[i].petID == _petID)
                    return pets[i];
            }
        }

        return null;
    }
}
