using UnityEngine;

public class VerticalDisplayingCredits : MonoBehaviour
{
    float speed = 70f;

    RectTransform myRect;

    float timerBeforeDestroy = 120f;

    float currentTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            return;

        currentTimer += Time.deltaTime;


        myRect.position = new Vector3(myRect.position.x, myRect.position.y + speed * Time.deltaTime);

        if (currentTimer >= timerBeforeDestroy)
        {
            Destroy(gameObject);
        }
    }
}
