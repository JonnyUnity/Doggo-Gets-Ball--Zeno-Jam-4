using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{

    private Vector2 _direction = Vector2.right;

    private List<Transform> _segments;
    public Transform segmentPrefab;

    private bool doMove = false;

    void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
    }


    void Update()
    {

        if (GameManager.Instance.State != GameState.InPlay)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            _direction = Vector2.up;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = Vector2.left;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _direction = Vector2.down;
            doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = Vector2.right;
            doMove = true;
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
            Grow();

            
            this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0f
            );

            GameManager.Instance.TilesRemaining--;
            doMove = false;            
            
        }        
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = this.transform.position;
        
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
