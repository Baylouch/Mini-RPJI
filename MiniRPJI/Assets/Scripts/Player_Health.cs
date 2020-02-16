/* Health.cs
Utilisé pour gérer la vie du joueur
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats_Control))]
public class Player_Health : MonoBehaviour
{
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float currentHealthPoints = 100f; // To deserialize
    [SerializeField] float vitalityMultiplier = 10f;

    Stats_Control currentStats;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField]SpriteRenderer rend; 

    // Start is called before the first frame update
    void Start()
    {
        currentStats = GetComponent<Stats_Control>();

        maxHealthPoints = currentStats.GetCurrentStatsByType(StatsType.VITALITY) * vitalityMultiplier;
        currentHealthPoints = maxHealthPoints;
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

    public void GetDamage(float amount)
    {
        currentHealthPoints -= amount;

        if (currentHealthPoints <= 0)
            Die();

        if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

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
