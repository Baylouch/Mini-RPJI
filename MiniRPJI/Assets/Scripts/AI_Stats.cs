using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stats : MonoBehaviour
{
    // Think about centralize all npc stats here as healthpoint, attack, armor
    public int level = 1;
    [SerializeField] int healthPoints = 100;
    [SerializeField] int experienceGain = 50;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public int GetHealthPoints()
    {
        return healthPoints;
    }
}
