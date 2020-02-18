using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    // Damage
    public int projectileDamage = 5;
    [SerializeField] float projectileSpeed = 5f;

    [SerializeField] float timerBeforeDestroy = 5f;

    // If its a player's projectile we need to know about Stats_Control to give xp when this projectile make a kill
    [HideInInspector]
    public Player_Stats playerStats;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * projectileSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timerBeforeDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<AI_Health>())
            {
                AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                enemyHealth.GetDamage(projectileDamage);
                // If we kill the enemy
                if (enemyHealth.IsDead())
                {
                    // Check if npc got AI_Stats on him
                    AI_Stats enemyStats = collision.gameObject.GetComponent<AI_Stats>();
                    if (enemyStats)
                    {
                        if (playerStats)
                        {
                            playerStats.GetExperience(enemyStats.GetExperienceGain());
                        }
                    }
                }

            }
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player_Health>())
            {
                Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
                playerHealth.GetDamage(projectileDamage);
            }
            Destroy(gameObject);
        }
    }
}
