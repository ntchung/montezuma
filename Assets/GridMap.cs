using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMap
{
	private Vector2 min;
	private Vector2 max;

	private int w;
	private int h;

	private float gridSize;

	private int numBins;
	private GridMapItem[] bins;

	private GridMapItem outBin;

	public GridMap(Vector2 _min, Vector2 _max, float _gridSize)
	{
		min = _min;
		max = _max;

		gridSize = _gridSize;

		Vector2 size = max - min;
		w = Mathf.CeilToInt(size.x / gridSize);
		h = Mathf.CeilToInt(size.y / gridSize);

		numBins = w * h;
		bins = new GridMapItem[numBins];
		for (int i = 0; i < numBins; i++) bins[i] = null;
	}

	public int GetBinIndex(Vector2 position)
	{
		int x = Mathf.FloorToInt((position.x - min.x) / gridSize);
		int y = Mathf.FloorToInt((position.y - min.y)/ gridSize);

		if (x < 0 || x >= w || y < 0 || y >= h) return -1;

		return x * h + y;
	}
}