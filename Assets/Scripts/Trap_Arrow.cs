using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Additional info")]
    [SerializeField] private float cooldown;
    [SerializeField] private bool rotationRight;
    [SerializeField] private float rotationSpeed = 120;
    private int direction = -1;


    private void Update()
    {
        direction = rotationRight ? -1 : 1;

        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }


    private void DestroyMe() 
    {
        GameObject arrowPrefab = GameManager.instance.arrowPreFab;

        GameManager.instance.CreateObject(arrowPrefab, transform, cooldown);

        Destroy(gameObject);
    } 
}
