using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Two_Level_Boid : MonoBehaviour
{
	public Vector3 end_pos = Vector3.zero;
	public float distance;
	public Two_Level_Head form_head;

	public bool isLeader = false;
	public GameObject head;
	public float moveSpeed = 2;
	public GameObject lineA;
	public GameObject lineB;
	public GameObject lineC;
	public GameObject lineD;
	public Material greenMat;
	public Material redMat;
	public string unitName;
	public UIHandler uiReceive;
	public Vector3 velocity;
	public float acceleration = 8f;
	public float raycastDistance = 1f;
	public float goalRotatePower = 7f;
	public float avoidRotatePower = 32f;
	private Rigidbody unitBody;

	public bool active = true;

	void Start()
	{
		Physics.IgnoreLayerCollision(9, 0);

		unitBody = gameObject.GetComponent<Rigidbody>();
		unitBody.freezeRotation = true;
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

		//rotation

		steer();
		head.transform.rotation = transform.rotation;

		//movement

		standardMove();

		//y axis

		transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);

		unitBody.velocity = Vector3.Lerp(unitBody.velocity, Vector3.zero, .7f);
	}

	void steer()
	{
		if (end_pos == Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.right), goalRotatePower * Time.deltaTime);
			return;
		}

		Quaternion goalRotation = Quaternion.LookRotation(end_pos - transform.position);
		float rr = rayResultFront();
		Quaternion avoidRotation = transform.rotation * Quaternion.AngleAxis(rr * 7, Vector3.up);

		transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, goalRotatePower * Time.deltaTime);
		if (rr != 0)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, avoidRotation, avoidRotatePower * Time.deltaTime);
			transform.position += transform.right * rr * Time.deltaTime / 30;
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

		float distanceA = Vector3.Distance(hitA.point, rayA.origin);
		float distanceB = Vector3.Distance(hitB.point, rayB.origin);
		float distanceC = Vector3.Distance(hitC.point, rayC.origin);
		float distanceD = Vector3.Distance(hitD.point, rayD.origin);

		if (a && b && c && d)
		{
			if (distanceA > distanceB)
				return 2 + Vector3.SignedAngle(transform.forward, end_pos - transform.position, transform.up);
			else
				return -2 + Vector3.SignedAngle(transform.forward, end_pos - transform.position, transform.up);
		}

		if (c && d)
		{
			if (distanceC < distanceD)
				return 2;
			else
				return -2;
		}
		if (c)
			return 2;
		if (d)
			return -2;

		if (a && b)
		{
			if (distanceA < distanceB)
				return 2;
			else
				return -2;
		}
		if (a)
			return 2;
		if (b)
			return -2;

		return 0;
	}

	void standardMove()
	{
		velocity += transform.forward * acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
		transform.position += velocity * Time.deltaTime * Mathf.Clamp01(Vector3.Distance(transform.position, end_pos));
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

	public void kill()
	{
		gameObject.SetActive(false);
		active = false;
		head.transform.position = Vector3.zero;
		lineA.transform.position = Vector3.zero;
		lineB.transform.position = Vector3.zero;
		lineC.transform.position = Vector3.zero;
		lineD.transform.position = Vector3.zero;
		form_head.Remove(transform.parent.gameObject);
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "A")
		{
			velocity += acceleration * (transform.position - c.transform.position) * .3f;
		}

		if (c.gameObject.tag == "B")
		{
			kill();
		}
	}
}
