using System.Collections;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] wayPoint;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
        transform.position = wayPoint[0].position;
    }


    private void Update()
    {
        anim.SetBool("active", canMove);

        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointIndex].position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoint[wayPointIndex].position) < 0.1f)
        {
            if (wayPointIndex == wayPoint.Length - 1 || wayPointIndex == 0)
                moveDirection = moveDirection * -1;

            wayPointIndex = wayPointIndex + moveDirection;
        }
    }

    
    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
