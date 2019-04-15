using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour {
	Image fill;
	Slider slider;
	void Start()
	{
		slider = GetComponent<Slider>();
		fill = gameObject.transform.Find("Fill Area").gameObject.transform.Find("Fill").GetComponent<Image>();
	}
	public void SetColor()
	{
		float hue = slider.value / 3F;
		fill.color = Color.HSVToRGB(hue, 0.3F, 0.90F);
	}
}