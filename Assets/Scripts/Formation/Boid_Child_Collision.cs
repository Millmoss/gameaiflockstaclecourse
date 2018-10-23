using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Child_Collision : MonoBehaviour {

    public Boid_Formation bf;
    public Formation_Lead fl;

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "B")
        {
            if(bf) bf.kill();
            if (fl) fl.kill();
        }
    }
}
