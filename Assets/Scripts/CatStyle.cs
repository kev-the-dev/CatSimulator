using UnityEngine;
using System;

// Represents the visual appearence of the cat (Coat type an color)
public class CatStyle
{
	// Type of coat
	public enum CatCoat {Solid, Spots, Calico};

	public CatStyle(CatCoat coat, Color primary_color, Color secondary_color)
	{
		this.coat = coat;
		this.primary_color = primary_color;
		this.secondary_color = secondary_color;

		// Apply this color/texture to the cat
		Apply();
	}

	// Type of fur coat
	CatCoat coat;
	// Primary color of fur
	Color primary_color;
	// Secondary color of fur (spots, stripes)
	Color secondary_color;

	// Generates and returns a random cat style
	public static CatStyle RandomStyle()
	{
		// TODO: generate more random colors by generating colors within ranges of HSV values
		Color[] possible_colors = {
			new Color(73 / 255F, 71 / 255F, 71 / 255F),
			new Color(104 / 255F, 61 / 255F, 54 / 255F),
			new Color(145 / 255F, 53 / 255F, 30 / 255F),
			new Color(183 / 255F, 151 / 255F, 136 / 255F)
		};
		CatCoat coat = CatCoat.Solid;
		Color primary = possible_colors[UnityEngine.Random.Range(0, possible_colors.Length)];
		
		return new CatStyle(coat, primary, new Color(0F, 0F, 0F));
	}

	// Apply the style to the Cat's renderer
	public void Apply()
	{
		// TODO: change texture based on coat type, apply secondary color for non-solid coats
		GameObject.Find("D0603113:cat_body1").GetComponent<Renderer>().material.color = primary_color;
	}

	public override string ToString()
	{
		return string.Format("CatStyle(coat={0}, primary_color={1}, secondary_color={2})",
							 coat, primary_color, secondary_color);
	}

	// Saves style to disk
	public void Save()
	{
		PlayerPrefs.SetString("style.coat", coat.ToString());
		PlayerPrefs.SetFloat("style.primary.r", primary_color.r);
		PlayerPrefs.SetFloat("style.primary.g", primary_color.g);
		PlayerPrefs.SetFloat("style.primary.b", primary_color.b);
		PlayerPrefs.SetFloat("style.secondary.r", secondary_color.r);
		PlayerPrefs.SetFloat("style.secondary.g", secondary_color.g);
		PlayerPrefs.SetFloat("style.secondary.b", secondary_color.b);
	}

	// Load from disk
	public static CatStyle Load()
	{
		return new CatStyle((CatCoat) Enum.Parse(typeof(CatCoat), PlayerPrefs.GetString("style.coat")),
			new Color(PlayerPrefs.GetFloat("style.primary.r"),
			      PlayerPrefs.GetFloat("style.primary.g"),
			      PlayerPrefs.GetFloat("style.primary.b")),
			new Color(PlayerPrefs.GetFloat("style.secondary.r"),
			      PlayerPrefs.GetFloat("style.secondary.g"),
			      PlayerPrefs.GetFloat("style.secondary.b"))); 
	}
}