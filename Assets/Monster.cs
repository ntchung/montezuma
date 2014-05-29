using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour 
{
	Vector2 position;
	Vector2 velocity;

	float direction;

	string animName;

	public float maxSpeed;
	public Path path;

	// Use this for initialization
	void Start () 
	{
		animName = animation.animation.name;

		position = Vector3.zero;
		velocity = Vector3.zero;
	}

	void SetAnimation(string name)
	{
		if (name != animName)
		{
			animName = name;
			animation.CrossFade(name);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		float move = Input.GetAxis("Horizontal");
		if (move != 0.0f) SetAnimation("walk");
		else SetAnimation("idle");
	}
}
