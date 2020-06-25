using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Decoy_Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int totalHealthPoints = 100; // Total player healthpoints
    public int GetTotalHealthPoints()
    {
        return totalHealthPoints;
    }
    public void SetTotalHealthPoints(int newTotal)
    {
        totalHealthPoints = newTotal;
    }

    private int currentHealthPoints; // Player current healthpoints
    public int GetCurrentHealthPoints()
    {
        return currentHealthPoints;
    }
    public void SetCurrentHealthPoints(float newHealthPoints)
    {
        int tempHealth = Mathf.RoundToInt(newHealthPoints);

        if (newHealthPoints < 0)
        {
            currentHealthPoints = 0;
        }
        else
        {
            currentHealthPoints = tempHealth;
        }
    }

    [SerializeField] GameObject damageText;
    [SerializeField] Transform DamageTextParent;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField] SpriteRenderer rend;

    [SerializeField] RectTransform healthLine;

    float lastDamagedTextXPosition = 0;
    bool isDead = false;
    public bool IsDead()
    {
        return isDead;
    }

    private void Start()
    {
        Player_Health playerHealth = Player_Stats.instance.GetComponent<Player_Health>();

        SetTotalHealthPoints(playerHealth.GetTotalHealthPoints());
        SetCurrentHealthPoints(GetTotalHealthPoints());
    }

    private void Update()
    {
        if (healthLine.sizeDelta != new Vector2((currentHealthPoints * 100) / totalHealthPoints, healthLine.sizeDelta.y))
        {
            healthLine.sizeDelta = new Vector2((currentHealthPoints * 100) / totalHealthPoints, healthLine.sizeDelta.y);
        }
    }

    #region Fade Coroutines

    // A FadeIn and FadeOut coroutine methods to apply on each sprite who doesnt have "TakeDamage" animation
    IEnumerator FadeIn() // Reapear
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;

            yield return new WaitForSeconds(0.01f);
        }
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

    void DisplayDamagedTextUI(int amount)
    {
        if (damageText)
        {
            GameObject _damagedText = Instantiate(damageText, DamageTextParent); // Create damagedText GO
            RectTransform _damagedTextRect = _damagedText.GetComponent<RectTransform>(); // Get acces to the RectTransform component

            float randomPosX = 0;

            // Give random position to _damagedText on X between -0.5f - 0.5f.
            // Check what was the last X position of the damaged text
            if (lastDamagedTextXPosition > 0) // If it was on the left
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x - 0.2f, _damagedTextRect.localPosition.x - 0.6f);
            }
            else if (lastDamagedTextXPosition < 0) // It was on the right
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x + 0.2f, _damagedTextRect.localPosition.x + 0.6f);
            }
            else // If its equal to 0, first time we use it.
            {
                randomPosX = Random.Range(_damagedTextRect.localPosition.x - 0.6f, _damagedTextRect.localPosition.x + 0.6f);
            }

            lastDamagedTextXPosition = randomPosX;

            _damagedTextRect.localPosition = new Vector2(randomPosX, _damagedTextRect.localPosition.y);

            _damagedText.GetComponent<Text>().text = amount.ToString();
        }
    }

    public void TakeDamage(int amount, bool playSound)
    {
        // Reduction of damage amount by % depending of our armor.
        // Full calculation is : CurrentArmor * 0.05 = resultat (% of attack reduction)
        //                       resultat /= 100 to obtain like 0.05 to reduce attack by 5% for exemple
        //                       DamageTaken = TotalDmg - TotalDmg * resultat

        float percentageOfAttackReduction = Player_Stats.instance.GetArmor() * Player_Stats.instance.GetArmorMultiplier();
        percentageOfAttackReduction /= 100;
        // Security for max attack reduction (60% of the attack max)
        if (percentageOfAttackReduction > 0.60f)
        {
            percentageOfAttackReduction = 0.60f;
        }
        float tempDamageAmount = amount - amount * percentageOfAttackReduction;

        // Display on the screen damage received
        DisplayDamagedTextUI(amount);

        float afterDamageHealthPoints = currentHealthPoints - tempDamageAmount;

        SetCurrentHealthPoints(afterDamageHealthPoints); // Then set healthpoint

        if (currentHealthPoints <= 0)
        {
            Die();
        }
        else if (rend)
        {
            StartCoroutine("FadeOut");
        }
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
