/* Player_Pets.cs
 * 
 * To attach on a child of the Player gameobject, contains an array of player available's pets initialize manually by the total pets present in game.
 * 
 * 
 * */

using UnityEngine;

public enum PetCategory { Cat, Dog, Alien };
public enum PetSex { Female, Male, Undefined };

public class Player_Pets : MonoBehaviour
{
    public const int questIDToUnlockPets = 12; // The quest ID to unlock pets in game.

    public const int playerPetsLength = 20;

    public static Player_Pets instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public PetsDataBase petsDataBase;

    public PetConfig currentPlayerPet;

    [SerializeField] GameObject petInvokeEffect;
    [SerializeField] GameObject petReturnEffect;

    [SerializeField] PetConfig[] playerPets = new PetConfig[20]; // Linked to playerPetsLength const.

    private bool petsUnlocked = false;
    public void SetPetsUnlocked(bool value)
    {
        if (value == true)
            Player_Success.instance.IncrementSuccessObjectiveByID(7);

        petsUnlocked = value;
    }
    public bool GetPetsUnlocked()
    {
        return petsUnlocked;
    }

    GameObject currentPlayerPetGO;

    private void Start()
    {
        for (int i = 0; i < playerPets.Length; i++)
        {
            if (playerPets[i] != null)
            {
                UI_Player.instance.playerPetsUI.AddPetButton(playerPets[i]);
            }
        }
    }

    // To get a player pet by index
    public PetConfig GetPlayerPetByIndex(int index)
    {
        if (playerPets[index] != null)
            return playerPets[index];

        return null;
    }

    // To set a player pet to an index
    public void SetPlayerPets(PetConfig petConfig, int indexToSet)
    {
        playerPets[indexToSet] = petConfig;
    }

    // To get player pet by ID
    public PetConfig GetPlayerPetByID(int _petID)
    {
        for (int i = 0; i < playerPetsLength; i++)
        {
            if (playerPets[i] == null)
                break;

            if (playerPets[i].petID == _petID)
                return playerPets[i];
        }

        return null;
    }

    // Method to get a new pet
    public void GetNewPet(PetConfig newPet)
    {
        int petIndex = -1; // Get the first index available in playerPets

        for (int i = 0; i < playerPets.Length; i++)
        {
            if (playerPets[i] == null)
            {
                petIndex = i;
                break;
            }
        }

        // if petIndex still -1, we didnt found available index
        if (petIndex == -1)
        {
            Debug.Log("No petIndex available in playerPets.");
            return;
        }

        // Set new pet
        playerPets[petIndex] = newPet;

        if (UI_Player.instance.playerPetsUI)
            UI_Player.instance.playerPetsUI.AddPetButton(newPet);

    }

    // Method to invoke a pet
    public void InvokePet(PetConfig petToInvoke)
    {
        // Before invoke, we need to check if there is already a pet. We can just use ReturnPet() method.
        ReturnPet();

        Vector3 petPosition = new Vector3(Random.Range(transform.position.x - 3f, transform.position.x + 3f), 
                                          Random.Range(transform.position.y - 3f, transform.position.y + 3f), 
                                          0f);

        currentPlayerPetGO = Instantiate(petToInvoke.petPrefab, petPosition, petToInvoke.petPrefab.transform.rotation);
        currentPlayerPet = petToInvoke;

        if (UI_Player.instance.playerPetsUI)
        {
            UI_Player.instance.playerPetsUI.SetPetButtonListener(petToInvoke);
        }

        if (petInvokeEffect)
        {
            GameObject effect = Instantiate(petInvokeEffect, petPosition, Quaternion.identity);

            if (GameObject.Find("Effects"))
            {
                GameObject effectHierarchy = GameObject.Find("Effects");
            }

            Destroy(effect, 1f);
        }

        if (Sound_Manager.instance)
        {
            switch (petToInvoke.petCategory)
            {
                case PetCategory.Cat:
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.catInvoke);
                    break;
                case PetCategory.Dog:
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.dogInvoke);
                    break;
                case PetCategory.Alien:
                    // TODO Create a sound for the alien
                    break;
            }
            
        }
    }

    // Method to return the current player's pet
    public void ReturnPet()
    {
        if (currentPlayerPet != null)
        {
            Vector3 petPosition = currentPlayerPetGO.transform.position;

            Destroy(currentPlayerPetGO);

            PetConfig tempPet = currentPlayerPet;

            currentPlayerPetGO = null;
            currentPlayerPet = null;

            if (UI_Player.instance.playerPetsUI)
            {
                UI_Player.instance.playerPetsUI.SetPetButtonListener(tempPet);
            }

            if (petReturnEffect)
            {
                GameObject effect = Instantiate(petReturnEffect, petPosition, Quaternion.identity);

                if (GameObject.Find("Effects"))
                {
                    GameObject effectHierarchy = GameObject.Find("Effects");
                }

                Destroy(effect, 1f);
            }
        }
    }
}
