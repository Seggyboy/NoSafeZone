using System;

public class SkillSet
{

	private Dictionary<SkillTypes, int> skillLevels;

	public SkillSet()
	{
		skillLevels = new();
		Random rand = new();

		foreach ( SkillTypes skill in Enum.GetValues( typeof( SkillTypes ) ) )
		{
			if ( skill == SkillTypes.None )
				continue;

			// Ensure it's a single valid flag (skip combinations like Aim | Strength)
			
				int level = rand.Next( 1, 11 ); // 1 to 10 inclusive
				skillLevels[skill] = level;
			
		}
	}

	private bool IsPowerOfTwo( int value )
	{
		return (value & (value - 1)) == 0;
	}

	// Set or add a skill level (1–10)
	public void SetSkillLevel( SkillTypes skill, int level )
	{
		if ( level < 1 || level > 10 )
		{
			throw new ArgumentOutOfRangeException( nameof( level ), "Skill level must be between 1 and 10." );
		}

		skillLevels[skill] = level;
	}

	// Remove skill entirely
	public void RemoveSkill( SkillTypes skill )
	{
		skillLevels[skill] = 0; // Set to 0 to indicate no skill level
	}


	// Get the skill level (returns 0 if skill not present)
	public int GetSkillLevel( SkillTypes skill )
	{
		return skillLevels.TryGetValue( skill, out var level ) ? level : 0;
	}

	// Optional: return all skills and levels
	public Dictionary<SkillTypes, int> GetAllSkills()
	{
		return new Dictionary<SkillTypes, int>( skillLevels );
	}


}
