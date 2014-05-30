﻿using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour 
{
	Vector2 position;

	public Vector2 Position {
		get {
			return position;
		}
		set {
			position = value;
		}
	}

	Vector2 forward;

	public Vector2 Forward {
		get {
			return forward;
		}
		set {
			forward = value;
		}
	}

	Vector2 side;

	public Vector2 Side {
		get {
			return side;
		}
		set {
			side = value;
		}
	}

	public Vector2 Velocity {
		get {
			return forward * speed;
		}
	}

	float speed;

	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
		}
	}

	float radius;

	public float Radius {
		get {
			return radius;
		}
		set {
			radius = value;
		}
	}

	float maxForce;

	public float MaxForce {
		get {
			return maxForce;
		}
		set {
			maxForce = value;
		}
	}

	float maxSpeed;

	public float MaxSpeed {
		get {
			return maxSpeed;
		}
		set {
			maxSpeed = value;
		}
	}

	Vector2 smoothedAcceleration;

	public Vector2 SmoothedAcceleration {
		get {
			return smoothedAcceleration;
		}
		set {
			smoothedAcceleration = value;
		}
	}

	public Vector2 RotateForwardToSide(Vector2 v)
	{
		return new Vector2(-v.y, v.x);
	}

	public virtual void Reset()
	{
		speed = 0.0f;
		radius = 0.5f;

		maxSpeed = 2.0f;
		maxForce = 8.0f;

		smoothedAcceleration = Vector2.zero;
	}

	public void RegenerateLocalSpace(Vector2 newVelocity)
	{
		if (speed > 0.0f)
		{
			forward = newVelocity / speed;
			side = RotateForwardToSide(forward);
		}
	}

	public Vector2 PredictFuturePosition (float predictionTime)
	{
		return position + (Velocity * predictionTime);
	}

	protected Vector2 AdjustRawSteeringForce(Vector2 force)
	{
		float maxAdjustedSpeed = 0.2f * maxSpeed;
		
		if (speed > maxAdjustedSpeed || force == Vector2.zero)
		{
			return force;
		}
		else
		{
			float range = speed / maxAdjustedSpeed;
			//float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 6.0f));
			//float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 10.0f));
			//float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 20.0f));
			//float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 100.0f));
			//float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 50.0f));
			float cosine = Mathf.Lerp(1.0f, -1.0f, Mathf.Pow(range, 20.0f));
			return Utility.LimitMaxDeviationAngle(force, cosine, forward);
		}
	}
		
	protected void ApplySteeringForce(Vector2 force, float elapsedTime)
	{
		Vector2 adjustedForce = AdjustRawSteeringForce(force);
		Vector2 clippedForce = Utility.TruncateLength(adjustedForce, maxForce);

		//Vector2 newAcceleration = clippedForce / mass;
		Vector2 newAcceleration = clippedForce;
		Vector2 newVelocity = Velocity;
		
		if (elapsedTime > 0.0f)
		{
			float smoothRate = Mathf.Clamp(9 * elapsedTime, 0.15f, 0.4f);
			Utility.BlendIntoAccumulator(smoothRate, newAcceleration, ref smoothedAcceleration);
		}
		
		newVelocity += smoothedAcceleration * elapsedTime;
		newVelocity = Utility.TruncateLength(newVelocity, maxSpeed);

		speed = newVelocity.magnitude;

		position += newVelocity * elapsedTime;

		RegenerateLocalSpace(newVelocity);
	}

	protected Vector2 SteerForSeek(Vector2 target)
	{
		Vector2 desiredVelocity = target - position;
		return desiredVelocity - Velocity;
	}

	protected Vector2 SteerToStayOnPath (float predictionTime, Path path)
	{
		Vector2 futurePosition = PredictFuturePosition(predictionTime);
		
		Vector2 tangent;
		float outside;

		Vector2 onPath = path.MapPointToPath(futurePosition, out tangent, out outside);
		
		if (outside < 0.0f)	return Vector2.zero;
		else return SteerForSeek(onPath);
	}
}
