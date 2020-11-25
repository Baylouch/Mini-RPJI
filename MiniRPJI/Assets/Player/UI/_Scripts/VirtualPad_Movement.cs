using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class VirtualPad_Movement : MonoBehaviour
{
    // Player_Movement.cs is actived before VirtualPad_Movement.cs

    Player_Movement p_movement;

    int mouseOverDirection = -1; // 0 = Top, 1 = Down, 2 = Left, 3 = Right.
    bool clicking = false; // To know when the player is clicking
    
    float doubleClicTimer = 0.3f; // To test double clic to process dash
    float lastTimeClic = 0f;

    // Start is called before the first frame update
    void Start()
    {
        p_movement = FindObjectOfType<Player_Movement>();

        if (!p_movement)
        {
            Debug.Log("No Player_Movement found to set VirtualPad_Movement.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        // We check if mouse (finger) is on move trigger
        if (IsMouseOverMoveTrigger())
        {
            // To know the last time player clicked to handle double clic
            if (Input.GetMouseButtonDown(0) && !clicking)
            {
                // To process dashing we want to get a double clic input
                if (lastTimeClic + doubleClicTimer >= Time.time)
                {
                    // Process dashing
                    p_movement.ProcessDashing();
                    lastTimeClic = Time.time;

                    return;
                }

                lastTimeClic = Time.time;
            }

            // If the player is currently clicking on the left mouse button
            if (Input.GetMouseButton(0))
            {
                if (!clicking)
                    clicking = true;

                switch (mouseOverDirection)
                {
                    case 0:
                        p_movement.ProcessUpperMove();
                        break;
                    case 1:
                        p_movement.ProcessDownMove();
                        break;
                    case 2:
                        p_movement.ProcessLeftMove();
                        break;
                    case 3:
                        p_movement.ProcessRightMove();
                        break;
                    default: break;
                }
            }
            else
            {
                p_movement.StopMove();
            }

            if (Input.GetMouseButtonUp(0) && clicking)
            {
                clicking = false;
            }
        }
        else // If we are not more on a move trigger and we were clicking, then stop the player. 
        {
            if (clicking)
            {
                clicking = false;

                p_movement.StopMove();
            }
        }
       
    }

    // Method to know when mouse is over a move button
    bool IsMouseOverMoveTrigger()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.tag == "MoveTrigger")
            {
                switch (raycastResultList[i].gameObject.name)
                {
                    case "Top movement trigger":
                        mouseOverDirection = 0;
                        break;
                    case "Down movement trigger":
                        mouseOverDirection = 1;
                        break;
                    case "Left movement trigger":
                        mouseOverDirection = 2;
                        break;
                    case "Right movement trigger":
                        mouseOverDirection = 3;
                        break;
                    default: break;
                }

                return true;
            }
        }

        return false;
    }

}
