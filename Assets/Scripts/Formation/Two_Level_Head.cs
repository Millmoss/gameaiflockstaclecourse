using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Two_Level_Head : MonoBehaviour
{
    public GameObject boid;
    public List<GameObject> boids;
    private int line_size, offset_multi = 1;
    private float dist_from_head = 1, dist_between = 0.5f;
    public GameObject check_collision;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject go = Instantiate(boid);
            boids.Add(go);
        }
        Realign(4);
        check_collision.GetComponent<Collision_Check>().leader = gameObject;
    }

    public bool Realign(int new_size)
    {
        if (boids.Count % new_size != 0)
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
        for (int i = 0; i < boids.Count; i += line_size)
        {
            for (int j = 0; j < line_size && j + i < boids.Count; j++)
            {
                boids[j + i].GetComponent<Two_Level_Boid>().distance = 1 + i;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = line_size / 2; i < boids.Count; i += line_size)
        {
            for (int j = -line_size / 2; j < line_size / 2 + 1 && i + j < boids.Count; j++)
            {
                boids[j + i].GetComponent<Two_Level_Boid>().end_pos = transform.position + transform.forward *
                   boids[j + i].GetComponent<Two_Level_Boid>().distance + (j + offset_multi * 0.5f) * transform.right;
                
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
        check_collision.transform.position = transform.position - transform.right * dist / 2
            + transform.forward * (boids.Count / 2 / line_size + dist_between * boids.Count / 2 / line_size);

        check_collision.transform.rotation = transform.rotation;

        check_collision.transform.localScale = new Vector3(dist, 1, boids.Count / 2);

        return check_collision.GetComponent<Collision_Check>().is_colliding;
    }

    public bool getRight(float dist)
    {
        check_collision.transform.position = transform.position + transform.right * dist / 2
            + transform.forward * (boids.Count / 2 / line_size + dist_between * boids.Count / 2 / line_size);

        check_collision.transform.rotation = transform.rotation;

        check_collision.transform.localScale = new Vector3(dist, 1, boids.Count / 2);

        return check_collision.GetComponent<Collision_Check>().is_colliding;
    }
}
