/* FloatingText.cs
 * Utilisé uniquement pour faire flotter un texte. Comme par exemple les dégats infligés / reçus.
 * gere aussi la destruction du gameobject
 * */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        StartCoroutine(FadeTextColor());

        Destroy(gameObject, timerToDestroy);
    }

    // Create a fade effect on the text color.
    IEnumerator FadeTextColor()
    {
        yield return new WaitForSeconds(0.2f);

        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = GetComponent<Text>().color;
            c.a = f;
            GetComponent<Text>().color = c;

            yield return new WaitForSeconds(0.01f);
        }
    }
}
