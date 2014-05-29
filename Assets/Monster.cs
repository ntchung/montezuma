using UnityEngine;
using System.Collections;

public class Monster : Agent 
{
	string animName;

	public Path path;

	// Use this for initialization
	void Start () 
	{
		Reset();
		animName = animation.animation.name;
	}

	protected override void Reset()
	{
		base.Reset();

		Position = new Vector2(transform.position.x, transform.position.z);
	
		//Forward = ???;
		//Side = RotateForwardToSide(Forward);
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
