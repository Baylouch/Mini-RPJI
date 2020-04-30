/* Menu_Controller.cs
 * Utilisé dans Level_Menu. Permet d'interagir avec les boutons du menu.
 * 
 * */

using UnityEngine;

public class Start_Menu_Controller : MonoBehaviour
{

    // Security for if player back to the menu from the game
    private void Start()
    {
        if (Player_Stats.instance)
            Destroy(Player_Stats.instance.gameObject);

        if (UI_Player.instance)
            Destroy(UI_Player.instance.gameObject);
    }

    public void StartGame()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.LoadGameLevels();
        }
    }

    public void LoadMenu()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.ChangeLevel("Load_Menu");
        }
    }

    public void OptionsMenu()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.ChangeLevel("Options_Menu");
        }
    }

    public void QuitGame()
    {
        Application.Quit();       
    }
}
