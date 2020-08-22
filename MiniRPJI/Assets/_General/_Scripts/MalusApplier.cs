/* MalusApplier.cs
 * 
 * Permet d'appliquer un malus au joueur ou à un ennemi.
 * 
 * 
 * */

using System.Collections;
using UnityEngine;

public enum MalusType { None, Slowed, InFire, Poisoned, Electrified }; // Used to know what projectile type from ApplyMalus coroutine.

[RequireComponent(typeof(IDamageable))]
public class MalusApplier : MonoBehaviour
{
    // Slowed variables
    [Header("Slow Malus")]
    [SerializeField] Color slowColor = Color.blue;

    // In fire variables
    [Header("Fire Malus")]
    [SerializeField] GameObject fireEffect; // A particle effect on sprite.

    // Poisoned variables
    [Header("Poison Malus")]
    [SerializeField] Color poisonColor = Color.magenta;

    // Electric variables
    [Header("Electric Malus")]
    [SerializeField] GameObject electricEffect;

    [SerializeField] Collider2D Env_Collider; // To disable when electrified

    Coroutine TakeDamagePerSecondCoroutine; // To know what damageOverTime coroutine is currently using
    Coroutine malusCoroutine; // To know when AI is already on a malus.
    MalusType currentMalus; // To know what is the current Malus to disable it before got a new one.

    GameObject currentEffect; // The current effect. No cumulable.
    float originalSpeed; // To know what's the speed at start. (When its got 0 malus on it)

    IDamageable health;
    Player_Stats player_Stats;
    AI_Stats ai_Stats;

    private void Start()
    {
        health = GetComponent<IDamageable>();

        if (health as Player_Health)
        {
            player_Stats = GetComponent<Player_Stats>();
            originalSpeed = player_Stats.GetSpeed();
        }
        else if (health as AI_Health)
        {
            ai_Stats = GetComponent<AI_Stats>();
            originalSpeed = ai_Stats.GetSpeed();
        }
        else if (health as Decoy_Health)
        {
            // Maybe later add specific thing here
        }
    }

    // TODO think about use only currentMalus and not malusCoroutine to check malus on the enemy ?

    // Method used in Player_Health when ai Die().
    public void RemoveCurrentEffect()
    {
        if (currentEffect)
            Destroy(currentEffect);
    }

    // Method used into Enemy_Projectile to deal overPower attack when enemy is already affect by the same malus type.
    public MalusType GetCurrentMalusType()
    {
        return currentMalus;
    }

    // Method call from Enemy_Projectile to set a malus on the Player.
    // bool takeDamage parameter exist because of overpower projectile. To inflict malus and damageovertime to ennemies but not projectile damage directly. (is now useless because no more overpower proj)
    public void SetMalus(MalusType type, int damageTaken, float malusTimer, float percentageChanceToApplyMalus, bool takeDamage = true)
    {
        // First before add a new Malus, verifity there is none already by checking malusCoroutine.
        RemoveMalus();

        malusCoroutine = StartCoroutine(ApplyMalus(type, damageTaken, malusTimer, percentageChanceToApplyMalus, takeDamage));
    }

    // Method to remove current malus on player.
    public void RemoveMalus()
    {
        if (malusCoroutine != null)
        {
            StopCoroutine(malusCoroutine);

            switch (currentMalus)
            {
                case MalusType.Slowed:
                    UnApplySlow();
                    break;
                case MalusType.InFire:
                    UnApplyFire();
                    break;
                case MalusType.Poisoned:
                    UnApplyPoison();
                    break;
                case MalusType.Electrified:
                    UnApplyElectric();
                    break;
            }
        }
    }

    // Method to apply malus inside SetMalus()
    IEnumerator ApplyMalus(MalusType type, int damageTaken, float malusTimer, float percentageChanceToApplyMalus, bool takeDamage)
    {
        switch (type)
        {
            // SLOWED PART
            case MalusType.Slowed:
                // Apply slow malus
                ApplySlow(damageTaken, takeDamage);

                // Wait slowed timer
                yield return new WaitForSeconds(malusTimer);

                // UnApply slow malus
                UnApplySlow();

                break;
            // FIRE PART
            case MalusType.InFire:
                // Apply fire malus
                ApplyFire(damageTaken, malusTimer, takeDamage);

                // Wait fire timer
                yield return new WaitForSeconds(malusTimer);

                // UnApply fire malus
                UnApplyFire();

                break;
            // POISONED PART
            case MalusType.Poisoned:
                // Apply poison malus
                ApplyPoison(damageTaken, malusTimer, takeDamage);

                // Wait poison timer
                yield return new WaitForSeconds(malusTimer);

                // UnApply poison malus
                UnApplyPoison();

                break;
            // ELECTRIFIED PART
            case MalusType.Electrified:
                // Apply electrified malus
                ApplyElectric(damageTaken, percentageChanceToApplyMalus, takeDamage);

                // Wait for electrified timer
                yield return new WaitForSeconds(malusTimer);

                // UnApply electrified malus
                UnApplyElectric();

                break;
        }
    }

    // Method to inflict damage over time (used for fire and poison)
    IEnumerator TakeDamagePerSecond(int damagePerSecond)
    {
        // Just wait a short amount of time and do the first burst, then burst every 1 sec
        yield return new WaitForSeconds(.5f);

        // Then go into infinite loop to do damage until we unapply malus.
        while (true)
        {
            if (health as AI_Health)
                (health as AI_Health).TakeDamageOverTime(damagePerSecond);
            else
                health.TakeDamage(damagePerSecond, false);

            yield return new WaitForSeconds(1);
        }
    }

    // ********************************** SLOW MALUS *********************************** \\
    void ApplySlow(int damageTaken, bool takeDamage)
    {
        if (takeDamage)
            health.TakeDamage(damageTaken, true);

        // Security for enemy to dont get malused if dead.
        if (health.IsDead())
            return;

        GetComponentInChildren<SpriteRenderer>().color = slowColor;

        if (health as Player_Health)
            player_Stats.SetSpeed(originalSpeed / 2);
        if (health as AI_Health)
            ai_Stats.SetSpeed(originalSpeed / 1.2f);

        currentMalus = MalusType.Slowed;
    }

    void UnApplySlow()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;

        if (health as Player_Health)
            player_Stats.SetSpeed(originalSpeed);
        if (health as AI_Health)
            ai_Stats.SetSpeed(originalSpeed);

        currentMalus = MalusType.None;
        malusCoroutine = null;
    }

    // ********************************** FIRE MALUS *********************************** \\
    void ApplyFire(int damageTaken, float malusTimer, bool takeDamage)
    {
        if (takeDamage)
            health.TakeDamage(damageTaken, true);

        // Security for enemy to dont get malused if dead.
        if (health.IsDead())
            return;

        if (fireEffect)
        {
            if (currentEffect != null)
            {
                Destroy(currentEffect);
            }

            float damagePerSecond = damageTaken / malusTimer;

            TakeDamagePerSecondCoroutine = StartCoroutine(TakeDamagePerSecond(Mathf.RoundToInt(damagePerSecond)));

            currentEffect = Instantiate(fireEffect, transform.position, Quaternion.identity);
            currentEffect.transform.parent = transform;
        }

        currentMalus = MalusType.InFire;
    }

    void UnApplyFire()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        if (TakeDamagePerSecondCoroutine != null)
        {
            StopCoroutine(TakeDamagePerSecondCoroutine);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }

    // ********************************** POISON MALUS *********************************** \\
    void ApplyPoison(int damageTaken, float malusTimer, bool takeDamage)
    {
        if (takeDamage)
            health.TakeDamage(damageTaken, true);

        // Security for enemy to dont get malused if dead.
        if (health.IsDead())
            return;

        GetComponentInChildren<SpriteRenderer>().color = poisonColor;

        float damagePerSecond = damageTaken / malusTimer;
        if (damagePerSecond < 1)
        {
            damagePerSecond = 1;
        }
        TakeDamagePerSecondCoroutine = StartCoroutine(TakeDamagePerSecond(Mathf.RoundToInt(damagePerSecond)));

        currentMalus = MalusType.Poisoned;
    }

    void UnApplyPoison()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;

        if (TakeDamagePerSecondCoroutine != null)
        {
            StopCoroutine(TakeDamagePerSecondCoroutine);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }

    // ********************************** ELECTRIC MALUS *********************************** \\
    void ApplyElectric(int damageTaken, float percentageChanceToParalyze, bool takeDamage)
    {
        if (takeDamage)
            health.TakeDamage(damageTaken, true);

        // Security for enemy to dont get malused if dead.
        if (health.IsDead())
            return;

        bool willApply = percentageChanceToParalyze > Random.Range(0, 101);
        if (willApply == true)
        {
            if (health as Player_Health)
            {
                GetComponent<Player_Movement>().canMove = false;
            }
            if (health as AI_Health)
                ai_Stats.SetSpeed(0f);

            Env_Collider.enabled = false; // disable ai_collider to avoid AI to be push by others

            if (currentEffect != null)
            {
                Destroy(currentEffect);
            }

            currentEffect = Instantiate(electricEffect, transform.position, Quaternion.identity);
            currentEffect.transform.parent = transform;

            currentMalus = MalusType.Electrified;

        }
    }

    void UnApplyElectric()
    {
        if (health as Player_Health)
        {
            GetComponent<Player_Movement>().canMove = true;

        }
        if (health as AI_Health)
            ai_Stats.SetSpeed(originalSpeed);

        Env_Collider.enabled = true;

        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        currentMalus = MalusType.None;
        malusCoroutine = null;

    }
}
