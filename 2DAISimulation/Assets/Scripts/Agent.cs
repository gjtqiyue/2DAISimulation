using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour, iTeleportable {

    protected int score = 0;
    protected bool isDead = false;
    protected int trapLeft = 2;

    virtual public void GetTeleported()
    {
        //choose one alcove and spawn there
        int x = Random.Range(0, 10);
        Vector3 newPos = GameManager.instance.GetAlcoveLocation(x);
        transform.position = newPos;
    }

    virtual public void Teleport()
    {
        if (trapLeft > 0)
        {
            //get the cloest target
            Transform target = GameManager.instance.GetCloestTeleportable(transform);
            target.gameObject.GetComponent<iTeleportable>().GetTeleported();
        }

        trapLeft -= 1;
    }

    virtual public Transform GetTransform()
    {
        return transform;
    }

    virtual public void Score()
    {
        score++;
    }

    virtual public int GetScore()
    {
        return score;
    }

    virtual public void Die()
    {
        isDead = true;
    }

    virtual public bool GetStatus()
    {
        return isDead;
    }

    virtual public bool hasTrapLeft()
    {
        if (trapLeft <= 0)
            return false;
        else
            return true;
    }
}
