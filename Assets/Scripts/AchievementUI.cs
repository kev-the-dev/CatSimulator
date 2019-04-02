using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour {
	void Start()
	{
		text = GameObject.Find("achievement_text").GetComponent <Text> ();
		last_display_time = -DISPLAY_PERIOD - 1F;
		to_display = new Queue<CatAchievement>();
	}
    public void OnUnlock(CatAchievement achievement) {
		to_display.Enqueue(achievement);
	}
	public void Update ()
	{
		if (Time.time - last_display_time > DISPLAY_PERIOD) {
			if (to_display.Count > 0) {
				EnableDisplay();
				CatAchievement current = to_display.Dequeue();
				text.text = current.name;
				last_display_time = Time.time;
			} else {
				DisableDisplay();
			}
		}
	}
	private void DisableDisplay()
	{
		GetComponent<Image>().enabled = false;
		text.enabled = false;
	}
	private void EnableDisplay()
	{
		GetComponent<Image>().enabled = true;
		text.enabled = true;
	}

	private Text text;
	private float last_display_time;
	private const float DISPLAY_PERIOD = 3F;
	private Queue<CatAchievement> to_display;
}