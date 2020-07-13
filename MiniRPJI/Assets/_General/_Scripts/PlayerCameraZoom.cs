using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCameraZoom : MonoBehaviour
{
    [SerializeField][Range(7, 11)] float camSize = 9;

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (cam.orthographicSize < 11f)
            {
                cam.orthographicSize += 0.3f;
            }
            else
            {
                cam.orthographicSize = 11f;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (cam.orthographicSize > 7f)
            {
                cam.orthographicSize -= 0.3f;
            }
            else
            {
                cam.orthographicSize = 7f;
            }
        }
    }
}
