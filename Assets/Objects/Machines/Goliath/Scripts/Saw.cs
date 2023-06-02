using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private int damage;
    public float speed;

    private void FixedUpdate()
    {
        transform.Rotate(-Vector3.forward, speed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            collider.gameObject.GetComponent<Player>().GetDamage(damage, transform);
    }
}