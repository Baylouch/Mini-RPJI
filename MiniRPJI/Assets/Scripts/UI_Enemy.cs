using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Enemy : MonoBehaviour
{

    [SerializeField] RectTransform healthLine;

    AI_Stats ai_stats;

    // Start is called before the first frame update
    void Start()
    {
        ai_stats = GetComponentInParent<AI_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ai_stats)
            healthLine.sizeDelta = new Vector2((ai_stats.GetCurrentHealthPoints() * 100) / ai_stats.GetTotalHealthPoints(), healthLine.sizeDelta.y);
    }
}
