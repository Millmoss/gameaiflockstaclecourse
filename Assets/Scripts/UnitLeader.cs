using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLeader : MonoBehaviour
{
	public GameObject head;
	public float moveSpeed = 1;
	private float t = 0;
	public string unitName;
	public UIHandler uiReceive;
	public Camera cam;
    public Vector3 velocity;
	public GameObject goal;
	//public Vector3 velocity;
	public float acceleration;

	void Start()
	{

	}

	void Update()
	{
		LayerMask mask = 1 << 9;
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane)), out hit, 200, mask))
		{
			goal.transform.position = hit.point;
		}

		if (Vector3.Distance(goal.transform.position, transform.position) > 1)
		{
			velocity += acceleration * (goal.transform.position - transform.position).normalized * Time.deltaTime;
			if (Vector3.Distance(goal.transform.position, transform.position) - 1 > moveSpeed)
				velocity = Vector3.ClampMagnitude(velocity, moveSpeed);
			else
				velocity = Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, Vector3.Distance(goal.transform.position, transform.position) - 1, .1f));
		}
		else
			velocity = Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, 0, .05f));

		transform.position += velocity * Time.deltaTime;

		//set y axis height

		transform.position = new Vector3(transform.position.x, .35f, transform.position.z);

		//set head position

		head.transform.rotation = Quaternion.Slerp(head.transform.rotation, Quaternion.LookRotation(goal.transform.position - transform.position), .1f);

		head.transform.position = transform.position + new Vector3(0, .3f, 0);
	}
}
