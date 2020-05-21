/* PositionRendererSorter.cs :
 * 
 * Provient du tutoriel youtube posté par Code Monkey : Quick Tip: Sorting Sprites in a 2D Game | Unity Tutorial
 * 
 * A attaché sur chaque gameobjects possèdant un sprite renderer afin que l'affichage soit correct. (Passer devant ou derriere un arbre par exemple)
 * 
 * */

using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField] private int sortingOrderBase = 5000;
    [SerializeField] private float offset = 0f;

    [SerializeField] private bool runOnlyOnce = false;

    [SerializeField] UI_Enemy enemyUI;

    private float timer;
    private float timerMax = .1f;
    private Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = timerMax;
            myRenderer.sortingOrder = Mathf.RoundToInt(sortingOrderBase - transform.position.y - offset);

            if (enemyUI != null)
            {
                if (enemyUI.GetCanvas() != null)
                    enemyUI.GetCanvas().sortingOrder = myRenderer.sortingOrder;
            }

            if (runOnlyOnce)
                Destroy(this);
        }
    }
}
