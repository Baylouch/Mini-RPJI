using UnityEngine;

[RequireComponent(typeof(Collider2D))] // To set as trigger to player interact with by collision
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public PlayerInteractionType interactionType;

    private bool isInteracting = false;
    public bool GetIsInteracting()
    {
        return isInteracting;
    }

    public virtual void Interact()
    {
        isInteracting = true;
    }  

    public virtual void UnInteract()
    {
        isInteracting = false;
    }
}
