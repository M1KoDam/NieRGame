using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;

    private bool _onFoot = true;
    private string _currentAnimation;

    private const float Speed = 10;
    private const float JumpForce = 1200;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private static float MovementAxis => Input.GetAxis("Horizontal");
    private Side _faceOrientation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (MovementAxis != 0)
        {
            _faceOrientation = MovementAxis > 0 ? Side.Right : Side.Left;
            Move();
        }
        
        else
        {
            Stay();
        }

        Flip();

        if (_onFoot && Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Move()
    {
        ChangeAnimation("Move_anim");
        _rb.velocity = new Vector2(MovementAxis * Speed, _rb.velocity.y);
    }

    private void Stay()
    {
        ChangeAnimation("Idle_anim");
    }

    private void Flip()
    {
        transform.localScale = _faceOrientation == Side.Right ? RightLocalScale : LeftLocalScale;
    }

    private void Jump()
    {
        _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        _onFoot = false;
    }

    private void ChangeAnimation(string anim)
    {
        if (_currentAnimation == anim)
            return;

        _currentAnimation = anim;
        _animator.Play(anim);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = true;
        }
    }
}