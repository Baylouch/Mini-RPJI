/* AI_Health.cs
Utilisé pour gérer la vie des NPCs
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AI_Health : MonoBehaviour
{
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float currentHealthPoints = 100f; // To deserialize

    [SerializeField] bool deathAnimation = false;
    bool isDead = false;
    float deathTiming = 5f; // For fix a bug, sometimes death animation don't play so gameobject isnt destroy

    Animator animator;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField]SpriteRenderer rend; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        currentHealthPoints = maxHealthPoints;
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
        }
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
        if (isDead)
            return;

        currentHealthPoints -= amount;

        if (currentHealthPoints <= 0)
        {
            isDead = true;
            if (deathAnimation) // Play death animation if there is one
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("normalAttack", false);
                animator.SetTrigger("isDead");
            }
            else
            {
                Die();
            }
        }
            
        if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

    // Don't forget to put in every end of death's animations
    public void Die()
    {

        // Drop objects, golds...

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
