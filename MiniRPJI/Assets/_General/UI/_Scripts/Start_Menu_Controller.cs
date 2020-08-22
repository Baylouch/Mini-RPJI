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

    // When player hit Start button
    public void StartGame()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.LoadTownLevel();
        }
    }

    // When player hit Load button
    public void LoadMenu()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.ChangeLevel("Load_Menu");
        }
    }

    // When player hit options button
    public void OptionsMenu()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.ChangeLevel("Options_Menu");
        }
    }

    // When player hit quit button
    public void QuitGame()
    {
        Application.Quit();       
    }
}
