using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Two_Level : MonoBehaviour
{
    public GameObject pathGoal;
    private ArrayList pathGoals;
    private int pathIndex;
    public GameObject head;
    public float moveSpeed = 1;
    public string unitName;
    public UIHandler uiReceive;
    public Vector3 velocity;
    public float acceleration;
    public float goalDistance = 1f;
    public float goalRotatePower = 7f;
    private Rigidbody unitBody;

    public bool widthCheck = true;

    void Start()
    {
        Physics.IgnoreLayerCollision(9, 0);

        Transform[] pathTemp = pathGoal.GetComponentsInChildren<Transform>();
        pathGoals = new ArrayList();
        for (int i = 0; i < pathTemp.Length; i++)
        {
            if (pathTemp[i].gameObject.tag == "Pathing")
            {
                pathGoals.Add(pathTemp[i].gameObject);
            }
        }
        pathIndex = 0;
        
        unitBody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        head.transform.position = transform.position + new Vector3(0, .3f, 0);
    }

    void FixedUpdate()
    {
        //rotation

        steer();
        head.transform.rotation = transform.rotation;

        //movement
        
        standardMove();

        //y axis

        transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);

        //pathing logic

        if (pathGoals.Count > pathIndex && Vector3.Distance(((GameObject)pathGoals[pathIndex]).transform.position, transform.position) < goalDistance)
            pathIndex++;

        unitBody.velocity = Vector3.Lerp(unitBody.velocity, Vector3.zero, .7f);
    }

    void steer()
    {
        if (pathGoals.Count <= pathIndex)
            return;

        Quaternion goalRotation = Quaternion.LookRotation(((GameObject)pathGoals[pathIndex]).transform.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, goalRotatePower * Time.deltaTime);
    }

    void standardMove()
    {
        velocity += transform.forward * acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
        transform.position += velocity * Time.deltaTime;
    }
}
