/* MakeItJumping.cs
 * 
 * Est utilisé pour faire sauter les ennemis dans le niveau des crédits.
 * 
 * */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MakeItJumping : MonoBehaviour
{
    [SerializeField] float jumpForce = 5f;

    Rigidbody2D myRb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        StartCoroutine(JumpIt());
    }

    IEnumerator JumpIt()
    {
        while (true)
        {
            float waitABit = Random.Range(0.2f, 1f);

            yield return new WaitForSeconds(waitABit);

            Vector2 forceToApply = new Vector2(0f, 1f) * Random.Range(jumpForce - 1f, jumpForce + 1f);

            int randomNumb = Random.Range(0, 2); // 2 is exclusive

            if (randomNumb == 0)
                anim.SetTrigger("LeftToRight");
            else
                anim.SetTrigger("RightToLeft");

            myRb.velocity = forceToApply;

            float timeToWait = Random.Range(0.15f, 0.4f);

            yield return new WaitForSeconds(timeToWait);

            myRb.velocity = -forceToApply;

            yield return new WaitForSeconds(timeToWait);

            myRb.velocity = Vector2.zero;
        }
    }
}
