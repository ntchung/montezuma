using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
	string animName;

	public SmoothPath path;
	public TerrainData terrainData;

	private const float speed = 4.0f;
	private float speedScale = 1.0f;
	private float walkDist;

	private int pathStartIndex;

	private Vector3 terrainNormal;
	private Vector3 prevPosition;

	// Use this for initialization
	void Start () 
	{
		animName = animation.animation.name;
		SetAnimation("walk");

		pathStartIndex = 0;
		walkDist = 0.0f;

		UpdateMove();

		prevPosition = transform.position;
	}

	void SetAnimation(string name)
	{
		if (name != animName)
		{
			animName = name;
			animation.CrossFade(name);
		}
	}

	void UpdateMove()
	{
		Vector2 position, forward;
		pathStartIndex = path.GetPathInfo(walkDist, out position, out forward, pathStartIndex);

		float x = position.x / terrainData.size.x;
		float y = position.y / terrainData.size.z;

		float h = terrainData.GetInterpolatedHeight(x, y);

		transform.position = new Vector3(position.x, h, position.y);
		transform.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0.0f, forward.y));
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 diff = transform.position - prevPosition;
		float length = diff.magnitude;

		if (length < 1e-5f) speedScale = 1.0f;
		else
		{
			speedScale = Mathf.Clamp(-diff.y / length / 0.707f, -1.0f, 1.0f);
			if (speedScale < 0.0f) speedScale = speedScale * 0.5f + 1.0f;
			else speedScale = speedScale * 0.2f + 1.0f;
		}

		prevPosition = transform.position;

		walkDist += Time.deltaTime * speed * speedScale;
		UpdateMove();
	}
}
