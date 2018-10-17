using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation_Lead : MonoBehaviour {

    public GameObject boid;
    public List<GameObject> boids;
    private int line_size, offset_multi = 1;

	// Use this for initialization
	void Start () {
		for(int i=0;i < 12;i++)
        {
            GameObject go = Instantiate(boid);
            boids.Add(go);
        }
        Realign(4);
	}

    public bool Realign(int new_size)
    {
        if(boids.Count % new_size != 0)
            return false;
        line_size = new_size;
        if (line_size % 2 == 0)
        {
            offset_multi = 1;
        }
        else
            offset_multi = 0;
        Formate();
        return true;
    }

    private void Formate()
    {
        for (int i = 0; i < boids.Count; i+=line_size)
        {
            for(int j=0;j < line_size && j + i < boids.Count; j++)
            {
                boids[j + i].GetComponent<Boid_Formation>().distance = 2 + i;
            }
            
        }
    }
	
	// Update is called once per frame
	void Update () {
		for(int i=line_size / 2 ;i < boids.Count;i+=line_size)
        {
            for(int j = -line_size / 2; j < line_size / 2 + 1 && i + j < boids.Count;j++)
            {
                boids[j + i].transform.position = transform.position + transform.forward *
                   boids[j + i].GetComponent<Boid_Formation>().distance + (j + offset_multi * 0.5f) * transform.right;
               boids[j + i].transform.rotation = transform.rotation;
            }
        }

       
	}

	public float getFormationWidth()
	{
		return line_size;
	}

	public List<GameObject> getBoids()
	{
		return boids;
	}

	public bool getLeft(float dist)
	{
		if (true)//can move left by dist
			return true;

		return false;
	}

	public bool getRight(float dist)
	{
		if (true)//can move right by dist
			return true;

		return false;
	}
}
