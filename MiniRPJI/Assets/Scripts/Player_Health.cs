/* Health.cs
Utilisé pour gérer la vie du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Stats))]
public class Player_Health : MonoBehaviour
{
    Player_Stats playerStats;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField]SpriteRenderer rend; 

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<Player_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // A FadeIn and FadeOut coroutine methods to apply on each sprite who doesnt have "TakeDamage" animation
    IEnumerator FadeIn() // Reapear
    {
        StopCoroutine("FadeOut");// Make end of FadeOut Coroutine

        for (float f = 0.05f; f <= 1; f+=0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;

            yield return new WaitForSeconds(0.01f);
        }
       
        StopCoroutine("FadeIn"); // Make end of this coroutine
    }

    IEnumerator FadeOut() // Dissapear
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine("FadeIn"); // And now reappear
    }

    public void GetDamage(int amount)
    {
        int tempcurrentHealthPoints = playerStats.getCurrentHealthPoints() - amount;

        if (tempcurrentHealthPoints <= 0) // To be sure we never got negative healthpoints
        {
            tempcurrentHealthPoints = 0;
        }

        playerStats.SetCurrentHealthPoints(tempcurrentHealthPoints); // Then set healthpoint

        if (GetComponentInChildren<UI_Player_Stats>()) // If player take damage when he's already in the stats panel
            GetComponentInChildren<UI_Player_Stats>().RefreshStatsDisplay();

        if (playerStats.getCurrentHealthPoints() <= 0)
            Die();
        else if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

    // TODO Don't destroy gameobject entirely?
    public void Die()
    {
        // Pour que la caméra sur le joueur ne se detruise pas
        if (gameObject.GetComponentInChildren<Camera>())
        {
            gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
        }

        Destroy(gameObject);
    }
}
