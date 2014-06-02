using UnityEngine;
using System.Collections;

public class GridMapItem
{
	private GridMap map;

	private int bin;
	private object obj;

	private GridMapItem next;

	public GridMapItem(GridMap _map, object _obj)
	{
		map = _map;
		obj = _obj;

		next = null;
	}

	public void UpdatePosition(Vector2 position)
	{
		int newBin = map.GetBinIndex(position);
		if (newBin != bin)
		{
			//TO DO
		}
	}
}