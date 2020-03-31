/* FloatingText.cs
 * Utilisé uniquement pour faire flotter un texte. Comme par exemple les dégats infligés / reçus.
 * gere aussi la destruction du gameobject
 * */
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] float floatingSpeed = .5f;
    [SerializeField] float timerToDestroy = .5f;

    RectTransform myRect;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, floatingSpeed);
        Destroy(gameObject, timerToDestroy);
    }
}
