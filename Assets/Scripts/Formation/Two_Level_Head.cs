using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Two_Level_Head : MonoBehaviour
{
    public List<GameObject> boids;
    private int line_size, offset_multi = 1;
    private float dist_from_head = 1f, dist_between = 0.6f;

    // Use this for initialization
    void Start()
    {
        Vector3 startpos = new Vector3();
        for(int i=0;i<boids.Count;i++)
        {
            startpos += boids[i].transform.position;
            boids[i].GetComponentInChildren<Two_Level_Boid>().form_head = this;
        }
        transform.position = startpos / boids.Count;


        Realign(3.2f);
        dist_from_head = -line_size/2;
    }

    public void Remove(GameObject ojb)
    {
        boids.Remove(ojb);
        Realign(3.2f / 12 * boids.Count);
    }

    public bool Realign(float goal_width)
    {
        int new_size = Mathf.FloorToInt(goal_width / .5f);
        if (new_size > 1)
            dist_between = .5f + goal_width % .5f / (new_size - 1);

        if (new_size == 0)
            return true;

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
                boids[j + i].GetComponentInChildren<Two_Level_Boid>().distance = dist_from_head + i / line_size + dist_between * i / line_size;
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
                boids[j + i].GetComponentInChildren<Two_Level_Boid>().end_pos = transform.position - transform.forward * dist_between *
                   boids[j + i].GetComponentInChildren<Two_Level_Boid>().distance + (j + offset_multi * 0.5f) * transform.right * dist_between;
                //print(boids.Count + " " + j + i);   
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
}
