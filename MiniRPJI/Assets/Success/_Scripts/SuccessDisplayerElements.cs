using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SuccessDisplayerElements : MonoBehaviour
{
    public Text successTitle;
    public Text successObjective;
    public Image successImage;
    public Button quitButton;

    [SerializeField] Image[] images = new Image[3]; // To do a fade effect
    [SerializeField] float timerToDestroy = 5f;
    float spawnedTime;
    bool destroyPhase = false;

    private void Start()
    {
        spawnedTime = Time.time;

        Destroy(gameObject, 30f); // A security to avoid popup to stay infinitly by any ways possible
    }

    private void Update()
    {
        if (Time.time > spawnedTime + timerToDestroy)
        {
            if (!destroyPhase)
            {
                destroyPhase = true;
                StartCoroutine(FadeOut());
            }
        }
    }

    // Method to fade every component before destroy
    IEnumerator FadeOut() // Dissapear
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = images[0].color;
            c.a = f;
            images[0].color = c;

            c = images[1].color;
            c.a = f;
            images[1].color = c;

            c = images[2].color;
            c.a = f;
            images[2].color = c;

            c = successTitle.color;
            c.a = f;
            successTitle.color = c;

            c = successObjective.color;
            c.a = f;
            successObjective.color = c;

            c = successImage.color;
            c.a = f;
            successImage.color = c;

            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }

}
