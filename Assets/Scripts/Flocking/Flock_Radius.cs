using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_Radius : MonoBehaviour {

    public Boid boid;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Movable>() && other.gameObject.name != "Unit Body")
        {
            // print(Vector3.Angle(transform.forward, transform.position - other.transform.position));
            if (other.GetComponent<Movable>() && other.gameObject.name != "Unit Body")
                boid.neighbors.Add(other.GetComponent<Movable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Movable>() && other.gameObject.name != "Unit Body")
            if(boid.neighbors.Count > 0)
                boid.neighbors.Remove(other.GetComponent<Movable>());
    }
}
