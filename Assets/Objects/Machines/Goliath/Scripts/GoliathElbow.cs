using UnityEngine;

public class GoliathElbow : MonoBehaviour
{
    public bool attacks;

    [SerializeField] private int damage;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (attacks && collider.gameObject.CompareTag("Player"))
            collider.gameObject.GetComponent<Player>().GetDamage(damage, transform);
    }
}