using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private float _moveInput;
    public bool faceOrientationRight = true;
    private bool _onFoot = true;

    private Animator _animator;
    private string _currentAnimation;

    void Move()
    {
        _moveInput = Input.GetAxis("Horizontal");
        ChangeAnimation(_moveInput != 0 ? "Move_anim" : "Idle_anim");
        _rb.velocity = new Vector2(_moveInput * speed, _rb.velocity.y);
    }

    void Jump()
    {
        _rb.AddForce(new Vector2(0, jumpForce*100), ForceMode2D.Impulse);
        _onFoot = false;
    }

    void Flip()
    {
        if (_moveInput > 0 && !faceOrientationRight || _moveInput < 0 && faceOrientationRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceOrientationRight = !faceOrientationRight;
        }
    }

    void ChangeAnimation(string animation)
    {
        if (_currentAnimation == animation) return;
        _animator.Play(animation);
        _currentAnimation = animation;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Flip();
        if (Input.GetKeyDown(KeyCode.Space) && _onFoot)
            Jump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = true;
        }
    }
}