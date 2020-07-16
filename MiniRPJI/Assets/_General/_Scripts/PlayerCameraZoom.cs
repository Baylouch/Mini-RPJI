using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (IsMouseOverUI())
            return;

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

    // Method to know when mouse is over UI then dont attack
    bool IsMouseOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }
}
