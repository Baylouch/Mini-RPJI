/* Menu_Controller.cs
 * Utilisé dans Level_Menu. Permet d'interagir avec les boutons du menu.
 * 
 * */

using UnityEngine;

public class Menu_Controller : MonoBehaviour
{

    // Security for if player back to the menu from the game
    private void Update()
    {
        if (Player_Stats.stats_instance)
            Destroy(Player_Stats.stats_instance.gameObject);

        if (UI_Player.ui_instance)
            Destroy(UI_Player.ui_instance.gameObject);
    }

    public void StartGame()
    {
        if (Level_Controller.instance)
        {
            Level_Controller.instance.ChangeLevel("Level_1");
        }
    }

    public void LoadGame()
    {
        if (Level_Controller.instance)
        {
            Level_Controller.instance.ChangeLevel("Level_Load");
        }
    }

    public void QuitGame()
    {
        if (Level_Controller.instance)
        {
            Application.Quit();
        }
    }
}
