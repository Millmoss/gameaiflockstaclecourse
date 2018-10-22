using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation_Lead : MonoBehaviour {

    public GameObject boid;
    public List<GameObject> boids;
    private int line_size, offset_multi = 1;
    private float dist_from_head = 1, dist_between = 0.5f;
    public GameObject check_collision;

	// Use this for initialization
	void Start () {
		for(int i=0;i < 12;i++)
        {
            GameObject go = Instantiate(boid);
            boids.Add(go);
        }
        Realign(3);
        check_collision.GetComponent<Collision_Check>().leader = gameObject;
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
                boids[j + i].GetComponent<Boid_Formation>().distance = dist_from_head + i / line_size + dist_between * i / line_size;
            }
            
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		getRight(10);
		getLeft(10);
        for (int i=line_size / 2 ;i < boids.Count;i+=line_size)
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
		check_collision.transform.position = transform.position + transform.right * dist / 2
			+ transform.forward * (boids.Count / 2 / line_size + dist_between * boids.Count / 2 / line_size);

		check_collision.transform.rotation = transform.rotation;

		check_collision.transform.localScale = new Vector3(dist, 1, boids.Count / 2);

		return check_collision.GetComponent<Collision_Check>().is_colliding;
	}

	public bool getRight(float dist)
	{
		check_collision.transform.position = transform.position - transform.right * dist / 2
			+ transform.forward * (boids.Count / 2 / line_size + dist_between * boids.Count / 2 / line_size);

		check_collision.transform.rotation = transform.rotation;

		check_collision.transform.localScale = new Vector3(dist, 1, boids.Count / 2);

		return check_collision.GetComponent<Collision_Check>().is_colliding;
    }
}
