using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEmergent : MonoBehaviour
{
	public GameObject head;
	public float moveSpeed = 1;
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
	public float raycastDistance = 1f;
	public float goalRotatePower = 7f;
	public float avoidRotatePower = 32f;
	public float visionDegrees = 90f;
	public GameObject groupParent;
	private List<Transform> group = null;
	private GameObject goal = null;

	public bool active = true;

	void Start()
	{
		Physics.IgnoreLayerCollision(9, 0);

		Transform[] temp = groupParent.GetComponentsInChildren<Transform>();
		for (int i = 0; i < temp.Length; i++)
		{
			//if (
		}
	}

	void Update()
	{
		if (!active)
			return;

		head.transform.position = transform.position + new Vector3(0, .3f, 0);
	}

	void FixedUpdate()
	{
		if (!active)
			return;

		//set goal

		List<Transform> followable = new List<Transform>();

		/*for (int i = 0; i < group.Length; i++)
		{
			if (inVision(group[i])
		}*/

		//rotation

		steer();
		head.transform.rotation = transform.rotation;

		//movement
		
		standardMove();

		//y axis

		transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);
	}

	bool inVision(Transform unit)
	{
		if (Vector3.Angle((unit.position - transform.position).normalized, transform.forward) < visionDegrees)
			return true;
		return false;
	}

	void steer()
	{
		Quaternion goalRotation = Quaternion.LookRotation(transform.position);
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
		bool a = Physics.Raycast(rayA, out hitA, raycastDistance, selfMask);
		rayDisplay(lineA, rayA.origin, rayA.origin + rayA.direction * raycastDistance, a);

		Ray rayB = new Ray(transform.position + transform.right * .25f, transform.forward);
		RaycastHit hitB = new RaycastHit();
		bool b = Physics.Raycast(rayB, out hitB, raycastDistance, selfMask);
		rayDisplay(lineB, rayB.origin, rayB.origin + rayB.direction * raycastDistance, b);

		Ray rayC = new Ray(transform.position, (transform.forward - transform.right * .15f));
		RaycastHit hitC = new RaycastHit();
		bool c = Physics.Raycast(rayC, out hitC, raycastDistance, selfMask);
		rayDisplay(lineC, rayC.origin, rayC.origin + rayC.direction * raycastDistance, c);

		Ray rayD = new Ray(transform.position, (transform.forward + transform.right * .15f));
		RaycastHit hitD = new RaycastHit();
		bool d = Physics.Raycast(rayD, out hitD, raycastDistance, selfMask);
		rayDisplay(lineD, rayD.origin, rayD.origin + rayD.direction * raycastDistance, d);

		//decision logic

		if (!a && !b && !c && !d)
			return 0;

		if (a && b && c && d)
		{
			return Vector3.SignedAngle(transform.forward, transform.position, transform.up) / 10;
		}

		float distanceA = Vector3.Distance(hitA.point, rayA.origin);
		float distanceB = Vector3.Distance(hitB.point, rayB.origin);
		float distanceC = Vector3.Distance(hitC.point, rayC.origin);
		float distanceD = Vector3.Distance(hitD.point, rayD.origin);

		if (c && d)
		{
			if (distanceC < distanceD)
				return Mathf.Clamp(raycastDistance / distanceC, 0, 5);
			else
				return Mathf.Clamp(raycastDistance / -distanceD, -5, 0);
		}
		if (c)
			return Mathf.Clamp(raycastDistance / distanceC, 0, 5);
		if (d)
			return Mathf.Clamp(raycastDistance / -distanceD, -5, 0);

		if (a && b)
		{
			if (distanceA < distanceB)
				return Mathf.Clamp(raycastDistance / distanceA, 0, 5);
			else
				return Mathf.Clamp(raycastDistance / -distanceB, -5, 0);
		}
		if (a)
			return Mathf.Clamp(raycastDistance / distanceA, 0, 5);
		if (b)
			return Mathf.Clamp(raycastDistance / -distanceB, -5, 0);

		return 0;
	}

	void standardMove()
	{
		velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
		transform.position += velocity * Time.deltaTime;
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
