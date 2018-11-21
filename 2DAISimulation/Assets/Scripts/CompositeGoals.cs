using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_WinGame : GoalComposite<AIAgentScript>
{
    public Goal_WinGame(AIAgentScript entity) : base(entity, (int)GoalType.goal_winGame) { }

    public override void Activate()
    {
        my_Status = GoalStatus.active;

        //plan the general overall solution
        //decide which goal is the most desired goal
        if (GameManager.instance.GetItemAmount() > 0)
            AddSubgoal(new Goal_CollectItem(owner));
    }


    public override GoalStatus Process()
    {
        //call active if it's not currently active
        if (my_Status == GoalStatus.inactive) Activate();

        //check if there is enemy closeby
        EnemyScript enemy = owner.hasEnemyAround();
        if (enemy != null && my_Subgoals.Peek().Type() != GoalType.goal_avoidEnemy)
        {
            AddSubgoal(new Goal_AvoidEnemy(owner, enemy));
        }

        my_Status = ProcessSubgoals();

        // if we have more to collect we reactivate the goal
        if (my_Status == GoalStatus.completed && GameManager.instance.GetItemAmount() != 0)
            Activate();
        

        return my_Status;
    }

    public override void Terminate()
    {
        Debug.Log("Terminate WinGame");
    }
}

// Collect item 
public class Goal_CollectItem : GoalComposite<AIAgentScript>
{
    int itemIndex = 0;
    Vector3 itemLocation;

    public Goal_CollectItem(AIAgentScript entity) : base(entity, (int)GoalType.goal_collectItem) { }

    public override void Activate()
    {
        // get the cloest item destination
        // then we add a subgoal moveToPosition to that location
        itemIndex = GameManager.instance.GetCloestItem(owner.transform.position);
        Debug.Log(itemIndex);
        if (itemIndex != -1)
        {
            itemLocation = GameManager.instance.GetItemLocation(itemIndex);
            AddSubgoal(new Goal_MoveToPosition(owner, itemLocation));
        }
    }

    public override GoalStatus Process()
    {
        //call active if it's not currently active
        if (my_Status == GoalStatus.inactive) Activate();

        my_Status = ProcessSubgoals();

        if (my_Status == GoalStatus.failed) Activate();

        return my_Status;
    }

    public override void Terminate()
    {
        Debug.Log("Terminate CollectItem");
    }
}

public class Goal_AvoidEnemy : GoalComposite<AIAgentScript>
{
    EnemyScript enemyRef;

    public Goal_AvoidEnemy(AIAgentScript entity, EnemyScript enemy) : base(entity, (int)GoalType.goal_avoidEnemy)
    {
        enemyRef = enemy;
    }

    public override void Activate()
    {
        my_Status = GoalStatus.active;

        //find a cloest alcove and hide 
        //enemy walking towards me
        // if the agent is currently in a alcove, we do nothing
        Vector3 dest = GameManager.instance.GetClosestAlcovePoint(owner.transform);
        AddSubgoal(new Goal_MoveToPosition(owner, dest));
        Debug.Log(dest);
    }

    public override GoalStatus Process()
    {
        //call active if it's not currently active
        if (my_Status == GoalStatus.inactive) Activate();

        //avoid enemy
        //if the enemy is walking towards him, find the safe spot until he is leaving
        //if the enemy is walking away, make sure that the agent it not too close
        //if the enemy is far enough we successfully avoided him
        my_Status = ProcessSubgoals();

        if (my_Status == GoalStatus.completed && (owner.transform.position - enemyRef.transform.position).magnitude < owner.SafeDistance())
        {
            // wait for the enemy to leave
            my_Status = GoalStatus.active;
        }
        //Debug.Log(my_Status);
        return my_Status;
    }

    public override void Terminate()
    {
        Debug.Log("successfully escaped from the enemy");
    }
}


// Move to position by using A* pathfinding planner
public class Goal_MoveToPosition : GoalComposite<AIAgentScript>
{
    private Vector3 destination;

    public Goal_MoveToPosition(AIAgentScript entity, Vector3 dest) : base(entity, (int)GoalType.goal_moveToPosition)
    {
        destination = dest;
    }

    public override void Activate()
    {
        my_Status = GoalStatus.active;
        List<Vector3Int> path = new List<Vector3Int>();
        // plan a path from current position to the destination
        // adjust coord to integers
        Vector3Int adjustedPos = new Vector3Int(Mathf.RoundToInt(owner.gameObject.transform.position.x), Mathf.RoundToInt(owner.gameObject.transform.position.y), 0);
        owner.transform.position = adjustedPos;
        path = PathPlanner.instance.PlanPath(owner.transform, destination);

        // if we get a path
        if (path != null)
            AddSubgoal(new Goal_FollowPath(owner, path));
        else
        {
            Debug.Log("no path available");
            my_Status = GoalStatus.failed;
        }
    }

    public override GoalStatus Process()
    {
        if (my_Status == GoalStatus.inactive) Activate();

        my_Status = ProcessSubgoals();

        // reactive if failed
        if (my_Status == GoalStatus.failed) { Debug.Log("replan"); Activate(); }

        return my_Status;
    }

    public override void Terminate()
    {
        Debug.Log("Terminate MoveToPosition");
    }
}


public class Goal_FollowPath : GoalComposite<AIAgentScript>
{
    private List<Vector3Int> my_Path;

    public Goal_FollowPath(AIAgentScript entity, List<Vector3Int> path) : base(entity, (int)GoalType.goal_followPath)
    {
        my_Path = path; 
    }


    public override void Activate()
    {
        my_Status = GoalStatus.active;

        AddSubgoal(new Goal_TravelEdge(owner, my_Path[0]));
        my_Path.RemoveAt(0);
    }

    public override GoalStatus Process()
    {
        //call active if it's not currently active
        if (my_Status == GoalStatus.inactive) Activate();

        // if there is still subgoals left 
        my_Status = ProcessSubgoals();

        if (my_Status == GoalStatus.completed && my_Path.Count != 0)
        {
            Activate();
        }

        return my_Status;
    }

    public override void Terminate()
    {
        // There is nothing to do right now either
        Debug.Log("Terminate TravelPath");
    }
}

//we need to update the item information every step and if something changed we need to replan
//int x = GameManager.instance.GetCloestItem(owner.transform.position);
//if (x == -1) RemoveAllSubgoals();
//if (x != -1 && x != itemIndex)
//{
//    Debug.Log("change item target");
//    RemoveAllSubgoals();
//    Activate();
//}
