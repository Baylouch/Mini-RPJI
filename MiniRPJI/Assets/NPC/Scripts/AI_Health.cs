/* AI_Health.cs
Utilisé pour gérer la vie des NPCs
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AI_Stats))] // To get healthpoints
public class AI_Health : MonoBehaviour
{
    // To know when AI just took damage, then we want to upgrade his chasing distance (via AI_Combat_Control)
    public bool damaged = false;
    [SerializeField] float damagedTimer = 5f;
    float currentDamagedTimer;

    [SerializeField] bool slowed = false;
    [SerializeField] float slowedTimer = 2f;
    float currentSlowedTimer;
    float trackSpeedBeforeDivide;
    bool slowedSet = false;

    [SerializeField] bool deathAnimation = false;
    bool isDead = false;
    float deathTiming = 5f; // For fix a bug, sometimes death animation don't play so gameobject isnt destroy

    [SerializeField] GameObject damageText; // To display damage inflict by player
    [SerializeField] Transform damageTextParent; // Used to instantiate damageText as child of this

    Animator animator;
    AI_Stats ai_stats;

    [Tooltip("Let it null if you don't want fading when damage taken.")]
    [SerializeField]SpriteRenderer rend; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ai_stats = GetComponent<AI_Stats>();

        currentDamagedTimer = damagedTimer;
        currentSlowedTimer = slowedTimer;
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

        if (slowed)
        {
            if (currentSlowedTimer > 0f)
            {
                currentSlowedTimer -= Time.deltaTime;
                if (!slowedSet)
                {
                    GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                    trackSpeedBeforeDivide = GetComponent<AI_Movement_Control>().speed;
                    GetComponent<AI_Movement_Control>().speed /= 2;
                    slowedSet = true;
                }
            }
            else
            {
                slowed = false;
                if (slowedSet)
                {
                    GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    GetComponent<AI_Movement_Control>().speed = trackSpeedBeforeDivide;
                    slowedSet = false;
                }
                currentSlowedTimer = slowedTimer;
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

        // If AI was moving, stop it
        if (GetComponent<Rigidbody2D>())
            if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            

        // Play animation
        animator.SetTrigger("isDead");
    }

    // Don't forget to put in every end of death's animations
    public void Die()
    {
        StopAllCoroutines();

        // Drop objects, golds...
        if (GetComponent<ItemDroper>())
        {
            GetComponent<ItemDroper>().DropItems();
        }
        
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

        // Display damage on the UI
        if (damageText)
        {
            GameObject _damagedText = Instantiate(damageText, damageTextParent);
            _damagedText.GetComponent<Text>().text = amount.ToString();
        }

        int tempHealthPoints = ai_stats.GetCurrentHealthPoints() - amount;
        if (tempHealthPoints < 0)
        {
            ai_stats.SetCurrentHealthPoints(0);
        }
        else
        {
            ai_stats.SetCurrentHealthPoints(tempHealthPoints);
        }

        if (ai_stats.GetCurrentHealthPoints() <= 0)
        {
            isDead = true;

            if (GetComponentInChildren<UI_Enemy>())
                GetComponentInChildren<UI_Enemy>().gameObject.SetActive(false);

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

    // Method use in Projectile.cs for slow enemy (or reset slowing if already is)
    public void Slowed()
    {
        slowed = true;
        currentSlowedTimer = slowedTimer;
    }
}
