using Sandbox;

public sealed class TacticalMusicManager : Component
{
	[Property] SoundHandle soundHandle;

	

	protected override void OnEnabled()
	{
		
	}

	public void PlayMusic()
	{
		soundHandle = Sound.Play( "sound/music/combatfinal.sound" );
	}
	public void StopMusic()
	{
		Sound.StopAll( 0.5f );
	}

	protected override void OnUpdate()
	{
		if (soundHandle == null)
		{
			return;
		}
		if ( !soundHandle.IsPlaying )
		{
			PlayMusic();
		}

	}
}
