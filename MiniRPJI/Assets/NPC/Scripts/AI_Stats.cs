using UnityEngine;

public class AI_Stats : MonoBehaviour
{
    [SerializeField] int level = 1;
    [SerializeField] int totalHealthPoints = 50;
    [SerializeField] int experienceGain = 50;

    // attack
    [SerializeField] private int damageMin = 7;
    [SerializeField] private int damageMax = 12;

    // Projectile damage
    [SerializeField] private int projectileDamageMin = 8;
    [SerializeField] private int projectileDamageMax= 10;

    int currentHealthPoints;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentHealthPoints(totalHealthPoints);
    }

    // For now just retourn experienceGain as experience gain. Later think about some maths
    public int GetExperienceGain()
    {
        return experienceGain;
    }

    public int GetTotalHealthPoints()
    {
        return totalHealthPoints;
    }

    public int GetCurrentHealthPoints()
    {
        return currentHealthPoints;
    }

    public void SetCurrentHealthPoints(int newHealthPoints)
    {
        currentHealthPoints = newHealthPoints;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetDamageMin()
    {
        return damageMin;
    }

    public int GetDamageMax()
    {
        return damageMax;
    }

    public int GetProjectileDamageMin()
    {
        return projectileDamageMin;
    }

    public int GetProjectileDamageMax()
    {
        return projectileDamageMax;
    }
}
