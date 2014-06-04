using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour 
{
	public GameObject goblin;

	private SmoothPath path = null;

	private List<Vector3> displayPoints;

	// Use this for initialization
	void Start () 
	{
		path = new SmoothPath(14);
		path.SetPoint(0, new Vector2(40.0f, 12.51211f));
		path.SetPoint(1, new Vector2(27.31694f, 15.50579f));
		path.SetPoint(2, new Vector2(0.0f, 13.1085f));
		path.SetPoint(3, new Vector2(31.02438f, 8.70939f));
		path.SetPoint(4, new Vector2(29.4586f, 4.315308f));
		path.SetPoint(5, new Vector2(24.59858f, 4.947972f));
		path.SetPoint(6, new Vector2(19.33292f, 4.624193f));
		path.SetPoint(7, new Vector2(15.29456f, 4.902945f));
		path.SetPoint(8, new Vector2(13.39676f, 6.435778f));
		path.SetPoint(9, new Vector2(12.84386f, 8.886725f));
		path.SetPoint(10, new Vector2(9.424064f, 8.247343f));
		path.SetPoint(11, new Vector2(9.531949f, 4.741022f));
		path.SetPoint(12, new Vector2(11.28376f, 1.784844f));
		path.SetPoint(13, new Vector2(19.87172f, 1.2862f));

		path.SetCurveRadius(0, 5.0f);
		path.SetCurveRadius(1, 1.35f);
		path.SetCurveRadius(2, 2.0f);
		path.SetCurveRadius(3, 2.0f);
		path.SetCurveRadius(4, 5.0f);
		path.SetCurveRadius(5, 5.0f);
		path.SetCurveRadius(6, 3.0f);
		path.SetCurveRadius(7, 2.0f);
		path.SetCurveRadius(8, 1.0f);
		path.SetCurveRadius(9, 2.0f);
		path.SetCurveRadius(10, 5.0f);
		path.SetCurveRadius(11, 2.0f);

		path.Init();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnDrawGizmosSelected()
	{
		if (path == null) return;

		Gizmos.color = Color.white;

		Vector2[] points = path.points;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector3 p0 = new Vector3(points[i].x, 10.0f, points[i].y);
			Vector3 p1 = new Vector3(points[i + 1].x, 10.0f, points[i + 1].y);
			Gizmos.DrawLine(p0, p1);
		}

		Gizmos.color = Color.blue;

		float[] curveRadii = path.curveRadii;
		Vector2[] curveCenters = path.curveCenters;
		Vector2[] curveLefts = path.curveLefts;

		float[] lengths = path.lengths;

		for (int i = 0; i < points.Length - 2; i++)
		{
			if (Mathf.Abs(curveRadii[i]) < Mathf.Epsilon) continue;

			float fullAngle = lengths[2 * i + 1] / curveRadii[i];

			Vector2 left = curveLefts[i];
			Vector2 center = curveCenters[i];

			Vector2 p0 = left;
			Vector2 p1 = p0;

			Gizmos.color = Color.blue;

			for (int j = 1; j <= 10; j++)
			{
				float angle = fullAngle * j / 10;

				float x = (left.x - center.x) * Mathf.Cos(angle) + (left.y - center.y) * Mathf.Sin(angle) + center.x;
				float y = -(left.x - center.x) * Mathf.Sin(angle) + (left.y - center.y) * Mathf.Cos(angle) + center.y;
				p1 = new Vector2(x, y);

				Gizmos.DrawLine(new Vector3(p0.x, 10.0f, p0.y), new Vector3(p1.x, 10.0f, p1.y));

				p0 = p1;
			}

			Gizmos.color = Color.gray;

			p0 = left;
			Gizmos.DrawLine(new Vector3(center.x, 10.0f, center.y), new Vector3(p0.x, 10.0f, p0.y));
			Gizmos.DrawLine(new Vector3(center.x, 10.0f, center.y), new Vector3(p1.x, 10.0f, p1.y));
		}

		Gizmos.color = Color.white;
	}
}
