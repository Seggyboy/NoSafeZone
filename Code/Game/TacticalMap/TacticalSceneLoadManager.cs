using Sandbox;
using System.Threading.Tasks;   // <-- add this

public sealed class TacticalSceneLoadManager : Component
{
	[Property] Board board;
	[Property] SoundManager SoundManager;

	protected override void OnEnabled()
	{
		_ = LoadSequence();      // fire-and-forget

		TacticalEvents.WinConditionMetEvent += () =>
		{
			Log.Info( "Win condition met!" );
			SoundManager.PlayWinSound();
			// Handle win condition, e.g., show victory screen
		};
		TacticalEvents.LossConditionMetEvent += () =>
		{
			Log.Info( "Loss condition met!" );
			SoundManager.PlayLossSound();
			// Handle loss condition, e.g., show defeat screen
		};
	}

	protected override void OnDisabled()
	{
		_ = LoadSequence();      // fire-and-forget

		TacticalEvents.WinConditionMetEvent -= () =>
		{
			Log.Info( "Win condition met!" );
			SoundManager.PlayWinSound();
			// Handle win condition, e.g., show victory screen
		};
		TacticalEvents.LossConditionMetEvent -= () =>
		{
			Log.Info( "Loss condition met!" );
			SoundManager.PlayLossSound();
			// Handle loss condition, e.g., show defeat screen
		};
	}

	private void BattleMaster_WinConditionMetEvent()
	{
		throw new System.NotImplementedException();
	}

	private async Task LoadSequence()
	{
		await Task.Delay( 1000 );        // wait 2 s (show a splash / loading screen here)

		board.GenerateMap();             // now build the tactical map

		await Task.Delay( 500 );         // tiny extra pause if you want

		SoundManager.PlaySounds();        // fade-in or start BGM
	}
}
