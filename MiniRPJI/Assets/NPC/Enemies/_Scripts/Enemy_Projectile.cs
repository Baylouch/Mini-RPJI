using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Projectile : MonoBehaviour
{
    [HideInInspector]
    public int projectileDamage = 0; // Is set in AI_Enemy_Combat

    public ProjectileType projectileType; // To set projectile effect (frost, fire, nothing..)

    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float timerBeforeDestroy = 3f;
    [SerializeField] float malusTimer = 2f;
    [SerializeField] float percentageChanceToApplyMalus = 100f;
    [SerializeField] GameObject impactEffect;
    [SerializeField] AudioClip impactSound;

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

                if (playerHealth.GetComponent<MalusApplier>())
                {
                    MalusApplier malusApplier = playerHealth.GetComponent<MalusApplier>();

                    // Now get projectile type to set the right malus on the enemy
                    switch (projectileType)
                    {
                        // Normal projectile
                        case ProjectileType.Normal:
                            playerHealth.TakeDamage(projectileDamage, true);
                            break;
                        // Frost projectile
                        case ProjectileType.Frost:
                            ProjectileNormalAttack(malusApplier, MalusType.Slowed);
                            break;
                        // Fire projectile
                        case ProjectileType.Fire:
                            ProjectileNormalAttack(malusApplier, MalusType.InFire);
                            break;
                        // Poison projectile
                        case ProjectileType.Poison:
                            ProjectileNormalAttack(malusApplier, MalusType.Poisoned);
                            break;
                        // Electric projectile
                        case ProjectileType.Electric:
                            ProjectileNormalAttack(malusApplier, MalusType.Electrified);
                            break;
                    }
                }
                else
                {
                    playerHealth.TakeDamage(projectileDamage, true);
                }
            }

            if (collision.gameObject.GetComponent<Decoy_Health>())
            {
                Decoy_Health decoyHealth = collision.gameObject.GetComponent<Decoy_Health>();

                if(decoyHealth.GetComponent<MalusApplier>())
                {
                    MalusApplier malusApplier = decoyHealth.GetComponent<MalusApplier>();

                    // Now get projectile type to set the right malus on the enemy
                    switch (projectileType)
                    {
                        // Normal projectile
                        case ProjectileType.Normal:
                            decoyHealth.TakeDamage(projectileDamage, true);
                            break;
                        // Frost projectile
                        case ProjectileType.Frost:
                            ProjectileNormalAttack(malusApplier, MalusType.Slowed);
                            break;
                        // Fire projectile
                        case ProjectileType.Fire:
                            ProjectileNormalAttack(malusApplier, MalusType.InFire);
                            break;
                        // Poison projectile
                        case ProjectileType.Poison:
                            ProjectileNormalAttack(malusApplier, MalusType.Poisoned);
                            break;
                        // Electric projectile
                        case ProjectileType.Electric:
                            ProjectileNormalAttack(malusApplier, MalusType.Electrified);
                            break;
                    }
                }
                else
                {
                    decoyHealth.TakeDamage(projectileDamage, true);
                }
            }

            if (impactEffect)
            {
                GameObject _impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(_impact, .5f);
            }

            if (impactSound)
            {
                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(impactSound);
                }
            }

            Destroy(gameObject);
        }
    }

    // Method to deal not overpower arrow attack, inflict damage and set malus if projectile got a type linked.
    void ProjectileNormalAttack(MalusApplier _malusApplier, MalusType type)
    {
        _malusApplier.SetMalus(type, projectileDamage, malusTimer, percentageChanceToApplyMalus);

        if (impactEffect)
        {
            GameObject _impact = Instantiate(impactEffect, transform.position, impactEffect.transform.rotation);
            Destroy(_impact, .5f);
        }
    }
}
