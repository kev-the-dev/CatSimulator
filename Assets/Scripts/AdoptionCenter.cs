using UnityEngine.SceneManagement;

class AdoptionCenter
{
	public static bool IsActive()
	{
		return "AdoptionCenter" == SceneManager.GetActiveScene().name;
	}
};