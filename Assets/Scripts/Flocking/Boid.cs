using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : Movable {
    
    public List<Movable> neighbors;
    public Vector3 seperation, cohesion, align, flock_center, flock_speed;

    protected void Update()
    {
        GetFlockStats();
        Seperation();
        Cohesion();
        Rotate();
        Move();
    }

    protected void ChildUpdate()
    {
        Update();
    }

    private void GetFlockStats()
    {
        //Flock center first.
        flock_center = Vector3.zero;
        flock_speed = Vector3.zero;
        for (int i = 0; i < neighbors.Count; i++)
        {
            flock_center += neighbors[i].transform.position;
            flock_speed += neighbors[i].transform.forward;
        }
        if (flock_center != Vector3.zero)
        { 
            flock_center /= neighbors.Count;
        }
        if(flock_speed != Vector3.zero)
            flock_speed /= neighbors.Count;
    }

    //Move towards the center of the local flock.
    private void Cohesion()
    {
        if (neighbors.Count == 0)
        {
            cohesion = Vector3.zero;
            return;
        }

        Vector3 direction = flock_center - transform.position;
        cohesion = direction.normalized;
        print(cohesion);
        
    }

    private void Seperation()
    {
        seperation = Vector3.zero;
        for (int i = 0; i < neighbors.Count; i++)
        {
            seperation += Seperation(i);
        }
        seperation = seperation.normalized;

    }

    private Vector3 Seperation(int pos)
    {
        Vector3 direction = transform.position - neighbors[pos].transform.position;
        float distance = direction.magnitude;

        float strength = Mathf.Min(0.5f / (distance * distance), max_vel);

        //print((strength * direction).normalized);
        return (strength * direction);

    }

    private void Rotate()
    {

        Vector3 summed_rotation = 1f * seperation + 1.35f * cohesion + 0.80f *flock_speed;
        summed_rotation = summed_rotation.normalized;
        if(summed_rotation != Vector3.zero)
        { 
            Quaternion end_rotation = Quaternion.LookRotation(
                summed_rotation,
                transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, end_rotation, 5 * Time.deltaTime);
        }
        //print(end_rotation.eulerAngles);

    }
}
