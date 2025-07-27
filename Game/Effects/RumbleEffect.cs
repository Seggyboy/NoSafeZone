using System;
using Sandbox;
using static Sandbox.Citizen.CitizenAnimationHelper;

public sealed class RumbleEffect : Component
{
	[Property] public float BumpMagnitude = 1f;
	[Property] public float BumpSpeed = 10f;
	[Property] public float XBumpMultiplier = 0.03f;
	[Property] public float YBumpMultiplier = 0.03f;
	[Property] public float ZBumpMultiplier = 0.03f;

	private float timeOffset;
	private Vector3 lastOffset;

	protected override void OnEnabled()
	{
		Random Rand = new Random();
		timeOffset = Rand.Float( 1000f );
		lastOffset = Vector3.Zero;
	}

	protected override void OnUpdate()
	{
		float time = Time.Now + timeOffset;

		float offsetX = MathF.Sin( time * BumpSpeed  * 0.9f) * BumpMagnitude * XBumpMultiplier;
		float offsetY = MathF.Cos( time * BumpSpeed * 0.9f ) * BumpMagnitude * YBumpMultiplier;
		float offsetZ = (MathF.Sin( time * 5f ) * 1.0f+ MathF.Sin( time * 2.7f ) * 0.5f) * ZBumpMultiplier ;
		Vector3 newOffset = new Vector3( offsetX, offsetY, offsetZ );

		// Remove last offset, apply new one
		WorldPosition -= lastOffset;
		WorldPosition += newOffset;

		lastOffset = newOffset;
	}

	protected override void OnDisabled()
	{
		// Cleanly remove the final offset when disabled
		WorldPosition -= lastOffset;
		lastOffset = Vector3.Zero;
	}
}
