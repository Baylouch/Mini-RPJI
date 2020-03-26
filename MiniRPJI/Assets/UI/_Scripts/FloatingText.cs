/* FloatingText.cs
 * Utilisé uniquement pour faire flotter un texte. Comme par exemple les dégats infligés / reçus.
 * gere aussi la destruction du gameobject
 * */
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] float floatSpeed = .02f;
    [SerializeField] float timerToDestroy = .5f;

    RectTransform myRect;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
        Destroy(gameObject, timerToDestroy);
    }

    private void Update()
    {
        myRect.position = new Vector3(myRect.position.x, myRect.position.y + floatSpeed, myRect.position.z);
    }
}
