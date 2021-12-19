using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DogController : MonoBehaviour
{
    public float speed = 10f;

    private Vector2 _direction = Vector2.down;
    private Vector2 _previousDirection;

    public Transform segmentPrefab;
    public Transform backLegsPrefab;
    public Transform cornerPrefab;

    private Rigidbody2D _rigidBody;
    private Vector2 targetPos;
    private Vector2 currPos;
    private bool doMove = false;
    private bool isMoving;
    private bool isFirstMove = true;

    private bool TooSoon;
    private bool LevelComplete;

    // Sprites
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Sprite Head_Up;
    [SerializeField]
    private Sprite Head_Down;
    [SerializeField]
    private Sprite Head_Side;

    [SerializeField]
    private Sprite BackLegs_Side;
    [SerializeField]
    private Sprite Backlegs_Up;
    [SerializeField]
    private Sprite BackLegs_Down;

    private GameObject Dog_Neck;

    // Audio
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip walkSound;
    [SerializeField]
    private AudioClip BonkSound;
    [SerializeField]
    private AudioClip successSound;
    [SerializeField]
    private AudioClip[] footstepSounds;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        Dog_Neck = gameObject.transform.Find("Neck").gameObject;
    }

    void Update()
    {

        if (GameManager.Instance.State == GameState.InPlay)
        {


            if (!isMoving)
            {
                if (Input.GetKeyDown(KeyCode.W) && _previousDirection != Vector2.down)
                {
                    _direction = Vector2.up;
                    doMove = true;
                    currPos = transform.position;
                    targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
                }
                else if (Input.GetKeyDown(KeyCode.A) && _previousDirection != Vector2.right)
                {
                    _direction = Vector2.left;
                    doMove = true;
                    currPos = transform.position;
                    targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
                }
                else if (Input.GetKeyDown(KeyCode.S) && _previousDirection != Vector2.up)
                {
                    _direction = Vector2.down;
                    doMove = true;
                    currPos = transform.position;
                    targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
                }
                else if (Input.GetKeyDown(KeyCode.D) && _previousDirection != Vector2.left)
                {
                    _direction = Vector2.right;
                    doMove = true;
                    currPos = transform.position;
                    targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
                }

                if (_direction == Vector2.down)
                {
                    _spriteRenderer.sprite = Head_Down;
                }
                else if (_direction == Vector2.up)
                {
                    _spriteRenderer.sprite = Head_Up;
                }
                else if (_direction == Vector2.left)
                {
                    _spriteRenderer.sprite = Head_Side;
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _spriteRenderer.sprite = Head_Side;
                    _spriteRenderer.flipX = false;
                }


                if (LevelComplete)
                {
                    GameManager.Instance.CompleteLevel();
                }
                else if (TooSoon)
                {
                    GameManager.Instance.CollectedBallEarly();
                }
            }
        }
    }


    private void FixedUpdate()
    {

        if (doMove)
        {
            StartCoroutine(MoveDog());
        }        
    }


    private IEnumerator MoveDog()
    {
        if (Vector2.Distance(transform.position, targetPos) > 0.001f)
        {
            _rigidBody.velocity = new Vector2(_direction.x * speed, _direction.y * speed);
            isMoving = true;
        }
        else
        {
            PlayFootstepSound();
            _rigidBody.velocity = Vector2.zero;
            Dog_Neck.SetActive(false);
            GameManager.Instance.TilesRemaining--;
            Grow();

            _previousDirection = _direction;

            doMove = false;
            isMoving = false;
        }

        yield return null;
    }

    private void PlayFootstepSound()
    {
        int selected = Random.Range(0, footstepSounds.Length - 1);
        _audioSource.PlayOneShot(footstepSounds[selected]);

    }


    private void Grow()
    {
        Transform segment;
        if (isFirstMove)
        {
            segment = Instantiate(this.backLegsPrefab);
            var backLegsSpriteRenderer = segment.transform.GetComponent<SpriteRenderer>();

            if (_direction == Vector2.up)
            {
                backLegsSpriteRenderer.sprite = Backlegs_Up;
            }
            else if (_direction == Vector2.left)
            {
                backLegsSpriteRenderer.sprite = BackLegs_Side;
                backLegsSpriteRenderer.flipX = true;
            }
            else if (_direction == Vector2.right)
            {
                backLegsSpriteRenderer.sprite = BackLegs_Side;
                backLegsSpriteRenderer.flipX = false;
            }
            else if (_direction == Vector2.down)
            {
                backLegsSpriteRenderer.sprite = BackLegs_Down;
            }
            isFirstMove = false;
        }
        else
        {
            if (_direction != _previousDirection)
            {
                segment = Instantiate(this.cornerPrefab);

                // rotate based on turn
                if (_direction == Vector2.up)
                {
                    if (_previousDirection == Vector2.left)
                    {
                        segment.Rotate(0, 0, 180);
                    }
                    else if (_previousDirection == Vector2.right)
                    {
                        // orig
                        segment.Rotate(0, 0, 270);
                    }
                }
                else if (_direction == Vector2.down)
                {
                    if (_previousDirection == Vector2.left)
                    {
                        segment.Rotate(0, 0, 90);
                    }
                    else if (_previousDirection == Vector2.right)
                    {
                        //segment.Rotate(0, 0, 90);
                        //orig
                    }
                }
                else if (_direction == Vector2.left)
                {
                    if (_previousDirection == Vector2.up)
                    {
                        //segment.Rotate(0, 0, 90);
                        // orig
                    }
                    else if (_previousDirection == Vector2.down)
                    {
                        segment.Rotate(0, 0, 270);
                        // orig
                    }
                }
                else if (_direction == Vector2.right)
                {
                    if (_previousDirection == Vector2.up)
                    {
                        segment.Rotate(0, 0, 90);
                    }
                    else if (_previousDirection == Vector2.down)
                    {
                        segment.Rotate(0, 0, 180);
                        // orig
                    }
                }               
            }
            else
            {
                segment = Instantiate(this.segmentPrefab);
                if (_direction == Vector2.left || _direction == Vector2.right)
                {
                    segment.Rotate(0, 0, 90);
                }
            }            
        }
        segment.position = currPos;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Body"))
        {
            _rigidBody.velocity = Vector2.zero;
            _audioSource.PlayOneShot(BonkSound);
            GameManager.Instance.SetFailState("Oof! Hit my own body!");
        }

        if (other.CompareTag("Wall"))
        {
            _rigidBody.velocity = Vector2.zero;
            _audioSource.PlayOneShot(BonkSound);
            GameManager.Instance.SetFailState("Dang! Hit a wall!");
        }

        if (other.CompareTag("Finish"))
        {
            Destroy(other.gameObject);
            if (GameManager.Instance.IsLevelComplete())
            {
                _audioSource.PlayOneShot(successSound);
                LevelComplete = true;
            }
            else
            {
                TooSoon = true;
            }
            
        }

    }



}
