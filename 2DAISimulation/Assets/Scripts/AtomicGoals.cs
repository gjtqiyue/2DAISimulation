using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Move one grid closer to the destination
 * 
 * */
public class Goal_TravelEdge : Goal<AIAgentScript>
{

    // For more complex terrain with edge in it, we can use edge class to hold more information about each specific edge.
    // Right now, we only need to use a destination variable
    private Vector3Int destination;
    bool isBlocked;

    public Goal_TravelEdge(AIAgentScript owner, Vector3Int dest) : base(owner, (int)GoalType.goal_travelEdge)
    {
        destination = dest;
        isBlocked = false;
    }


    public override void Activate()
    {
        my_Status = GoalStatus.active;
    }

    public override GoalStatus Process()
    {
        //Debug.DrawLine(owner.transform.position, destination, Color.red);
        //Debug.Log(destination);
        //call active if it's not currently active
        if (my_Status == GoalStatus.inactive) Activate();

        //if the edge is not reachable we have to replan
        if (Vector3.Distance(owner.transform.position, destination) > 1.5f)
        {
            Debug.Log("cannot reach the node");
            my_Status = GoalStatus.failed;
            return my_Status;
        }

        // follow the edge
        owner.Move(destination);


        // check if get blocked by player
        CheckIfStuck();

        if (owner.transform.position == destination) my_Status = GoalStatus.completed;

        return my_Status;
    }

    public override void Terminate()
    {
        // right now we don't have anything need to do
    }


    // If the agent is blocked by player, it will either teleport it or find another way around 
    private void CheckIfStuck()
    {
        //if it's blocked by a player
        isBlocked = owner.isBlocked();

        if (isBlocked == true)
        {
            int x = Random.Range(0, 10);
            if (x == 1)
                owner.Teleport();
            else
            {
                Debug.Log("blocked");
                my_Status = GoalStatus.failed;
            }
        }
    }
}
