using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPath : MonoBehaviour
{
	public float formationWidth = 1;		//represents width of formation in current state, unit width inclusive
	public GameObject pathGoal;
	private ArrayList pathGoals;
	private int pathIndex;
	public GameObject head;
	public float moveSpeed = 1;
	private float t = 0;
	public GameObject lineA;
	public GameObject lineB;
	public GameObject lineC;
	public GameObject lineD;
	public GameObject lineE;
	public GameObject lineF;
	public Material greenMat;
	public Material redMat;
	public string unitName;
	public UIHandler uiReceive;
	public Vector3 velocity;
	public float acceleration;
	public float goalDistance = 1f;
	public float goalRotatePower = 7f;
	public float avoidRotatePower = 32f;

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

		moveLogic();

		//y axis

		transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);

		//pathing logic

		if (pathGoals.Count > pathIndex && Vector3.Distance(((GameObject)pathGoals[pathIndex]).transform.position, transform.position) < goalDistance)
			pathIndex++;
	}

	void steer()
	{
		if (pathGoals.Count <= pathIndex)
			return;

		Quaternion goalRotation = Quaternion.LookRotation(((GameObject)pathGoals[pathIndex]).transform.position - transform.position);
		float rr = rayResultFront();
		Quaternion avoidRotation = transform.rotation * Quaternion.AngleAxis(rr * 4, Vector3.up);

		transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, goalRotatePower * Time.deltaTime);
		if (rr != 0)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, avoidRotation, avoidRotatePower * Time.deltaTime);
			transform.position += transform.right * rr * Time.deltaTime;
		}
	}

	float rayResultFront()   //returns negative for turn power left, positive for turn power right, zero for no change of course
	{
		LayerMask selfMask = 1 << 0;

		Ray rayA = new Ray(transform.position - transform.right * .25f, transform.forward);
		RaycastHit hitA = new RaycastHit();
		bool a = Physics.Raycast(rayA, out hitA, goalDistance, selfMask);
		rayDisplay(lineA, rayA.origin, rayA.origin + rayA.direction * goalDistance, a);

		Ray rayB = new Ray(transform.position + transform.right * .25f, transform.forward);
		RaycastHit hitB = new RaycastHit();
		bool b = Physics.Raycast(rayB, out hitB, goalDistance, selfMask);
		rayDisplay(lineB, rayB.origin, rayB.origin + rayB.direction * goalDistance, b);

		Ray rayC = new Ray(transform.position, (transform.forward - transform.right * .15f));
		RaycastHit hitC = new RaycastHit();
		bool c = Physics.Raycast(rayC, out hitC, goalDistance, selfMask);
		rayDisplay(lineC, rayC.origin, rayC.origin + rayC.direction * goalDistance, c);

		Ray rayD = new Ray(transform.position, (transform.forward + transform.right * .15f));
		RaycastHit hitD = new RaycastHit();
		bool d = Physics.Raycast(rayD, out hitD, goalDistance, selfMask);
		rayDisplay(lineD, rayD.origin, rayD.origin + rayD.direction * goalDistance, d);

		//decision logic

		if (!a && !b && !c && !d)
			return 0;

		if (a && b && c && d)
		{
			return Vector3.SignedAngle(transform.forward, ((GameObject)pathGoals[pathIndex]).transform.position - transform.position, transform.up) / 10;
		}

		float distanceA = Vector3.Distance(hitA.point, rayA.origin);
		float distanceB = Vector3.Distance(hitB.point, rayB.origin);
		float distanceC = Vector3.Distance(hitC.point, rayC.origin);
		float distanceD = Vector3.Distance(hitD.point, rayD.origin);

		if (c && d)
		{
			if (distanceC < distanceD)
				return Mathf.Clamp(goalDistance / distanceC, 0, 5);
			else
				return Mathf.Clamp(goalDistance / -distanceD, -5, 0);
		}
		if (c)
			return Mathf.Clamp(goalDistance / distanceC, 0, 5);
		if (d)
			return Mathf.Clamp(goalDistance / -distanceD, -5, 0);

		if (a && b)
		{
			if (distanceA < distanceB)
				return Mathf.Clamp(goalDistance / distanceA, 0, 5);
			else
				return Mathf.Clamp(goalDistance / -distanceB, -5, 0);
		}
		if (a)
			return Mathf.Clamp(goalDistance / distanceA, 0, 5);
		if (b)
			return Mathf.Clamp(goalDistance / -distanceB, -5, 0);

		return 0;
	}

	void moveLogic()
	{
		Vector3 resultLeft = rayResultLeft();
		Vector3 resultRight = rayResultRight();
		bool l = resultLeft != transform.position;
		bool r = resultRight != transform.position;

		if (!l && !r)
		{
			standardMove();
		}

		if (l && r)
		{
			float goalWidth = Vector3.Distance(resultLeft, resultRight);

			if (formationWidth < goalWidth)
			{

			}
		}
		

	}

	void standardMove()
	{
		velocity += transform.forward * acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
		transform.position += velocity * Time.deltaTime;
	}

	bool groupStatus()
	{
		return true;	//only if group isn't lagging
	}

	Vector3 rayResultLeft()
	{
		LayerMask selfMask = 1 << 0;

		Ray rayE = new Ray(transform.position, -transform.right);
		RaycastHit hitE = new RaycastHit();
		bool e = Physics.Raycast(rayE, out hitE, (formationWidth / 2 + .1f), selfMask);
		rayDisplay(lineE, rayE.origin, rayE.origin + rayE.direction * (formationWidth / 2 + .1f), e);

		if (e)
			return hitE.point;

		return transform.position;
	}

	Vector3 rayResultRight()
	{
		LayerMask selfMask = 1 << 0;

		Ray rayF = new Ray(transform.position, transform.right);
		RaycastHit hitF = new RaycastHit();
		bool f = Physics.Raycast(rayF, out hitF, (formationWidth / 2 + .1f), selfMask);
		rayDisplay(lineF, rayF.origin, rayF.origin + rayF.direction * (formationWidth / 2 + .1f), f);

		if (f)
			return hitF.point;

		return transform.position;
	}

	void rayDisplay(GameObject line, Vector3 origin, Vector3 end, bool didHit)
	{
		line.transform.position = (origin + end) / 2;
		line.transform.rotation = Quaternion.LookRotation(end - origin);
		line.transform.localScale = new Vector3(.05f, .05f, Vector3.Distance(origin, end));
		if (!didHit)
			line.GetComponent<MeshRenderer>().material = greenMat;
		else
			line.GetComponent<MeshRenderer>().material = redMat;
	}
}
