/* PlayerDataSetter.cs 
 * Sert a être instancié dans la scene "Level_Load", étant persistant, permet de retenir
 * la sauvegarde a charger une fois "Level_1" chargé. Est directement détruit une fois les données configurées.
 * 
 * 
 * */

using UnityEngine;

public class Player_Data_Setter : MonoBehaviour
{
    public int dataToSet;

    public bool canSet = false; // If player want load game when he's already in a game, we need to put this true when OnLevelWasLoad to wait new scene loading.

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        canSet = true;
    }

    private void Update()
    {
        if (!canSet)
            return;

        if (Scenes_Control.instance)
        {
            if (Scenes_Control.instance.GetCurrentSceneBuildIndex() >= 3) // If we're in a playable level.
            {
                // Check if you find all player's components to set data.
                if (Player_Stats.instance)
                {
                    if (Player_Inventory.instance)
                    {
                        if (Player_Quest_Control.instance)
                        {
                            // Now set data
                            if (Game_Data_Control.data_instance)
                            {
                                if (Game_Data_Control.data_instance.GetLoadData(dataToSet) != null)
                                {
                                    Game_Data_Control.data_instance.LoadPlayerData(dataToSet);

                                    Destroy(gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
