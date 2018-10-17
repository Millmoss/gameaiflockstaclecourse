using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
	public UnitFlockFollow unitScript;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (((tag == "A" && other.tag == "B") || (tag == "B" && other.tag == "A")) && other.name != "Trigger")
		{
			unitScript.addToLocal(other.gameObject);
		}
	}
}
