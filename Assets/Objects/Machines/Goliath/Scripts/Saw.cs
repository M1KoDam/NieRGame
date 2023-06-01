using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private int damage;
    public float speed;

    private void FixedUpdate()
    {
        transform.Rotate(-Vector3.forward, speed);
    }

    private void OnCollisionStay2D(Collision2D collsion)
    {
        if (collsion.gameObject.CompareTag("Player"))
            collsion.gameObject.GetComponent<Player>().GetDamage(damage, transform);
    }
}