using UnityEngine;
using UnityEngine.UI;

public class UI_Enemy : MonoBehaviour
{
    [SerializeField] RectTransform healthLine;
    [SerializeField] Text levelText;

    AI_Stats ai_stats;
    Canvas myCanvas;
    public Canvas GetCanvas()
    {
        if (myCanvas != null)
            return myCanvas;
        else
            return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        ai_stats = GetComponentInParent<AI_Stats>();

        if (ai_stats && levelText)
            levelText.text = ai_stats.GetLevel().ToString();

        myCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ai_stats)
        {
            if (healthLine.sizeDelta != new Vector2((ai_stats.GetCurrentHealthPoints() * 100) / ai_stats.GetTotalHealthPoints(), healthLine.sizeDelta.y))
            {
                healthLine.sizeDelta = new Vector2((ai_stats.GetCurrentHealthPoints() * 100) / ai_stats.GetTotalHealthPoints(), healthLine.sizeDelta.y);
            }
        }
    }
}
