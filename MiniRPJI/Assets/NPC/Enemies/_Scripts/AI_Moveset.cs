using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
// Require AI_Enemy_Movement too for activation
public class AI_Moveset : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    [SerializeField] float nextMoveTimerMin = 1.5f;
    [SerializeField] float nextMoveTimerMax = 2f;
    [SerializeField] float movingDuration = 0.45f;

    bool autoMove = true;
    public bool GetAutoMove()
    {
        return autoMove;
    }
    bool currentlyAutoMove = false;
    bool readyForNextMove = false;

    float currentMovingTimer;
    int wayToGo; // To choose move direction
    int useMoveNumber; // To choose a predefined move set

    Rigidbody2D myRb;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();

        currentMovingTimer = Random.Range(nextMoveTimerMin, nextMoveTimerMax);
        wayToGo = 0;
        useMoveNumber = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (autoMove)
        {
            if (currentMovingTimer > 0f)
            {
                currentMovingTimer -= Time.fixedDeltaTime;
                if (readyForNextMove)
                    readyForNextMove = false;

            }
            else
            {
                if (!readyForNextMove)
                    readyForNextMove = true;
            }

            if (readyForNextMove && !currentlyAutoMove)
            {
                StartCoroutine("ProcessMovementSet");
            }
        }
    }

    IEnumerator ProcessMovementSet()
    {
        currentlyAutoMove = true;

        // If it's the first time we're here. (To get more randomness on the start moveset, without all npc will use the same)
        if (useMoveNumber == -1)
        {
            useMoveNumber = Random.Range(0, 4);
        }

        switch (useMoveNumber)
        {
            case 0:
                MoveAroundX();
                break;
            case 1:
                MoveAroundSquare();
                break;
            case 2:
                MoveAroundLeftAndRight();
                break;
            case 3:
                MoveAroundUpDown();
                break;
            default:
                Debug.Log("No moveset processing.");
                break;
        }

        yield return new WaitForSeconds(movingDuration);

        currentMovingTimer = Random.Range(nextMoveTimerMin, nextMoveTimerMax);
        currentlyAutoMove = false;
        readyForNextMove = false;
        wayToGo++;

        if (myRb.velocity != Vector2.zero)
        {
            myRb.velocity = Vector2.zero;
        }
    }

    public void ResetMovementParams()
    {
        wayToGo = -1;
        useMoveNumber = Random.Range(0, 4);
    }

    public void SwitchAutoMove(bool value)
    {
        if (value == true)
        {
            autoMove = true;
        }
        else
        {
            autoMove = false;
            currentlyAutoMove = false;
            StopAllCoroutines();
        }
    }

    #region AI Moveset
    void MoveAroundX()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
            ResetMovementParams();
        }
    }

    void MoveAroundSquare()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
            ResetMovementParams();
        }
    }

    void MoveAroundLeftAndRight()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(-1f, 0f) * speed;
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(1f, 0f) * speed;
            ResetMovementParams();
        }
    }

    void MoveAroundUpDown()
    {
        if (wayToGo == 0)
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
        }
        else if (wayToGo == 1)
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
        }
        else if (wayToGo == 2)
        {
            myRb.velocity = new Vector2(0f, -1f) * speed;
        }
        else if (wayToGo >= 3)
        {
            myRb.velocity = new Vector2(0f, 1f) * speed;
            ResetMovementParams();
        }
    }

    #endregion
}
