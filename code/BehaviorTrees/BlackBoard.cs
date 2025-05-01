using Sandbox;

public sealed class BlackBoard : Component
{
	[Property] GameObject Player;

	public Vector3 PlayerLocation;
	public Rotation PlayerRotation;
	public Vector3 PlayerVelocity;
	
	protected override void OnUpdate()
	{
		if (Player.IsValid())
		{
			PlayerLocation = Player.WorldPosition;
			PlayerRotation = Player.WorldRotation;
			PlayerVelocity = Player.GetComponent<PlayerController>().Velocity;
		}
	}
}
