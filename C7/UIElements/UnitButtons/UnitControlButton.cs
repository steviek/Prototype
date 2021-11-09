using Godot;
using ConvertCiv3Media;
using System;

public class UnitControlButton : TextureButton
{

    private string name;
    private int graphicsX;
    private int graphicsY;
    private Action<string> onPressedAction;

    public UnitControlButton(string name, int graphicsX, int graphicsY, Action<string> onPressedAction)
    {
        this.name = name;
        this.graphicsX = graphicsX;
        this.graphicsY = graphicsY;
        this.onPressedAction = onPressedAction;
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pcx buttonPcx = new Pcx(Util.Civ3MediaPath("Conquests/Art/interface/NormButtons.PCX"));
        Pcx buttonPcxRollover = new Pcx(Util.Civ3MediaPath("Conquests/Art/interface/rolloverbuttons.PCX"));
        Pcx buttonPcxPressed = new Pcx(Util.Civ3MediaPath("Conquests/Art/interface/highlightedbuttons.PCX"));
		Pcx buttonPcxAlpha = new Pcx(Util.Civ3MediaPath("Conquests/Art/interface/ButtonAlpha.pcx"));
		ImageTexture menuTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(buttonPcx, buttonPcxAlpha, graphicsX, graphicsY, 32, 32);
        ImageTexture rolloverTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(buttonPcxRollover, buttonPcxAlpha, graphicsX, graphicsY, 32, 32);
        ImageTexture pressedTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(buttonPcxPressed, buttonPcxAlpha, graphicsX, graphicsY, 32, 32);
		this.TextureNormal = menuTexture;
        this.TextureHover = rolloverTexture;
        this.TexturePressed = pressedTexture;

        this.Connect("pressed", this, "ButtonPressed");
	}

    private void ButtonPressed()
    {
        onPressedAction(this.name);
    }
}
