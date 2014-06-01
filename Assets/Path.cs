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

	private float segmentLength;
	private Vector2 segmentNormal;
	private float segmentProjection;

	private Vector2 chosen;

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

	private float PointToSegmentDistance(Vector2 p, Vector2 p0, Vector2 p1)
	{
		Vector2 local = p - p0;

		segmentProjection = Vector2.Dot(segmentNormal, local);

		if (segmentProjection < 0.0f)
		{
			chosen = p0;
			segmentProjection = 0;

			return Vector2.Distance(p, p0);
		}

		if (segmentProjection > segmentLength)
		{
			chosen = p1;
			segmentProjection = segmentLength;

			return Vector2.Distance(p, p1);
		}

		chosen = p0 + segmentNormal * segmentProjection;
		return Vector2.Distance(p, chosen);
	}


	public Vector2 MapPointToPath(Vector2 point, out Vector2 tangent, out float outside)
	{
		float d;
		float minDistance = float.MaxValue;

		Vector2 onPath = point;

		tangent = Vector2.zero;
		
		for (int i = 0; i < numPoints - 1; i++)
		{
			segmentLength = lengths[i];
			segmentNormal = normals[i];

			d = PointToSegmentDistance(point, points[i], points[i + 1]);

			if (d < minDistance)
			{
				minDistance = d;
				onPath = chosen;

				tangent = segmentNormal;
			}
		}
		
		outside = Vector2.Distance(onPath, point) - radius;

		return onPath;
	}

	public float MapPointToPathDistance(Vector2 point)
	{
		float d;
		float minDistance = float.MaxValue;

		float segmentLengthTotal = 0.0f;
		float pathDistance = 0.0f;

		float segmentLength;
		
		for (int i = 0; i < numPoints - 1; i++)
		{
			segmentLength = lengths[i];
			segmentNormal = normals[i];

			d = PointToSegmentDistance(point, points[i], points[i + 1]);

			if (d < minDistance)
			{
				minDistance = d;
				pathDistance = segmentLengthTotal + segmentProjection;
			}

			segmentLengthTotal += segmentLength;
		}
		
		return pathDistance;
	}

	public Vector2 MapPathDistanceToPoint(float pathDistance)
	{
		float remaining = pathDistance;

		if (pathDistance < 0.0f) return points[0];
		if (pathDistance >= totalLength) return points[numPoints - 1];

		Vector2 result = points[0];

		for (int i = 0; i < numPoints - 1; i++)
		{
			segmentLength = lengths[i];

			if (segmentLength < remaining)
			{
				remaining -= segmentLength;
			}
			else
			{
				float ratio = remaining / segmentLength;
				result = Vector2.Lerp(points[i], points[i + 1], ratio);
				break;
			}
		}

		return result;
	}
}
