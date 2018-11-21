using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableAgentScript : Agent {

    public float fireRate = 2;
    public float speed = 5;
    private Vector3 velocity;
    private float lastFireTime;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time > lastFireTime + fireRate)
        {
            lastFireTime = Time.time;
            Teleport();
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (h < 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (v > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (v < 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 180);
        }

            

        velocity = new Vector3(h * Time.deltaTime * speed, v * Time.deltaTime * speed);

        transform.position += velocity;
    }

    public override bool GetStatus()
    {
        return base.GetStatus();
    }

    
}
