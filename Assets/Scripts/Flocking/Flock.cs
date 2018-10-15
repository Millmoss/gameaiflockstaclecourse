using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public List<GameObject> friends;
    public GameObject head;
    private Vector3 seperation_velocity = new Vector3(0f, 0f, 0f),
        cohesion_velocity = new Vector3(0f, 0f, 0f);
    
    //max_prediction must be nonzero
    public float max_velocity = 10f, max_accel = 5f;
    public Vector3 target_position;
    public float threshhold = 4.25f, pref_magnitude_btn_boids = 0.25f;
    private Vector3 flock_center, flock_velocity, seperation_velocity_tmp = Vector3.zero;
    private Vector3 accel;
    public Vector3 velocity;
    private Quaternion end_rotation;
    private float strength;
    public Vector3 ddd;

    private void Start()
    {
        //friends.Add(head);
    }

    private void Update()
    {
       // transform.position = new Vector3(transform.position.x,0.35f,transform.position.z);
        GetFlockCenter();
        GetFlockVelocity();
        Seperation();
        Cohesion();
        Move();
    }

    private void GetFlockCenter()
    {
        flock_center = Vector3.zero;
        for(int i=0;i<friends.Count;i++)
        {
            flock_center += friends[i].transform.position;
        }
        if(flock_center != Vector3.zero)
            flock_center /= friends.Count;
    }

    private void GetFlockVelocity()
    {
        flock_velocity = Vector3.zero;
        for (int i = 0; i < friends.Count; i++)
        {
         //   flock_velocity += friends[i].GetComponent<Movable>().velocity;
        }
        if (flock_velocity != Vector3.zero)
            flock_velocity /= friends.Count;
    }

    private void Seperation()
    {
        seperation_velocity_tmp = Vector3.zero;
        for (int i=0;i < friends.Count; i++)
        {
            Seperation(i);
        }
        seperation_velocity = seperation_velocity_tmp;
        if(seperation_velocity!=Vector3.zero)
            seperation_velocity /= friends.Count;
        
    }

    private void Seperation(int pos)    
    {
        Vector3 direction = -friends[pos].transform.position + transform.position;
        float distance = direction.magnitude;
        // distance = Mathf.Clamp(distance, 0, threshhold);
        //strength = max_velocity * (threshhold - distance) / threshhold;

        strength = Mathf.Min(threshhold / (distance * distance), max_velocity);
        //print((direction).magnitude);
     

        if((direction).magnitude < pref_magnitude_btn_boids)
            seperation_velocity_tmp += strength * direction;

        

    }

    //Move towards the target.
    protected void Move()
    {
        velocity = ddd;

        //velocity /= 2;

        //Vector3 end_velocity = cohesion_velocity;
        //Cap velocity and acceleration.
        if (velocity.magnitude > max_velocity)
        {
            velocity = velocity.normalized;
            velocity *= max_velocity;
        }

        print(transform.name+" "+ seperation_velocity + " " + cohesion_velocity + " " + flock_velocity+" "+velocity);

        Quaternion end_rotation = Quaternion.LookRotation((seperation_velocity + cohesion_velocity + (flock_velocity + flock_center - transform.position)).normalized, 
            transform.up);

        transform.position = transform.position + transform.rotation.normalized * velocity * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, end_rotation, 5 * Time.deltaTime);
        //Move
        //transform.position = transform.position +
            //velocity * Time.deltaTime;
        
    }

    //Move towards the center of the local flock.
    private void Cohesion()
    {
        if(friends.Count == 0)
        {
            cohesion_velocity = Vector3.zero;
            return;
        }
        Vector3 direction = -flock_center + transform.position;
        float target_speed;
        float distance = direction.magnitude;
        if (distance < pref_magnitude_btn_boids)
            target_speed = 0;

        else
            target_speed = max_velocity * (distance - pref_magnitude_btn_boids )/ pref_magnitude_btn_boids;

        //print(target_speed);

        cohesion_velocity = direction;
        cohesion_velocity = cohesion_velocity.normalized;
        cohesion_velocity *= -target_speed;



    }


}
