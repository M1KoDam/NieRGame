using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDestroying : MonoBehaviour
{
    private bool _isActive;
    
    public void Activate()
    {
        foreach(var childBody in GetComponentsInChildren<Rigidbody2D>())
        {
            childBody.bodyType = RigidbodyType2D.Dynamic;
            childBody.mass = 5f;
            var scene = SceneManager.GetActiveScene();
            if (scene.name == "FlightTop")
            {
                childBody.gravityScale = 0;
                _isActive = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        if (((Vector2)transform.localScale).magnitude >= 0.01f)
            transform.localScale -= new Vector3(0.005f, 0.005f, 0);
    }
}
