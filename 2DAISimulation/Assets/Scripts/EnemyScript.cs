using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour, iTeleportable {

    public float speed;

    private BoxCollider2D visionCollider;
    private BoxCollider2D bodyCollider;
    private SpriteRenderer visionBox;

    private Vector3 spawnPos;
    private Vector3 direction;
    private Vector3 spawnDir;
    private bool reachWall = false;

    // Use this for initialization
    void Start () {
        visionCollider = transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
        visionBox = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

        spawnPos = transform.position;

        Respawn();
	}
	
	void Update()
    {

        if ((transform.position.x <= 24 && transform.position.x >= 22))
        {
            visionBox.enabled = false;
            visionCollider.enabled = false;
        }

        if ((transform.position.x > 24 || transform.position.x < 22))
        {
            visionCollider.enabled = true;
            visionBox.enabled = true;
        }

        if (transform.position.x < -5 || transform.position.x > 48)
        {
            //despawn and respawn 
            Respawn();
        }

        if (!reachWall)
        {
            if (transform.position.x >= 21 && direction == -Vector3.left)
            {
                RandomlyTurn();
            }
            else if (transform.position.x <= 24 && direction == Vector3.left)
            {
                RandomlyTurn();
            }
        }
    }

    private void Respawn()
    {
        transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, 0);

        if (transform.position.x == -5)
        {
            spawnDir = -Vector3.left;

        }
        else
        {
            spawnDir = Vector3.left;

        }

        direction = spawnDir;
    }

    private void RandomlyTurn()
    {
        reachWall = true;
        // randomly decide to continue or head back
        int ran = Random.Range(0, 2);
        if (ran == 1)
        {
            // change direction
            direction = new Vector3(-direction.x, direction.y, 0);
        }
    }

    void FixedUpdate () {
        transform.position += direction * speed * Time.deltaTime;

        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, direction.x * 90 - 90);
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void GetTeleported()
    {
        transform.position = spawnPos;
        Respawn();
    }

    virtual public Transform GetTransform()
    {
        return transform;
    }

}
