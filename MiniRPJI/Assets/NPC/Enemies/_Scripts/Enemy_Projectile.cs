using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Projectile : MonoBehaviour
{
    [HideInInspector]
    public int projectileDamage = 0; // Is set in AI_Enemy_Combat

    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float timerBeforeDestroy = 3f;
    [SerializeField] GameObject impactEffect;

    bool used = false;

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
        if (used)
            return;

        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player_Health>())
            {
                Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
                playerHealth.TakeDamage(projectileDamage);
            }

            if (collision.gameObject.GetComponent<Decoy_Health>())
            {
                Decoy_Health decoyHealth = collision.gameObject.GetComponent<Decoy_Health>();
                decoyHealth.TakeDamage(projectileDamage);
            }

            if (impactEffect)
            {
                GameObject _impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(_impact, .5f);
            }

            Destroy(gameObject);
        }
    }
}
