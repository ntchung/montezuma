using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

		outBin = null;
	}

	public int GetBinIndex(Vector2 position)
	{
		int x = Mathf.FloorToInt((position.x - min.x) / gridSize);
		int y = Mathf.FloorToInt((position.y - min.y)/ gridSize);

		if (x < 0 || x >= w || y < 0 || y >= h) return -1;

		return x * h + y;
	}

	public GridMapItem CreateItem(Vector2 position, object obj)
	{
		int bin = GetBinIndex(position);

		GridMapItem item = new GridMapItem();
		item.obj = obj;
		item.position = position;
		item.bin = bin;
		item.next = item.prev = null;

		AddItem(item);
		return item;
	}

	private void AddItem(GridMapItem item)
	{
		if (item.bin > 0)
		{
			AddItem(item, ref bins[item.bin]);
		}
		else 
		{
			AddItem(item, ref outBin);
		}
	}

	private void AddItem(GridMapItem item, ref GridMapItem head)
	{
		item.next = head;
		if (head != null) head.prev = item;

		head = item;
	}

	private void RemoveItem(GridMapItem item)
	{
		if (item.bin > 0)
		{
			RemoveItem(item, ref bins[item.bin]);
		}
		else 
		{
			RemoveItem(item, ref outBin);
		}
	}

	public void RemoveItem(GridMapItem item, ref GridMapItem head)
	{
		if (item == head) head = item.next;
		else item.prev.next = item.next;

		if (item.next != null) item.next.prev = item.prev;
	}

	public void UpdateItemPosition(GridMapItem item, Vector2 position)
	{
		item.position = position;

		int newBin = GetBinIndex(position);
		if (newBin != item.bin)
		{
			RemoveItem(item);

			item.bin = newBin;
			item.prev = item.next = null;

			AddItem(item);
		}
	}

	public void FindItemsOutside(Vector2 position, float radius, Action<object> callback)
	{
		float radiusSqr = radius * radius;
		float d;

		GridMapItem head = outBin;
		while (head != null)
		{
			d = (position - head.position).sqrMagnitude;
			if (d < radiusSqr) callback(head.obj);
			
			head = head.next;
		}
	}

	public void FindItems(Vector2 position, float radius, Action<object> callback)
	{
		int minBinX = Mathf.FloorToInt((position.x - radius - min.x) / gridSize);
		int maxBinX = Mathf.CeilToInt((position.x + radius - min.x) / gridSize);
		int minBinY = Mathf.FloorToInt((position.y - radius - min.y) / gridSize);
		int maxBinY = Mathf.CeilToInt((position.y + radius - min.y) / gridSize);

		if (maxBinX < 0 || minBinX >= w || maxBinY < 0 || minBinY >= h)
		{
			FindItemsOutside(position, radius, callback);
			return;
		}

		bool partlyOut = false;

		if (minBinX < 0) 
		{
			minBinX = 0;
			partlyOut = true;
		}

		if (maxBinX >= w) 
		{
			maxBinX = w - 1;
			partlyOut = true;
		}

		if (minBinY < 0) 
		{
			minBinY = 0;
			partlyOut = true;
		}
		
		if (maxBinY >= h) 
		{
			maxBinY = h - 1;
			partlyOut = true;
		}

		if (partlyOut) FindItemsOutside(position, radius, callback);

		float radiusSqr = radius * radius;
		float d;

		for (int i = minBinX; i <= maxBinX; i++)
		{
			for (int j = minBinY; j <= maxBinY; j++)
			{
				GridMapItem head = bins[i * h + j];
				while (head != null)
				{
					d = (position - head.position).sqrMagnitude;
					if (d < radiusSqr) callback(head.obj);

					head = head.next;
				}
			}
		}
	}
}