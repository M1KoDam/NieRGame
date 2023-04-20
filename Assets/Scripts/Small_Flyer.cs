using UnityEngine;

public class Small_Flyer : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    public Transform[] moveSpot;
    public int idCurMoveSpot;
    
    public float waitTime;
    private float _time;
    
    private const float PatrolSpeed = 3;
    private Vector3 _velocity;
    private float _angle;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _time = waitTime;
    }
    
    private void Wait()
    {
        _rb.velocity = new Vector2(0, 0.2f);
    }

    // Update is called once per frame
    private void Update()
    {
        _rb.MoveRotation(_angle);
        _rb.velocity = _velocity;
    }

    private void FixedUpdate()
    {
        Patroling();
    }

    private void Patroling()
    {
        if (Vector2.Distance(transform.position, moveSpot[idCurMoveSpot].transform.position) < 0.2f)
        {
            if (_time <= 0)
                ChangeSpotId();
            else
            {
                _time -= Time.deltaTime;
                Wait();
            }
        }

        else
            GoToSpot();
    }

    private void ChangeSpotId()
    {
        idCurMoveSpot++;
        
        if (idCurMoveSpot >= moveSpot.Length)
            idCurMoveSpot = 0;

        _time = waitTime;
    }

    private void GoToSpot()
    { 
        _velocity = (moveSpot[idCurMoveSpot].transform.position - _rb.transform.position).normalized * PatrolSpeed;
        //_angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
    }
}
