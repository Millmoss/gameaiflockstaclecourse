using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLeaderPath : MonoBehaviour
{
	public GameObject pathGoal;
	private ArrayList pathGoals;
	public int pathIndex;
	public GameObject head;
	public float moveSpeed = 1;
	private Vector3 movement;
	private float t = 0;
	public GameObject radius;
	public string unitName;
	public UIHandler uiReceive;
	public Vector3 velocity;
	public float acceleration;
	public float goalRotatePower = .05f;

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

		if (Vector3.Distance(((GameObject)pathGoals[pathIndex]).transform.position, transform.position) < .3f)
		{
			velocity = Vector3.Lerp(velocity, Vector3.zero, .1f);
			transform.position += velocity * Time.deltaTime;
		}
		else
		{
			velocity += transform.forward * acceleration * Time.deltaTime;
			velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
			transform.position += velocity * Time.deltaTime;
		}

		//y axis

		transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);
	}

	void steer()
	{
		if (pathGoals.Count <= pathIndex)
			return;

		Quaternion goalRotation = Quaternion.LookRotation(((GameObject)pathGoals[pathIndex]).transform.position - transform.position);

		transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, goalRotatePower * Time.deltaTime);
	}

	public void pathContinue()	//call this when flock has reached current path goal
	{
		if (pathGoals.Count > pathIndex)
			pathIndex++;
	}

	void radiusDisplay(float rad, Vector3 pos)
	{
		radius.transform.position = pos;
		radius.transform.localScale = new Vector3(rad, .05f, rad);
	}
}
