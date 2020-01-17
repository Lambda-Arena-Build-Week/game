using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeMenu : MonoBehaviour
{
	public Player player;
	public Player menuPlayer;

	public Color pantsColor = new Color(0.0f / 255.0f, 12.0f / 255.0f, 178.0f / 255.0f);
	public Color hairColor = new Color(95.0f / 255.0f, 2.0f / 255.0f, 2.0f / 255.0f);
	public Color shirtColor = new Color(217.0f / 255.0f, 39.0f / 255.0f, 0.0f / 255.0f);
	public Color shoesColor = new Color(101.0f / 255.0f, 17.0f / 255.0f, 6.0f / 255.0f);
	public Color skinColor = new Color(219.0f / 255.0f, 171.0f / 255.0f, 125.0f / 255.0f);

    public void Start()
	{
        this.SetOriginalColors();
	}

    public void SetOriginalColors()
    {
        pantsColor = player.pantsColor;
        hairColor = player.hairColor;
        shirtColor = player.shirtColor;
        shoesColor = player.shoesColor;
        skinColor = player.skinColor;

        menuPlayer.pantsColor = pantsColor;
        menuPlayer.hairColor = hairColor;
        menuPlayer.shirtColor = shirtColor;
        menuPlayer.shoesColor = shoesColor;
        menuPlayer.skinColor = skinColor;

        menuPlayer.ChangeColors();
    }

    public void ChangeSkinColor(Color color)
	{
		skinColor = color;

		menuPlayer.skinColor = color;
		menuPlayer.ChangeColors();
	}

	public void GetSkinColor(ColorPicker picker)
	{
		skinColor = menuPlayer.skinColor;

		picker.SetColor(skinColor);
	}

    public void ChangeHairColor(Color color)
    {
        hairColor = color;

        menuPlayer.hairColor = color;
        menuPlayer.ChangeColors();
    }

    public void GetHairColor(ColorPicker picker)
    {
        hairColor = menuPlayer.hairColor;

        picker.SetColor(hairColor);
    }

    public void ChangeShirtColor(Color color)
    {
        shirtColor = color;

        menuPlayer.shirtColor = color;
        menuPlayer.ChangeColors();
    }

    public void GetShirtColor(ColorPicker picker)
    {
        shirtColor = menuPlayer.shirtColor;

        picker.SetColor(shirtColor);
    }

    public void ChangePantsColor(Color color)
    {
        pantsColor = color;

        menuPlayer.pantsColor = color;
        menuPlayer.ChangeColors();
    }

    public void GetPantsColor(ColorPicker picker)
    {
        pantsColor = menuPlayer.pantsColor;

        picker.SetColor(pantsColor);
    }

    public void ChangeShoesColor(Color color)
    {
        shoesColor = color;

        menuPlayer.shoesColor = color;
        menuPlayer.ChangeColors();
    }

    public void GetShoesColor(ColorPicker picker)
    {
        shoesColor = menuPlayer.shoesColor;

        picker.SetColor(shoesColor);
    }

    public void ChangePlayerColors()
    {
        player.hairColor = hairColor;
        player.skinColor = skinColor;
        player.shirtColor = shirtColor;
        player.pantsColor = pantsColor;
        player.shoesColor = shoesColor;
    }

    public void Cancel()
    {
        this.SetOriginalColors();
    }
}
