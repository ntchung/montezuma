using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothPath
{
	public Vector2[] points;
	public Vector2[] normals;

	public Vector2[] curveCenters;
	public Vector2[] curveLefts;
	public Vector2[] curveRights;
	public float[] curveRadii;

	public float[] lengths;
	public float[] accumLengths;

	public float totalLength;

	public int numLengths;
	public int numPoints;

	public SmoothPath(int _numPoints)
	{
		numPoints = _numPoints;

		points = new Vector2[numPoints];
		normals = new Vector2[numPoints - 1];

		curveCenters = new Vector2[numPoints - 2];
		curveLefts = new Vector2[numPoints - 2];
		curveRights = new Vector2[numPoints - 2];
		curveRadii = new float[numPoints - 2];

		numLengths = 2 * numPoints - 3;
		lengths = new float[numLengths];
		accumLengths = new float[numLengths];
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
		for (int i = 0; i < numPoints - 1; i++)
		{
			normals[i] = (points[i + 1] - points[i]).normalized;
		}

		for (int i = 0; i < numLengths; i++)
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
			Vector2 u = -normals[i];
			Vector2 v = normals[i + 1];

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
			curveRights[i] = points[i + 1] + v * l;
			curveCenters[i] = curveLefts[i] + un * curveRadii[i];

			float d = Mathf.Sqrt(l * l + curveRadii[i] * curveRadii[i]);
			lengths[i * 2 + 1] = Mathf.Acos(Mathf.Abs(curveRadii[i]) / d) * Mathf.Abs(curveRadii[i]) * 2.0f;
		}

		accumLengths[0] = lengths[0];

		for (int i = 1; i < numLengths; i++)
			accumLengths[i] = accumLengths[i - 1] + lengths[i];

		totalLength = accumLengths[2 * numPoints - 4];
	}

	public int GetPathInfo(float d, out Vector2 position, out Vector2 forward, int startIndex = 0)
	{
		if (startIndex > 0)	d -= accumLengths[startIndex - 1];

		for (int i = startIndex; i < numLengths; i++)
		{
			if (d < lengths[i])
			{
				int id = i / 2;
				float ratio = d / lengths[i];

				if ((i & 1) == 1)
				{
					float angle = d / curveRadii[id];
					float sinA = Mathf.Sin(angle);
					float cosA = Mathf.Cos(angle);

					Vector2 left = curveLefts[id];
					Vector2 center = curveCenters[id];

					float x = (left.x - center.x) * cosA + (left.y - center.y) * sinA + center.x;
					float y = -(left.x - center.x) * sinA + (left.y - center.y) * cosA + center.y;

					position = new Vector2(x, y);

					Vector2 normal = normals[id];

					x = normal.x * cosA + normal.y * sinA;
					y = -normal.x * sinA + normal.y * cosA;

					forward = new Vector2(x, y);
				}
				else 
				{
					Vector2 left = ((id == 0) ? points[0] : curveRights[id - 1]);
					Vector2 right = ((id == numPoints - 2) ? points[numPoints - 1] : curveLefts[id]);

					position = Vector2.Lerp(left, right, ratio);
					forward = normals[id];
				}

				return i;
			}
			else d -= lengths[i];
		}

		position = points[numPoints - 1];
		forward = normals[numPoints - 2];

		return numLengths;
	}
}

