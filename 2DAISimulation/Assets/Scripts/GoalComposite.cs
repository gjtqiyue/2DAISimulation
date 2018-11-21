using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Composite goal which has subgoals to process 
 */
public abstract class GoalComposite<entity_type> : Goal<entity_type> {

    protected Stack<Goal<entity_type>> my_Subgoals;

    protected GoalComposite(entity_type entity, int type) : base(entity, type)
    {
        my_Subgoals = new Stack<Goal<entity_type>>();
    }

    // Add subgoals to the list
    public void AddSubgoal(Goal<entity_type> goal)
    {
        my_Subgoals.Push(goal);
    }


    // Process all the gubgoals 
    public GoalStatus ProcessSubgoals()
    {
        // while there is still some tasks left, we remove any goals that has been completed or failed
        while (my_Subgoals.Count > 0  && (my_Subgoals.Peek().isCompleted() || my_Subgoals.Peek().hasFailed()))
        {
            my_Subgoals.Pop().Terminate();
        }
        // process the next goal
        if (my_Subgoals.Count > 0)
        {
            //get the status of the current goal
            GoalStatus status = my_Subgoals.Peek().Process();

            //to make sure that we keep processing goals after completed message is received
            if (status == GoalStatus.completed && my_Subgoals.Count > 1)
            {
                return GoalStatus.active;
            }

            return status;
        }

        //all the goal is competed
        else
        {
            return GoalStatus.completed;
        }
    }

    // Remove
    public void RemoveAllSubgoals()
    {
        for (int i=0; i < my_Subgoals.Count; i++)
        {
            Goal<entity_type> goal = my_Subgoals.Pop();
            goal.Terminate();
        }

        my_Subgoals.Clear();
    }

    // Not implemented for now
    public void HandleMessage()
    {

    }
   
}
