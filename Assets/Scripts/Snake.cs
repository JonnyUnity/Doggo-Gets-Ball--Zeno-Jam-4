using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    public float speed = 10f;

    private Vector2 _direction = Vector2.right;

    private List<Transform> _segments;
    public Transform segmentPrefab;

    private Rigidbody2D _rigidBody;
    private Vector2 targetPos;
    private Vector2 currPos;
    private bool doMove = false;
    private bool isMoving;
    private bool isFirstMove = true;

    public bool TooSoon;
    public bool LevelComplete;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
    }


    void Update()
    {

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _direction = Vector2.up;
                doMove = true;
                currPos = transform.position;
                targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                _direction = Vector2.left;
                doMove = true;
                currPos = transform.position;
                targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _direction = Vector2.down;
                doMove = true;
                currPos = transform.position;
                targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                _direction = Vector2.right;
                doMove = true;
                currPos = transform.position;
                targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
            }
        

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.RestartLevel();
            }

            if (TooSoon)
            {
                GameManager.Instance.SetFailState("Too soon!");
            }
            if (LevelComplete)
            {
                GameManager.Instance.State = GameState.LevelEnding;
            }

            //// Check for level complete?
            //if (GameManager.Instance.IsLevelComplete())
            //{
              

            //    Debug.Log("Level Complete!");
            //    //LevelComplete = true;
                
            //}
            //else
            //{
            //    Debug.Log("Too Soon!");
            //    //TooSoon = true;
                
            //}

        }
    }


    // Put physics related code here
    private void FixedUpdate()
    {

        if (doMove)
        {
            // if safe to grow!
            //Debug.Log("Moving!");
            //currPos = transform.position;
            StartCoroutine(MoveDog());

            //if (Vector2.Distance(transform.position, targetPos) > 0.001f)
            //{
            //    _rigidBody.velocity = new Vector2(_direction.x * speed, _direction.y * speed);
            //    isMoving = true;
            //}
            //else
            //{
            //    _rigidBody.velocity = Vector2.zero;
            //    GameManager.Instance.TilesRemaining--;
            //    Grow();


            //    doMove = false;
            //    isMoving = false;
            //}
        }        
    }


    private IEnumerator MoveDog()
    {


        //targetPos = new Vector2(transform.position.x + _direction.x, transform.position.y + _direction.y);
        if (Vector2.Distance(transform.position, targetPos) > 0.001f)
        {
            _rigidBody.velocity = new Vector2(_direction.x * speed, _direction.y * speed);
            isMoving = true;
        }
        else
        {
            _rigidBody.velocity = Vector2.zero;
            GameManager.Instance.TilesRemaining--;
            Grow();


            doMove = false;
            isMoving = false;
        }

        yield return null;

        //while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        //{
        //    Debug.Log("Distance = " + Vector2.Distance(transform.position, targetPos) + " : " + transform.position + " : " + targetPos);
        //    _rigidBody.velocity = new Vector2(_direction.x * speed, _direction.y * speed);
        //    yield return null;
        //}


        //_rigidBody.velocity = Vector2.zero;
        //transform.position = targetPos;

        //GameManager.Instance.TilesRemaining--;

        //Grow();

        //doMove = false;
        //isMoving = false;
        //yield return 0;

    }



    private void Grow()
    {
        Transform segment;
        if (isFirstMove)
        {
            Debug.Log("FIRST MOVE!");
            segment = Instantiate(this.segmentPrefab);
            isFirstMove = false;
        }
        else
        {
            Debug.Log("ANOTHER MOVE!");
            segment = Instantiate(this.segmentPrefab);
        }
        segment.position = currPos;

        
        _segments.Add(segment);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Body"))
        {
            //Destroy(this);
            _rigidBody.velocity = Vector2.zero;
            GameManager.Instance.SetFailState("Oof! Hit my own body!");
        }

        if (other.CompareTag("Wall"))
        {
            //Destroy(this);
            _rigidBody.velocity = Vector2.zero;
            GameManager.Instance.SetFailState("Dang! Hit a wall!");
        }

        if (other.CompareTag("Finish"))
        {
            if (GameManager.Instance.IsLevelComplete())
            {
                Debug.Log("Level Complete!");
                LevelComplete = true;
                //GameManager.Instance.State = GameState.LevelEnding;
            }
            else
            {
                Debug.Log("Too Soon!");
                TooSoon = true;
                //GameManager.Instance.SetFailState("Too soon!");
            }
            
        }

    }



}
