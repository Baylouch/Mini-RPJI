using UnityEngine;
using System.Collections;

public enum Enemy_Projectile_Movement_Type { ZigZag, Triangle, Circle, Boomrang, Random, None };

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy_Projectile))]
public class Enemy_Projectile_Movement : MonoBehaviour
{
    [HideInInspector]
    public AI_Enemy_Combat theEnemyWhoLaunchTheProjectile;

    [SerializeField] Enemy_Projectile_Movement_Type movementType;

    Rigidbody2D myRb;

    Enemy_Projectile projectile;

    [SerializeField] float intervalMovement = .2f;

    float zRotation = 0f;
    bool applyCircleMove = false;
    Vector3 rotationAxis = new Vector3(0f, 0f, 1f);
    float rotationForce = 230f;

    [SerializeField] GameObject emptyPrefabToApplyCircle;
    GameObject throwedGO;

    // Start is called before the first frame update
    void Start()
    {
        // Just a security
        if (movementType == Enemy_Projectile_Movement_Type.None)
        {
            Destroy(this);
        }

        myRb = GetComponent<Rigidbody2D>();
        projectile = GetComponent<Enemy_Projectile>();

        if (theEnemyWhoLaunchTheProjectile != null)
            zRotation = theEnemyWhoLaunchTheProjectile.GetFirePointRotationZ();
        else
        {
            Debug.Log("No AI_Enemy_Combat to apply Enemy_Projectile_Movement.");
            Destroy(this);
        }

        switch (movementType)
        {
            case Enemy_Projectile_Movement_Type.ZigZag:
                StartCoroutine(ZigZag());
                break;
            case Enemy_Projectile_Movement_Type.Triangle:
                StartCoroutine(Triangle());
                break;
            case Enemy_Projectile_Movement_Type.Circle:
                SetCircleMovement();
                break;
            case Enemy_Projectile_Movement_Type.Boomrang:
                StartCoroutine(Boomrang());
                break;
            case Enemy_Projectile_Movement_Type.Random:
                int randomVar = Random.Range(0, 5); // 5 is exclusive
                switch(randomVar)
                {
                    case 0:
                        StartCoroutine(ZigZag());
                        break;
                    case 1:
                        StartCoroutine(Triangle());
                        break;
                    case 2:
                        SetCircleMovement();
                        break;
                    case 3:
                        StartCoroutine(Boomrang());
                        break;
                    case 4: // Apply no movement
                        Destroy(this);
                        break;
                    default: break;
                }
                break;
            default: break;
        }
    }

    private void Update()
    {
        if (throwedGO && applyCircleMove)
        {
            transform.RotateAround(throwedGO.transform.position, rotationAxis, rotationForce * Time.deltaTime);            
        }
    }

    void SetCircleMovement()
    {
        // TODO INSTEAD OF CREATE NEW GAMEOBJECT, WE MUST INSTANTIATE IT VIA A PREFAB.
        // BECAUSE UNITY DONT LET YOU CHANGE THE ROTATION OF A NEW GAMEOBJECT. BUT WITH A PREFAB ITS OK. (DONE)

        // ADD RANDOMNESS (DONE)
        // TO THE POSITION
        // TO THE ROTATIONAXIS - / +
        // TO THE SPEED
        Vector3 throwedPos = new Vector3(Random.Range(theEnemyWhoLaunchTheProjectile.transform.position.x - 2f, theEnemyWhoLaunchTheProjectile.transform.position.x + 2f),
                                         Random.Range(theEnemyWhoLaunchTheProjectile.transform.position.y - 2f, theEnemyWhoLaunchTheProjectile.transform.position.y + 2f),
                                         0f);

        GameObject throwedGOTemp = Instantiate(emptyPrefabToApplyCircle, throwedPos, theEnemyWhoLaunchTheProjectile.GetFirePointRotation());

        Rigidbody2D tempRB = throwedGOTemp.AddComponent<Rigidbody2D>();
        tempRB.gravityScale = 0;
        tempRB.velocity = throwedGOTemp.transform.up * (projectile.GetProjectileSpeed() / Random.Range(1f, 2f));

        throwedGO = throwedGOTemp;

        int rotForceCondition = Random.Range(1, 3);

        if (rotForceCondition > 1)
        {
            rotationForce = -rotationForce;
        }

        applyCircleMove = true;

        Destroy(throwedGOTemp, projectile.GetTimerBeforeDestroy() + 1f);
    }

    IEnumerator ZigZag()
    {
        bool toggler = true;
        bool firstTime = true; // To avoid projectile to go too down or up.

        while (true)
        {
            if (zRotation == 0) // Top direction
            {
                if (firstTime)
                {
                    if (myRb.velocity != new Vector2(.5f, 1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(.5f, 1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                    }

                    firstTime = false;
                }
                else if (toggler)
                {
                    if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                    }

                }
                else
                {
                    if (myRb.velocity != new Vector2(-1f, 1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != 35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                    }
                }
            }
            else if (zRotation == 180) // Bottom direction
            {
                if (firstTime)
                {
                    if (myRb.velocity != new Vector2(.5f, -1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(.5f, -1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -135f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                    }

                    firstTime = false;
                }
                else if(toggler)
                {
                    if (myRb.velocity != new Vector2(1f, -1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -135f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                    }
                }
                else
                {
                    if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != 135f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                    }
                }
            }
            else if (zRotation == 90) // Left direction
            {
                if (firstTime)
                {
                    if (myRb.velocity != new Vector2(-1f, .5f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(-1f, .5f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != 35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                    }

                    firstTime = false;
                }
                else if(toggler)
                {
                    if (myRb.velocity != new Vector2(-1f, 1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();

                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != 35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                    }
                }
                else
                {
                    if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();


                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != 135f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                    }
                }
            }
            else if (zRotation == 270) // Right direction
            {
                if (firstTime)
                {
                    if (myRb.velocity != new Vector2(1f, .5f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(1f, .5f) * projectile.GetProjectileSpeed();


                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                    }

                    firstTime = false;
                }
                else if(toggler)
                {
                    if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();


                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -35f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                    }
                }
                else
                {
                    if (myRb.velocity != new Vector2(1f, -1f) * projectile.GetProjectileSpeed())
                        myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();


                    if (!GetComponent<MakeProjectileRotate>())
                    {
                        if (transform.rotation.eulerAngles.z != -135f)
                            transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                    }
                }
            }
            
            toggler = !toggler;

            yield return new WaitForSeconds(intervalMovement);
        }
    }

    IEnumerator Triangle()
    {
        float specificMovementInterval = .2f;

        while (true)
        {
            if (zRotation == 0) // Top direction
            {
                if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 0f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(0f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 0f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else if (zRotation == 180) // Bottom direction
            {
                if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 0f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(0f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 180f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                }
            }
            else if (zRotation == 90) // Left direction
            {
                if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(0f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 180f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, 0f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }
            }
            else if (zRotation == 270) // Right direction
            {
                if (myRb.velocity != new Vector2(1f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(-1f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(0f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(0f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 0f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                yield return new WaitForSeconds(specificMovementInterval);

                if (myRb.velocity != new Vector2(1f, 0f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, 0f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                }
            }

            yield return new WaitForSeconds(intervalMovement);

        }
    }

    IEnumerator Boomrang()
    {
        while (true)
        {
            if (zRotation == 0) // Top direction
            {
                if (myRb.velocity != new Vector2(.5f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.5f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-.5f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.5f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-.2f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.2f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 15f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 15f);
                }

                yield return new WaitForSeconds(intervalMovement / 2);

                if (myRb.velocity != new Vector2(-.5f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.5f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(.5f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.5f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(.2f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.2f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 180f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                }

                yield return new WaitForSeconds(intervalMovement);

                Destroy(gameObject);
            }
            else if (zRotation == 180) // Bottom direction
            {
                if (myRb.velocity != new Vector2(.5f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.5f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-.5f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.5f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-.2f, -1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.2f, -1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 15f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 15f);
                }

                yield return new WaitForSeconds(intervalMovement / 2);

                if (myRb.velocity != new Vector2(-.5f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-.5f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(.5f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.5f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(.2f, 1f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(.2f, 1f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 0f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                yield return new WaitForSeconds(intervalMovement);

                Destroy(gameObject);
            }
            else if (zRotation == 90) // Left direction
            {
                if (myRb.velocity != new Vector2(-1f, .5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, .5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-1f, -.5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -.5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-1f, -.2f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -.2f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 115f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 115f);
                }

                yield return new WaitForSeconds(intervalMovement / 2);

                if (myRb.velocity != new Vector2(1f, -.5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -.5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(1f, .5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, .5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(1f, .2f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, .2f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                }

                yield return new WaitForSeconds(intervalMovement);

                Destroy(gameObject);
            }
            else if (zRotation == 270) // Right direction
            {
                if (myRb.velocity != new Vector2(1f, .5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, .5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(1f, -.5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -.5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(1f, -.2f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(1f, -.2f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != -115f)
                        transform.rotation = Quaternion.Euler(0f, 0f, -115f);
                }

                yield return new WaitForSeconds(intervalMovement / 2);

                if (myRb.velocity != new Vector2(-1f, -.5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, -.5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 135f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-1f, .5f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, .5f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 35f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 35f);
                }

                yield return new WaitForSeconds(intervalMovement);

                if (myRb.velocity != new Vector2(-1f, .2f) * projectile.GetProjectileSpeed())
                    myRb.velocity = new Vector2(-1f, .2f) * projectile.GetProjectileSpeed();

                if (!GetComponent<MakeProjectileRotate>())
                {
                    if (transform.rotation.eulerAngles.z != 90f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }

                yield return new WaitForSeconds(intervalMovement);

                Destroy(gameObject);
            }
            else
            {
                break;
            }
        }
    }
}
