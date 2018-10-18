using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Check : MonoBehaviour {

    public GameObject leader;
    private string ignore_tag = "boid";
    public bool is_colliding = false;

    private void OnTriggerStay(Collider other)
    {
        if(is_colliding != true)
            if(other.tag != ignore_tag)
                if(other.gameObject != leader)
                    is_colliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        is_colliding = false;
    }



}
