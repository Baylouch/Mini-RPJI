/* Cheats.cs
 * 
 * Contient tout les cheats du jeu.
 * 
 * 
 * */

using UnityEngine;

public class Cheats : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (Player_Stats.instance)
                {
                    Player_Stats.instance.CheatLevelUp();
                }
            }
        }
    }
}
