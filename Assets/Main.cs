using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	public GameObject[] beacons;

	public GameObject goblin;

	private List<Vector3> displayPoints = null;

	private Path path;

	// Use this for initialization
	void Start () 
	{
		displayPoints = new List<Vector3>();

		for (int i = 0; i < beacons.Length; i++)
		{
			Vector3 pos = beacons[i].transform.position;
			pos.y = 10.0f;
			
			displayPoints.Add(pos);
		}

		path = new Path(displayPoints);

		for (int i = 0; i < 5; i++)
			for (int j = 0; j < 5; j++)
			{
				GameObject gameObject = Instantiate(goblin, new Vector3(i * 8.0f, 0.0f, j * 8.0f), Quaternion.identity) as GameObject;
				Monster monster = gameObject.GetComponent<Monster>();
				monster.path = path;
			}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnDrawGizmos()
	{
		if (displayPoints == null) return;

		for (int i = 0; i < displayPoints.Count - 1; i++)
		{
			Gizmos.DrawLine(displayPoints[i], displayPoints[i + 1]);
		}
	}
}
