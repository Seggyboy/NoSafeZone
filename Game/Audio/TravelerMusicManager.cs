using Sandbox;

public sealed class TravelerMusicManager : Component
{
	[Property] MusicManager MusicManager { get; set; }
	public string MusicPath = "sound/music/travelingmusic.sound";
	protected override void OnEnabled()
	{
		
	}

	public void GrabMusicManager()
	{
		MusicManager = Scene.GetAllComponents<MusicManager>().FirstOrDefault();
		PlayMusic();
	}

	public void PlayMusic()
	{
		MusicManager.PlayTrack( "sound/music/travelingmusic.sound", true );
	}

	protected override void OnUpdate()
	{
		if (MusicManager == null)
		{
			GrabMusicManager();
		}

	}
}
