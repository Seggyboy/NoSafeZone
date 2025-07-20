using Sandbox;

public sealed class TravelerMusicManager : Component
{
	SoundHandle soundHandle;
	protected override void OnEnabled()
	{
		PlayMusic();
	}

	public void PlayMusic()
	{
		soundHandle = Sound.Play( "sound/music/travelingmusic.sound" );
	}
	public void StopMusic()
	{
		Sound.StopAll( 0.5f );
	}

	protected override void OnUpdate()
	{
		if ( !soundHandle.IsPlaying )
		{
			PlayMusic();
		}

	}
}
