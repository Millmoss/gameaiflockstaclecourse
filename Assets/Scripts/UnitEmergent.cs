using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEmergent : MonoBehaviour
{
	public bool isLeader = false;
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
	public float visionDegrees = 75f;
	public GameObject groupParent;
	private UnitEmergent[] group = null;
	public UnitEmergent goal = null;
	private UnitEmergent goalScript = null;
	bool left = false;
	private Rigidbody unitBody;

	public bool active = true;

	void Start()
	{
		Physics.IgnoreLayerCollision(9, 0);
		
		group = groupParent.GetComponentsInChildren<UnitEmergent>();
		unitBody = gameObject.GetComponent<Rigidbody>();
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

		setGoal();

		//rotation

		steer();
		head.transform.rotation = transform.rotation;

		//movement
		
		standardMove();

		//y axis

		transform.position = new Vector3(transform.position.x, 0 + .35f, transform.position.z);

		unitBody.velocity = Vector3.Lerp(unitBody.velocity, Vector3.zero, .7f);
	}

	void setGoal()
	{
		if (hasValidGoal(0))
			return;

		List<UnitEmergent> followable = new List<UnitEmergent>();

		for (int i = 0; i < group.Length; i++)
		{
			if (inVision(group[i].gameObject.transform) && group[i].hasValidGoal(0))
			{
				followable.Add(group[i]);
			}
		}

		if (followable.Count < 1)
			followable.Add(group[0]);

		float nearest = Vector3.Distance(transform.position, followable[0].gameObject.transform.position);
		int nearIndex = 0;

		for (int i = 0; i < followable.Count; i++)
		{
			float dist = Vector3.Distance(transform.position, followable[i].gameObject.transform.position);

			if ((dist != 0 && dist < nearest && followable[i].GetComponent<UnitEmergent>().goal != transform) || nearest == 0)
			{
				nearest = dist;
				nearIndex = i;
			}
		}

		int taken = 0;

		for (int i = 0; i < group.Length; i++)
		{
			if (group[i].goal != null && group[i].goal.gameObject.transform == followable[nearIndex].gameObject.transform)
			{
				taken++;
			}
		}

		if (taken > 1)
			return;

		if (taken == 1)
			left = false;
		else
			left = true;

		print(left);
		goal = followable[nearIndex];
		goalScript = followable[nearIndex];
	}

	public bool hasValidGoal(int i)
	{
		if (goal == null && !isLeader)
			return false;

		if (i > group.Length)
			return false;

		if (isLeader)
			return true;
		else
			return goal.hasValidGoal(i + 1);
	}

	public bool inVision(Transform unit)
	{
		if (Vector3.Angle((unit.position - transform.position).normalized, transform.forward) < visionDegrees)
			return true;
		return false;
	}

	void steer()
	{
		if (goal == null)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.right), goalRotatePower * Time.deltaTime);
			return;
		}

		Vector3 look;
		if (left)
			look = goal.gameObject.transform.position - goal.gameObject.transform.forward - goal.gameObject.transform.right * .9f;
		else
			look = goal.gameObject.transform.position - goal.gameObject.transform.forward + goal.gameObject.transform.right * .9f;

		Quaternion goalRotation = Quaternion.LookRotation(look - transform.position);
		float rr = rayResultFront();
		Quaternion avoidRotation = transform.rotation * Quaternion.AngleAxis(rr * 4, Vector3.up);

		transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, goalRotatePower * Time.deltaTime);
		if (rr != 0)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, avoidRotation, avoidRotatePower * Time.deltaTime);
			transform.position += transform.right * rr * Time.deltaTime / 10;
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
				return 2 + Vector3.SignedAngle(transform.forward, goal.gameObject.transform.position - transform.position, transform.up);
			else
				return -2 + Vector3.SignedAngle(transform.forward, goal.gameObject.transform.position - transform.position, transform.up);
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
		if (goal == null)
		{
			velocity = Vector3.Lerp(velocity, Vector3.zero, acceleration * Time.deltaTime);
		}

		velocity += transform.forward * acceleration * Time.deltaTime;
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

	public void kill()
	{
		if (isLeader)
		{
			if (group.Length < 1)
			{
				head.transform.position = Vector3.zero;
				Destroy(gameObject);
			}

			int nearIndex = -1;
			float nearest = 100;
			for (int i = 0; i < group.Length; i++)
			{
				if (group[i].gameObject.transform != transform && (nearest > Vector3.Distance(group[i].gameObject.transform.position, transform.position) || nearIndex == -1))
				{
					nearest = Vector3.Distance(group[i].gameObject.transform.position, transform.position);
					nearIndex = i;
				}
			}

			transform.position = group[nearIndex].gameObject.transform.position;
			velocity = group[nearIndex].velocity;

			group[nearIndex].kill();
		}
		else
		{
			if (group.Length < 1)
			{
				gameObject.SetActive(false);
				return;
			}

			for (int i = 0; i < group.Length; i++)
			{
				if (group[i].gameObject.transform != transform)
					group[i].removeUnit(transform);
			}

			gameObject.SetActive(false);
			active = false;
			head.transform.position = Vector3.zero;
		}
	}

	void removeUnit(Transform t)
	{
		int index = 0;

		for (int i = 0; i < group.Length; i++)
		{
			if (group[i].gameObject.transform == t)
				index = i;
		}

		UnitEmergent[] temp = new UnitEmergent[group.Length - 1];

		for (int i = 0; i < temp.Length; i++)
		{
			if (i == index)
			{
				temp[i] = group[group.Length - 1];
			}
			else
				temp[i] = group[i];
		}
		
		group = temp;
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "B")
		{
			kill();
		}
	}
}
