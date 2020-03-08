using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stats : MonoBehaviour
{
    // Think about centralize all npc stats here as healthpoint, attack, armor
    public int level = 1;
    [SerializeField] int totalHealthPoints = 100;
    [SerializeField] int experienceGain = 50;

    int currentHealthPoints;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentHealthPoints(totalHealthPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
