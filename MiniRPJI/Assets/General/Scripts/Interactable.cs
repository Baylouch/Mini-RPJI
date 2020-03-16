using UnityEngine;

[RequireComponent(typeof(Collider2D))] // To set as trigger to player interact with by collision
public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log("Interacted with : " + gameObject.name);
    }

    
}
