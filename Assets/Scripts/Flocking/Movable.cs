using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{

    public float max_accel, max_vel, accel_rate, velocity, accel;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    protected void Move()
    {

        transform.position = transform.position +
            transform.forward * velocity * Time.deltaTime +
            transform.forward * accel / 2 * Time.deltaTime * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        velocity += accel * Time.deltaTime;
        accel += accel_rate * Time.deltaTime;

        accel = Mathf.Clamp(accel, -max_accel, max_accel);
        velocity = Mathf.Clamp(velocity, -max_vel, max_vel);

    }
}
