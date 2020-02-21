/* AI_Health.cs
Utilisé pour gérer la vie des NPCs
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Stats))] // To get healthpoints
public class AI_Health : MonoBehaviour
{
    // To know when AI just took damage, then we want to upgrade his chasing distance (via AI_Combat_Control)
    public bool damaged = false;
    [SerializeField] float damagedTimer = 5f;
    float currentDamagedTimer;

    [SerializeField] int currentHealthPoints = 100; // To deserialize

    [SerializeField] bool deathAnimation = false;
    bool isDead = false;
    float deathTiming = 5f; // For fix a bug, sometimes death animation don't play so gameobject isnt destroy

    Animator animator;
    AI_Stats ai_stats;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField]SpriteRenderer rend; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_stats = GetComponent<AI_Stats>();

        currentHealthPoints = ai_stats.GetHealthPoints();
        currentDamagedTimer = damagedTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Security for dead npcs
        if (isDead)
        {
            deathTiming -= Time.deltaTime;
            if (deathTiming <= 0f)
            {
                Die();
            }
            return;
        }

        if (damaged)
        {
            if (currentDamagedTimer > 0f)
            {
                currentDamagedTimer -= Time.deltaTime;
            }
            else
            {
                damaged = false;
                currentDamagedTimer = damagedTimer;
            }
        }
    }

    #region fade coroutines
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

    #endregion

    void PlayDeathAnimation()
    {
        // Disable others AI components
        if (GetComponent<AI_Movement_Control>())
            GetComponent<AI_Movement_Control>().enabled = false;
        if (GetComponent<AI_Moveset>())
            GetComponent<AI_Moveset>().enabled = false;
        if (GetComponent<AI_Combat_Control>())
            GetComponent<AI_Combat_Control>().enabled = false;
        // And collider (fix issue with xp twice a time)
        if (GetComponent<Collider2D>())
            GetComponent<Collider2D>().enabled = false;

        // Set all animator parameters to false
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("normalAttack", false);

        // Play animation
        animator.SetTrigger("isDead");
    }

    // Don't forget to put in every end of death's animations
    public void Die()
    {

        // Drop objects, golds...
        
        Destroy(gameObject);
    }

    // To know in other scripts if current npc is dead
    public bool IsDead()
    {
        return isDead;
    }

    public void GetDamage(int amount)
    {
        if (isDead)
            return;

        currentHealthPoints -= amount;

        if (currentHealthPoints <= 0)
        {
            isDead = true;
            if (deathAnimation) // Play death animation if there is one
            {
                PlayDeathAnimation();
            }
            else
            {
                Die();
            }
            return;
        }

        damaged = true;
        currentDamagedTimer = damagedTimer;

        if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

    public void SetHealthPoints(int amount)
    {
        currentHealthPoints = amount;
    }

    public void AddHealthPoints(int amount)
    {
        currentHealthPoints += amount;
    }
}
