using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StopPlayerWithTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player_Movement player_movement = collision.gameObject.GetComponent<Player_Movement>();
            if (player_movement)
            {
                player_movement.SetPlayerVelocity(Vector2.zero);
            }

            SetDarkKingCreditText darkKingCreditText = FindObjectOfType<SetDarkKingCreditText>();
            if (darkKingCreditText)
            {
                darkKingCreditText.ActiveTextPanel();
            }
        }
    }
}
