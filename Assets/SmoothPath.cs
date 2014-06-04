using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothPath
{
	public Vector2[] points;

	public Vector2[] curveCenters;
	public Vector2[] curveLefts;
	public float[] curveRadii;

	public float[] lengths;

	public int numLengths;
	public int numPoints;

	public SmoothPath(int _numPoints)
	{
		numPoints = _numPoints;

		points = new Vector2[numPoints];

		curveCenters = new Vector2[numPoints - 2];
		curveLefts = new Vector2[numPoints - 2];
		curveRadii = new float[numPoints - 2];

		numLengths = 2 * numPoints - 3;
		lengths = new float[numLengths];
	}

	public void SetPoint(int id, Vector2 point)
	{
		points[id] = point;
	}

	public void SetCurveRadius(int id, float radius)
	{
		curveRadii[id] = radius;
	}

	public void Init()
	{
		for (int i = 0; i < 2 * numPoints - 3; i++)
		{
			if ((i & 1) == 1) lengths[i] = 0;
			else
			{
				int id = i / 2;
				lengths[i] = (points[id + 1] - points[id]).magnitude;
			}
		}

		for (int i = 0; i < numPoints - 2; i++)
		{
			Vector2 u = (points[i] - points[i + 1]).normalized;
			Vector2 v = (points[i + 2] - points[i + 1]).normalized;

			float cosa = u.x * v.x + u.y * v.y;
			float sina = u.x * v.y - u.y * v.x;

			float angle = Mathf.Atan2(sina, cosa);

			float l = curveRadii[i] / Mathf.Tan(angle / 2.0f);
			//float l = (1 + cosa) / sina * curveRadii[i];

			//Debug.Log (angle * 180.0f / Mathf.PI + " " + sina + " " + cosa + " " + l);

			Vector2 un = new Vector2(-u.y, u.x);

			if (l < 0.0f)
			{
				l = -l;
				curveRadii[i] = -curveRadii[i];
				//un = -un;
			}

			lengths[i * 2] -= l;
			lengths[i * 2 + 2] -= l;

			//curveLefts[i] = points[i + 1] + u * 5;

			curveLefts[i] = points[i + 1] + u * l;
			curveCenters[i] = curveLefts[i] + un * curveRadii[i];

			float d = Mathf.Sqrt(l * l + curveRadii[i] * curveRadii[i]);
			lengths[i * 2 + 1] = Mathf.Acos(Mathf.Abs(curveRadii[i]) / d) * Mathf.Abs(curveRadii[i]) * 2.0f;
		}
	}
}

