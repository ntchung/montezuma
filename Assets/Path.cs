using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour 
{
	float scale;

	public float Scale {
		get {
			return scale;
		}
	}

	public GameObject[] beacons;

	private List<Vector3> displayPoints = null;

	private float radius = 0.0f;

	private List<Vector2> points = null;
	private int numPoints;

	private Vector2[] normals;
	private float[] lengths;

	private float totalLength;

	// Use this for initialization
	void Start () 
	{
		scale = 5.0f;

		displayPoints = new List<Vector3>();

		numPoints = 0;
		points = new List<Vector2>();

		for (int i = 0; i < beacons.Length; i++)
		{
			Vector3 pos = beacons[i].transform.position;
			pos.y = 10.0f;

			displayPoints.Add(pos);

			numPoints++;
			points.Add(new Vector2(pos.x, pos.z) / scale);
		}

		normals = new Vector2[numPoints - 1];
		lengths = new float[numPoints - 1];

		totalLength = 0.0f;
		for (int i = 0; i < numPoints - 1; i++)
		{
			normals[i] = points[i + 1] - points[i];
			lengths[i] = normals[i].magnitude;
			normals[i] /= lengths[i];

			totalLength += lengths[i];
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnDrawGizmos()
	{
		if (points == null) return;

		for (int i = 0; i < numPoints - 1; i++)
		{
			Gizmos.DrawLine(displayPoints[i], displayPoints[i + 1]);
		}
	}

	private float PointToSegmentDistance(Vector2 p, Vector2 p0, Vector2 p1, out Vector2 result)
	{
		Vector2 v = p1 - p0;
		Vector2 w = p - p0;

		float c1 = Vector2.Dot(w, v);
		if (c1 <= 0.0f)
		{
			result = p0;
			return Vector2.Distance(p, p0);
		}

		float c2 = Vector2.Dot(v, v);
		if (c2 <= c1)
		{
			result = p1;
			return Vector2.Distance(p, p1);
		}

		float b = c1 / c2;
		result = p0 + b * v;
		return Vector2.Distance(p, result);
	}


	public Vector2 MapPointToPath (Vector2 point, out Vector2 tangent, out float outside)
	{
		float d;
		float minDistance = float.MaxValue;

		Vector2 chosen;
		Vector2 onPath = point;

		tangent = Vector2.zero;
		
		for (int i = 0; i < numPoints - 1; i++)
		{
			d = PointToSegmentDistance(point, points[i], points[i + 1], out chosen);

			if (d < minDistance)
			{
				minDistance = d;
				onPath = chosen;

				tangent = normals[i];
			}
		}
		
		outside = Vector2.Distance(onPath, point) - radius;

		return onPath;
	}
}
