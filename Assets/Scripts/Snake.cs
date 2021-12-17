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

        if (GameManager.Instance.State != GameState.InPlay)
            return;

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
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RestartLevel();
        }
    }

    // Put physics related code here
    private void FixedUpdate()
    {

        if (doMove)
        {
            // if safe to grow!
            Debug.Log("Growing!");
            Debug.Log($"{this.transform.position}");

            
            float step = Time.deltaTime * 50;
            //transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
            //_rigidBody.MovePosition(targetPos);

            if (Vector2.Distance(transform.position, targetPos) > 0.001f)
            {
                //_rigidBody.velocity = new Vector2(_direction.x * step, _direction.y * step);
                _rigidBody.velocity = new Vector2(_direction.x * speed, _direction.y * speed);
                isMoving = true;
            }
            else
            {
                _rigidBody.velocity = Vector2.zero;
                Grow();
                GameManager.Instance.TilesRemaining--;
                doMove = false;
                isMoving = false;
            }

            //if (Vector2.Distance(transform.position, targetPos) < 0.001f)
            //{
            //    Grow();
            //    //this.transform.position = new Vector3(
            //    //Mathf.Round(this.transform.position.x) + _direction.x,
            //    //Mathf.Round(this.transform.position.y) + _direction.y,
            //    //0f
            //    //);



            //    GameManager.Instance.TilesRemaining--;
            //    doMove = false;
            //}

            //_rigidBody.MovePosition(new Vector2(_direction.x, _direction.y));
            //StartCoroutine(PerformMove());
                     
            
        }        
    }

    private IEnumerator PerformMove()
    {
        var origX = transform.position.x;
        var origY = transform.position.y;

        var targetX = origX + _direction.x;
        var targetY = origY + _direction.y;

        var currX = origX;
        var currY = origY;

        do
        {
            _rigidBody.MovePosition(new Vector2(currX, currY));

            currX = Mathf.Clamp(currX + Time.deltaTime, origX, targetX);
            currY = Mathf.Clamp(currY + Time.deltaTime, origY, targetY);

            yield return null;
        }
        while (currY != origY && currX != origX);

        _rigidBody.MovePosition(new Vector2(targetX, targetY));


    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = currPos;
        
        _segments.Add(segment);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            Debug.Log("Oof! Hit my own body!");
            GameManager.Instance.RestartLevel();
        }

        if (other.tag == "Wall")
        {
            Debug.Log("Dang! Hit a wall!");
            GameManager.Instance.RestartLevel();
        }

        if (other.tag == "Finish")
        {
            Debug.Log("Collected Star!");
            if (GameManager.Instance.IsLevelComplete())
            {
                Debug.Log("I did it! I AM EVERYWHERE!");
                GameManager.Instance.LoadNextLevel();
            }
            else
            {
                Debug.Log("Fail!, not everywhere!");
                GameManager.Instance.RestartLevel();
            }
            
        }

    }



}
