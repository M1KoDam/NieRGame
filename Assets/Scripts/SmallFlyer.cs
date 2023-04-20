using UnityEngine;

public class SmallFlyer : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    public Transform[] moveSpot;
    public int curId;
    
    public float waitTime;
    private float _time;
    
    private const float PatrolSpeed = 3;
    private Vector3 _velocity;
    private float _angle;

    private Side FaceOrientation
        => _velocity.x > 0
                ? Side.Right
                : Side.Left;
    
    private static readonly Vector3 RightLocalScale = new(-1, 1);
    private static readonly Vector3 LeftLocalScale = new(1, 1);
    
    private Vector2 SmallFlyerToTarget => moveSpot[curId].transform.position - _rb.transform.position;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _time = waitTime;
    }
    
    private void Wait()
    {
        _velocity = new Vector2(0, 0.2f);
        if (_angle != 0)
            _angle /= 2;
        if (_angle < 1f)
            _angle = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;
        
        _rb.MoveRotation(_angle);
        _rb.velocity = _velocity;
    }

    private void FixedUpdate()
    {
        Patrolling();
    }

    private void Patrolling()
    {
        if (SmallFlyerToTarget.magnitude < 0.2f)
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
        curId++;
        
        if (curId >= moveSpot.Length)
            curId = 0;

        _time = waitTime;
    }

    private void GoToSpot()
    { 
        _velocity = (moveSpot[curId].transform.position - _rb.transform.position).normalized * PatrolSpeed;
        var strongVector = FaceOrientation == Side.Left ? Vector2.left : Vector2.right;
        _angle = -Vector2.SignedAngle(SmallFlyerToTarget, strongVector);
    }
}
