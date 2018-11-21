using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIAgentScript : Agent {

    public double offset = 0.5;
    public float checkDistance = 10;
    GoalComposite<AIAgentScript> goal;
    GoalStatus my_Status;
    int[] my_WorldState = new int[4];
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private int my_SafeDistance = 8;
    private bool isBlockedByPlayer = false;

    private void Start()
    {
        // once it's spawned, the goal is set to win the game
        goal = new Goal_WinGame(this);
    }

    
    private void FixedUpdate()
    {
        // if the goal is not satisfied we keep trying
        if (my_Status != GoalStatus.completed)
            my_Status = goal.Process();
    }

    public void Move(Vector3 dest)
    {
        Vector3 diff = dest - transform.position;
        //adjust rotation
        if (diff.x > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (diff.x < 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (diff.y > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (diff.y < 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 180);
        }

        transform.position = Vector3.MoveTowards(transform.position, dest, Speed());  
    }

    
    public override bool GetStatus()
    {
        return base.GetStatus();
    }

    public float Speed()
    {
        return speed  * Time.deltaTime;
    }


    // check if there is enemy around it
    // if no enemy then return -1
    // otherwise return the enemy reference
    public EnemyScript hasEnemyAround()
    {
        int index = -1;
        float min = checkDistance;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;
        for (int i=0; i<enemies.Length; i++)
        {
            GameObject enemy = enemies[i];
            
            float dx = Mathf.Abs(enemies[i].transform.position.x - transform.position.x);
           
            if (transform.position.y < 8 && enemy.transform.position.y < 8)
            {
                if (dx < min)
                {
                    min = dx;
                    index = i;
                }
            }
            else if (transform.position.y > 8 && enemy.transform.position.y > 8)
            {
                if (dx < min)
                {
                    min = dx;
                    index = i;
                }
            }
        }
        if (index == -1) return null;
        

        return enemies[index].GetComponent<EnemyScript>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            
            isBlockedByPlayer = true;
        }
        else
        {
            isBlockedByPlayer = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            isBlockedByPlayer = false;
        }
    }

    public bool isBlocked()
    {
        
        return isBlockedByPlayer;
    }

    public float SafeDistance()
    {
        return my_SafeDistance;
    }

    
}
