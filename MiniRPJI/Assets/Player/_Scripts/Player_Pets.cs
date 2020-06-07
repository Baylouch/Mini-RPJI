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

    [SerializeField] PetConfig[] playerPets = new PetConfig[20]; // Linked to playerPetsLength const.

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

        if (UI_Player.instance && UI_Player.instance.playerPetsUI)
        {
            UI_Player.instance.playerPetsUI.SetPetButtonListener(petToInvoke);
        }
    }

    // Method to return the current player's pet
    public void ReturnPet()
    {
        if (currentPlayerPet != null)
        {
            Destroy(currentPlayerPetGO);

            PetConfig tempPet = currentPlayerPet;

            currentPlayerPetGO = null;
            currentPlayerPet = null;

            if (UI_Player.instance && UI_Player.instance.playerPetsUI)
            {
                UI_Player.instance.playerPetsUI.SetPetButtonListener(tempPet);
            }
        }
    }
}
