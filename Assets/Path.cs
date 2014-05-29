using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour 
{
	public GameObject[] beacons;

	private List<Vector3> points = null;

	// Use this for initialization
	void Start () 
	{
		points = new List<Vector3>();
		for (int i = 0; i < beacons.Length; i++)
		{
			Vector3 pos = beacons[i].transform.position;
			pos.y = 10.0f;
			points.Add(pos);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnDrawGizmos()
	{
		if (points == null) return;

		for (int i = 0; i < points.Count - 1; i++)
		{
			Gizmos.DrawLine(points[i], points[i + 1]);
		}
	}
}
