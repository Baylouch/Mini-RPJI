using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HurtEffect : MonoBehaviour
{
    public float alphaLimit = 0; // 0.0 to 1.0. Set into Player_Health

    [SerializeField] Image[] hurtImage;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    public void StartFadeEffect()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn() // Reapear
    {
        for (float f = 0.05f; f <= alphaLimit; f += 0.05f)
        {
            for (int i = 0; i < hurtImage.Length; i++)
            {
                Color c = hurtImage[i].color;
                c.a = f;
                hurtImage[i].color = c;
            }

            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine("FadeOut");       
    }

    IEnumerator FadeOut() // Dissapear
    {
        for (float f = alphaLimit; f >= -0.05f; f -= 0.05f)
        {
            for (int i = 0; i < hurtImage.Length; i++)
            {
                Color c = hurtImage[i].color;
                c.a = f;
                hurtImage[i].color = c;
            }

            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }
}
