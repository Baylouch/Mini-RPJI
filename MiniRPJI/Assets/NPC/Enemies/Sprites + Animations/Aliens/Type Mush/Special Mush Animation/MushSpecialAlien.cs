using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MushSpecialAlien : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Incrementator()
    {
        anim.SetInteger("Incrementator", anim.GetInteger("Incrementator") + 1);
    }
}
