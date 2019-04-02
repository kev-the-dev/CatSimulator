using UnityEngine;

// Helper functions for managing game save
public class GameSave
{
	// Versioning for the scripting so load/saves across different versions do not create issues
	// NOTE: MUST be incremented each time the script changes in a way that changes save/load functionality
	private const int ScriptMajorVersion = 3;

	// True if there is a valid game save on disk
	public static bool Valid()
	{
		return PlayerPrefs.HasKey("script_version") && PlayerPrefs.GetInt("script_version") == ScriptMajorVersion;
	}

	// Commit saved preferences to disk
	public static void Commit()
	{
		PlayerPrefs.SetFloat("savetime", Time.time);
		PlayerPrefs.SetInt("script_version", ScriptMajorVersion);
		PlayerPrefs.Save();
	}

	// Clear script version key, so scripts will think no save exists
	public static void Clear()
	{
		PlayerPrefs.DeleteKey("script_version");
	}
}