using System;
using Sandbox;

public sealed class Weapon : Component
{
	[Property] public Board board { get; set; }

	[Property] public Unit OwnerUnit { get; set; }

	public string Name { get; set; } = "Default Weapon";
	public int Damage { get; set; } = 25;
	public int AccurateRange { get; set; } = 5;
	public int MaxRange { get; set; } = 10;

	[Property] public int MaxAmmo { get; set; } = 30;

	[Property] public int CurrentAmmo { get; set; } = 30;

	public string ModelPath { get; set; } = "models/weapons/shittygun.vmdl";


	[Property] Tile TestOriginTile { get; set; }
	[Property] Tile TestTargetTile { get; set; }


	public SkinnedModelRenderer ModelRenderer { get; set; }

	protected override void OnAwake()
	{
		ModelRenderer = GameObject.AddComponent<SkinnedModelRenderer>();
	}

	protected override void OnEnabled()
	{
		ModelRenderer.Model = Model.Load( ModelPath );
		board = GameObject.Scene.GetAllComponents<Board>().FirstOrDefault();
		OwnerUnit = this.GetComponentInParent<Unit>();
	}

	[Button( "Test Shoot" )]

	public void TestShoot()
	{
		//OwnerUnit.CurrentTile.GetComponent<ModelRenderer>().Tint = Color.Green; // Indicate origin tile with green tint
		TestTargetTile.GetComponent<ModelRenderer>().Tint = Color.Green;
		Shoot(OwnerUnit.CurrentTile, TestTargetTile, OwnerUnit.Aim, 0 );
		
			
	}

	public void Shoot(Tile OriginTile, Tile DestTile, int Aim, int Panic)
	{
		if ( CurrentAmmo <= 0 )
		{
			Log.Warning( $"{Name} is out of ammo!" );
			return;
		}

		if ( OriginTile == null || DestTile == null )
		{
			Log.Warning( "Origin or destination tile is null." );
			return;
		}

		var targetTile = ChooseTargetTile( DestTile, Aim, Panic, board );

		targetTile.GetComponent<ModelRenderer>().Tint = Color.Red; // Indicate hit tile with red tint

		var go = new GameObject( "Bullet" );
		go.Name = $"{Name} Bullet";
		go.WorldPosition = OriginTile.WorldPosition; // Set bullet position to origin tile
		var bullet = go.AddComponent<Bullet>();
		bullet.StartPos = OriginTile.WorldPosition + Vector3.Up * 30f; // Raise slightly above ground
		bullet.EndPos = targetTile.WorldPosition + Vector3.Up * 30f;
		bullet.Speed = 1500f;
		bullet.Damage = Damage;

		// Here we'll initialize the physical bullet and have it travel through space physically. We may need to add physics to all the walls to do this.
		Sound.Play( "sound/weapons/m4a1gunshot.sound", GameObject.WorldPosition );
		//Log.Info( $"{Name} fired! Damage: {Damage}, Range: {AccurateRange} - {MaxRange}" );
		CurrentAmmo--;
	}

	[Button( "Reload" )]
	public void Reload()
	{
		if ( CurrentAmmo == MaxAmmo )
		{
			Log.Warning( $"{Name} is already fully loaded!" );
			return;
		}
		CurrentAmmo = MaxAmmo;
		Log.Info( $"{Name} reloaded! Current ammo: {CurrentAmmo}" );
	}

	public Tile ChooseTargetTile( Tile targetTile, int Aim, int Panic, Board board )
	{
		float accuracy = Aim / 10f;
		float panicPenalty = Panic / 20f;
		//float rangePenalty = Math.Clamp( (float)(targetTile.WorldPosition - OwnerUnit.CurrentTile.WorldPosition).Length / AccurateRange, 0.0f, 1.0f );
		float effectiveAccuracy = accuracy * (1.0f - panicPenalty); //* (1.0f - rangePenalty);

		float roll = Game.Random.Float(); 

		if ( roll <= effectiveAccuracy )
			return targetTile;

		float missFactor = 1.0f - effectiveAccuracy;
		int maxScatterRadius = Math.Clamp( (int)(missFactor * 4 + Panic / 2), 1, 5 );

		for ( int i = 0; i < 10; i++ ) // up to 10 tries to find a valid tile
		{
			int dx = Game.Random.Int( -maxScatterRadius, maxScatterRadius );
			int dy = Game.Random.Int( -maxScatterRadius, maxScatterRadius );

			Tile offsetTile = board.GetTile( targetTile.X + dx, targetTile.Y + dy );
			if ( offsetTile != null && offsetTile != OwnerUnit.CurrentTile )
				return offsetTile;
		}

		return targetTile;
	}


}
