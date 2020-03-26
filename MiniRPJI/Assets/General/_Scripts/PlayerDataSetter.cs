/* PlayerDataSetter.cs 
 * Sert a être instancié dans la scene "Level_Load", étant persistant, permet de retenir
 * la sauvegarde a charger une fois "Level_1" chargé. Est directement détruit une fois les données configurées.
 * 
 * 
 * */

using UnityEngine;

public class PlayerDataSetter : MonoBehaviour
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

        if (Level_Controller.instance)
        {
            if (Level_Controller.instance.GetCurrentSceneBuildIndex() > 2) // If we're in a playable level.
            {
                // Check if you find all player's components to set data.
                if (Player_Stats.stats_instance)
                {
                    if (Player_Inventory.inventory_instance)
                    {
                        if (Player_Quest_Control.quest_instance)
                        {
                            // Now set data
                            if (GameDataControl.dataControl_instance)
                            {
                                if (GameDataControl.dataControl_instance.GetLoadData(dataToSet) != null)
                                {
                                    GameDataControl.dataControl_instance.LoadPlayerData(dataToSet);

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
