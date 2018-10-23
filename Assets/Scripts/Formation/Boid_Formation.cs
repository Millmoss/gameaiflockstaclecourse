using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Formation : MonoBehaviour {

    public float distance;
    public Formation_Lead form_head;



    public void kill()
    {
        gameObject.SetActive(false);
        form_head.Remove(transform.gameObject);
        //send kill upward to manager
    }
}
