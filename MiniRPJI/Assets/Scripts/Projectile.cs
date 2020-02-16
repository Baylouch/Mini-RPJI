using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    // Damage
    public int projectileDamage = 5;

    [SerializeField] float timerBeforeDestroy = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timerBeforeDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Player_Health>())
            {
                Player_Health enemyHealth = collision.gameObject.GetComponent<Player_Health>();
                enemyHealth.GetDamage(projectileDamage);
                
            }
            if (collision.gameObject.GetComponent<AI_Health>())
            {
                AI_Health enemyHealth = collision.gameObject.GetComponent<AI_Health>();
                enemyHealth.GetDamage(projectileDamage);

            }
            Destroy(gameObject);
        }
    }

}
