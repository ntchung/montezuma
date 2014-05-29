using UnityEngine;
using System.Collections;

public class Utility
{
	public static Vector2 ParallelComponent(Vector2 vec, Vector2 unitBasis)
	{
		float projection = Vector2.Dot(vec, unitBasis);
		return unitBasis * projection;
	}
	
	public static Vector2 PerpendicularComponent(Vector2 vec, Vector2 unitBasis)
	{
		return vec - ParallelComponent(vec, unitBasis);
	}

	public static Vector2 TruncateLength(Vector2 vec, float maxLength)
	{
		float maxLengthSquared = maxLength * maxLength;
		float vecLengthSquared = vec.sqrMagnitude;

		if (vecLengthSquared <= maxLengthSquared) return vec;
		else return vec * (maxLength / Mathf.Sqrt(vecLengthSquared));
	}

	public static float RemapInterval (float x, float in0, float in1, float out0, float out1)
	{
		float relative = (x - in0) / (in1 - in0);
		return Mathf.Lerp(out0, out1, relative);
	}
	
	public static float RemapIntervalClip (float x, float in0, float in1, float out0, float out1)
	{
		float relative = (x - in0) / (in1 - in0);
		return Mathf.Lerp(out0, out1, Mathf.Clamp01(relative));
	}
	

	public static int IntervalComparison (float x, float lowerBound, float upperBound)
	{
		if (x < lowerBound) return -1;
		if (x > upperBound) return +1;
		return 0;
	}

	public static void BlendIntoAccumulator(float smoothRate, float newValue, ref float smoothedAccumulator)
	{
		smoothedAccumulator = Mathf.Lerp(smoothedAccumulator, newValue, Mathf.Clamp01(smoothRate));
	}

	public static void BlendIntoAccumulator(float smoothRate, Vector2 newValue, ref Vector2 smoothedAccumulator)
	{
		smoothedAccumulator = Vector2.Lerp(smoothedAccumulator, newValue, Mathf.Clamp01(smoothRate));
	}

	public static Vector2 VecLimitDeviationAngleUtility (bool insideOrOutside, Vector2 source, 
	                                                     float cosineOfConeAngle, Vector2 basis)
	{
		float sourceLength = source.magnitude;
		if (sourceLength == 0.0f) return source;
		
		Vector2 direction = source / sourceLength;
		float cosineOfSourceAngle = Vector2.Dot(direction, basis);

		if (insideOrOutside)
		{
			if (cosineOfSourceAngle >= cosineOfConeAngle) return source;
		}
		else
		{
			if (cosineOfSourceAngle <= cosineOfConeAngle) return source;
		}
		
		Vector2 perp = PerpendicularComponent(source, basis);
		
		Vector2 unitPerp = perp.normalized;
	
		float perpDist = Mathf.Sqrt(1 - (cosineOfConeAngle * cosineOfConeAngle));
		Vector2 c0 = basis * cosineOfConeAngle;
		Vector2 c1 = unitPerp * perpDist;
		return (c0 + c1) * sourceLength;
	}

	public static Vector2 LimitMaxDeviationAngle(Vector2 source, float cosineOfConeAngle, Vector2 basis)
	{
		return VecLimitDeviationAngleUtility(true, source, cosineOfConeAngle, basis);
	}
	
	public static Vector2 LimitMinDeviationAngle(Vector2 source, float cosineOfConeAngle, Vector2 basis)
	{    
		return VecLimitDeviationAngleUtility(false, source, cosineOfConeAngle, basis);
	}
}
