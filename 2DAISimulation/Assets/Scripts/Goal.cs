using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalStatus
{
    inactive,
    active,
    completed,
    failed
}

public enum GoalType
{
    goal_winGame,
    goal_travelEdge,
    goal_followPath,
    goal_moveToPosition,
    goal_plan,
    goal_collectItem,
    goal_avoidEnemy
}

public abstract class Goal<entity_type>
{

    protected entity_type owner;                    //owner of this goal

    protected GoalStatus my_Status;                 //status of the goal
    protected int my_Type;                          //type of the goal

    protected Goal(entity_type entity, int type)
    {
        owner = entity;
        my_Type = type;
        my_Status = GoalStatus.inactive;
    }


    public bool isActive()
    {
        if (my_Status == GoalStatus.active) return true;
        else return false;
    }

    public bool isInactive()
    {
        if (my_Status == GoalStatus.inactive) return true;
        else return false;
    }

    public bool isCompleted()
    {
        if (my_Status == GoalStatus.completed) return true;
        else return false;
    }

    public bool hasFailed()
    {
        if (my_Status == GoalStatus.failed) return true;
        else return false;
    }

    public GoalType Type()
    {
        return (GoalType)my_Type;
    }

    // abstract methods implementation
    // must be implemented in the child classes

    public abstract void Activate();

    public abstract GoalStatus Process();

    public abstract void Terminate();
}
