using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool active;

    //[SerializeField] private bool canBeReactivated;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active) // if checkpoint already active
            return; 

        Player player = collision.GetComponent<Player>();

        if (player != null)
            ActivateCheckpoint();
    }


    private void ActivateCheckpoint()
    {
        active = true;
        anim.SetTrigger("activate");
        GameManager.instance.UpdateRespawnPosition(transform);
    }
}
