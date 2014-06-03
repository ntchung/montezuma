using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour 
{
	static public List<Agent> neighbors = new List<Agent>();

	public GridMap gridMap;
	public GridMapItem gridMapItem;

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
		gridMapItem = gridMap.CreateItem(position, this);

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

		gridMap.UpdateItemPosition(gridMapItem, position);
	}

	protected float	PredictNearestApproachTime(Agent other)
	{
		Vector2 myVelocity = Velocity;
		Vector2 otherVelocity = other.Velocity;
		Vector2 relVelocity = otherVelocity - myVelocity;
		float relSpeed = relVelocity.magnitude;
		
		if (relSpeed == 0.0f) return 0.0f;

		Vector2 relTangent = relVelocity / relSpeed;
		Vector2 relPosition = position - other.position;
		float projection = Vector2.Dot(relTangent, relPosition);

		return projection / relSpeed;
	}

	protected float	ComputeNearestApproachPositions(Agent other, float time, out Vector2 ourPosition, out Vector2 hisPosition)
	{
		Vector2 myTravel = forward * speed * time;
		Vector2 otherTravel = other.forward * other.speed * time;
		
		ourPosition = position + myTravel;
		hisPosition = other.position + otherTravel;
		
		return Vector2.Distance(ourPosition, hisPosition);
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

	protected Vector2 SteerToFollowPath(int direction, float predictionTime, Path path)
	{
		float pathDistanceOffset = direction * predictionTime * speed;
		Vector2 futurePosition = PredictFuturePosition(predictionTime);
		
		float nowPathDistance =	path.MapPointToPathDistance(position);
		float futurePathDistance = path.MapPointToPathDistance(futurePosition);
		
		bool rightway = ((pathDistanceOffset > 0) ? (nowPathDistance < futurePathDistance) : (nowPathDistance > futurePathDistance));

		Vector2 tangent;
		float outside;

		path.MapPointToPath(futurePosition, out tangent, out outside);
		
		if ((outside < 0.0f) && rightway)
		{
			return Vector2.zero;
		}
		else
		{
			float targetPathDistance = nowPathDistance + pathDistanceOffset;
			Vector2 target = path.MapPathDistanceToPoint(targetPathDistance);
			
			return SteerForSeek (target);
		}
	}

	protected Vector2 SteerToAvoidCloseNeighbors(float minSeparationDistance, List<Agent> others)
	{
		foreach (Agent other in others)
		{
			if (other == this) continue;

			float sumOfRadii = radius + other.radius;
			float minCenterToCenter = minSeparationDistance + sumOfRadii;
			Vector2 offset = other.position - position;
			float currentDistance = offset.magnitude;
			
			if (currentDistance < minCenterToCenter)
			{
				return Utility.PerpendicularComponent(-offset, forward);
			}
		}
		
		return Vector2.zero;
	}

	protected Vector2 SteerToAvoidNeighbors(float minTimeToCollision, List<Agent> others)
	{
		Vector2 separation = SteerToAvoidCloseNeighbors(0.0f, others);
		if (separation != Vector2.zero) return separation;
		
		float steer = 0.0f;
		Agent threat = null;
		
		float minTime = minTimeToCollision;
		
		Vector2 threatPositionAtNearestApproach = position;

		Vector2 threatPosition;
		Vector2 ourPosition;
		
		foreach (Agent other in others)
		{
			if (other == this) continue;

			float collisionDangerThreshold = radius * 2.0f;
			float time = PredictNearestApproachTime(other);
			
			if ((time >= 0.0f) && (time < minTime))
			{
				if (ComputeNearestApproachPositions(other, time, out ourPosition, out threatPosition) < collisionDangerThreshold)
				{
					minTime = time;
					threat = other;

					threatPositionAtNearestApproach = threatPosition;
				}
			}
		}
		
		if (threat != null)
		{
			float parallelness = Vector2.Dot(forward, threat.forward);
			float angle = 0.707f;
			
			if (parallelness < -angle)
			{
				Vector2 offset = threatPositionAtNearestApproach - position;
				float sideDot = Vector2.Dot(offset, side);
				steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
			}
			else if (parallelness > angle)
			{
				Vector2 offset = threat.position - position;
				float sideDot = Vector2.Dot(offset, side);
				steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
			}
			else
			{
				if (threat.speed <= speed)
				{
					float sideDot = Vector2.Dot(side, threat.Velocity);
					steer = (sideDot > 0.0f) ? -1.0f : 1.0f;
				}
			}
		}
		
		return side * steer;
	}

	public void FindNeighborsCallback(object obj)
	{
		neighbors.Add(obj as Agent);
	}

	public void FindNeighbors(float radius)
	{
		neighbors.Clear();
		gridMap.FindItems(position, radius, FindNeighborsCallback); 
	}
}
