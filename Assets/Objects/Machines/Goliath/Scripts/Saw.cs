using UnityEngine;

public class Saw : MonoBehaviour
{
    public float speed;
    public bool attacks;
    
    [SerializeField] private int damage;

    private void FixedUpdate()
    {
        transform.Rotate(-Vector3.forward, speed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (attacks && collider.gameObject.CompareTag("Player"))
            collider.gameObject.GetComponent<Player>().GetDamage(damage, transform);
    }
}