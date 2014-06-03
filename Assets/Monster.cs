using UnityEngine;
using System.Collections;

public class Monster : Agent 
{
	string animName;

	public Path path;

	// Use this for initialization
	void Start () 
	{
		animName = animation.animation.name;

		Position = new Vector2(transform.position.x, transform.position.z) / path.Scale;
		
		Forward = Vector2.up;
		Side = RotateForwardToSide(Forward);
		
		SetAnimation("walk");

		Reset();
	}

	public override void Reset()
	{
		base.Reset();
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
		//float move = Input.GetAxis("Horizontal");
		//if (move != 0.0f) SetAnimation("walk");
		//else SetAnimation("idle");

		float elapsedTime = Time.deltaTime;

		ApplySteeringForce(DetermineCombinedSteering(elapsedTime), elapsedTime);
		
		Vector3 pos = transform.position;
		pos.x = Position.x * path.Scale;
		pos.z = Position.y * path.Scale;
		transform.position = pos;

		Vector3 forward = new Vector3(Forward.x, 0.0f, Forward.y);
		transform.rotation = Quaternion.LookRotation(forward);
	}

	public Vector2 DetermineCombinedSteering(float elapsedTime)
	{
		Vector2 steeringForce = Forward;

		const float leakThrough = 0.1f;

		Vector2 collisionAvoidance = Vector2.zero;
		float caLeadTime = 1.5f;
		
		float maxRadius = caLeadTime * MaxSpeed * 2.0f;

		FindNeighbors(maxRadius);

		if (leakThrough < UnityEngine.Random.Range(0.0f, 1.0f))
		{
			collisionAvoidance = SteerToAvoidNeighbors(caLeadTime, neighbors) * 2.0f;
		}
		
		if (collisionAvoidance != Vector2.zero)
		{
			steeringForce += collisionAvoidance;
		}
		else 
		{
			const float pfLeadTime = 3.0f;
			Vector2 pathFollow = SteerToFollowPath(1, pfLeadTime, path);

			steeringForce += pathFollow * 0.5f;
		}

		return steeringForce;
	}
}
