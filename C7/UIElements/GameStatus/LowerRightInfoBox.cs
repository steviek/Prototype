using Godot;
using System;
using ConvertCiv3Media;
using C7GameData;

public class LowerRightInfoBox : TextureRect
{

	TextureButton nextTurnButton = new TextureButton();
	ImageTexture nextTurnOnTexture;
	ImageTexture nextTurnOffTexture;
	ImageTexture nextTurnBlinkTexture;

	Label lblUnitSelected = new Label();
	Label attackDefenseMovement = new Label();
	Label terrainType = new Label();
	Label yearAndGold = new Label();
	
	Timer blinkingTimer = new Timer();
	Boolean timerStarted = false;	//This "isStopped" returns false if it's never been started.  So we need this to know if we've ever started it.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CreateUI();
	}

	private void CreateUI() {
		Pcx boxRightColor = new Pcx(Util.Civ3MediaPath("Art/interface/box right color.pcx"));
		Pcx boxRightAlpha = new Pcx(Util.Civ3MediaPath("Art/interface/box right alpha.pcx"));
		ImageTexture boxRight = PCXToGodot.getImageFromPCXWithAlphaBlend(boxRightColor, boxRightAlpha);
		TextureRect boxRightRectangle = new TextureRect();
		boxRightRectangle.Texture = boxRight;
		boxRightRectangle.SetPosition(new Vector2(0, 0));
		AddChild(boxRightRectangle);

		Pcx nextTurnColor = new Pcx(Util.Civ3MediaPath("Art/interface/nextturn states color.pcx"));
		Pcx nextTurnAlpha = new Pcx(Util.Civ3MediaPath("Art/interface/nextturn states alpha.pcx"));
		nextTurnOffTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(nextTurnColor, nextTurnAlpha, 0, 0, 47, 28);
		nextTurnOnTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(nextTurnColor, nextTurnAlpha, 47, 0, 47, 28);
		nextTurnBlinkTexture = PCXToGodot.getImageFromPCXWithAlphaBlend(nextTurnColor, nextTurnAlpha, 94, 0, 47, 28);

		nextTurnButton.TextureNormal = nextTurnOffTexture;
		nextTurnButton.TextureHover = nextTurnOnTexture;
		nextTurnButton.SetPosition(new Vector2(0, 0));
		AddChild(nextTurnButton);
		nextTurnButton.Connect("pressed", this, "turnEnded");


		//Labels and whatnot in this text box
		lblUnitSelected.Text = "Settler";
		lblUnitSelected.AddColorOverride("font_color", new Color(0, 0, 0));
		lblUnitSelected.Align = Label.AlignEnum.Right;
		lblUnitSelected.SetPosition(new Vector2(0, 20));
		lblUnitSelected.AnchorRight = 1.0f;
		lblUnitSelected.MarginRight = -35;
		boxRightRectangle.AddChild(lblUnitSelected);
		
		attackDefenseMovement.Text = "0.0. 1/1";
		attackDefenseMovement.AddColorOverride("font_color", new Color(0, 0, 0));
		attackDefenseMovement.Align = Label.AlignEnum.Right;
		attackDefenseMovement.SetPosition(new Vector2(0, 35));
		attackDefenseMovement.AnchorRight = 1.0f;
		attackDefenseMovement.MarginRight = -35;
		boxRightRectangle.AddChild(attackDefenseMovement);
		
		terrainType.Text = "Grassland";
		terrainType.AddColorOverride("font_color", new Color(0, 0, 0));
		terrainType.Align = Label.AlignEnum.Right;
		terrainType.SetPosition(new Vector2(0, 50));
		terrainType.AnchorRight = 1.0f;
		terrainType.MarginRight = -35;
		boxRightRectangle.AddChild(terrainType);
		
		//For the centered labels, we anchor them center, with equal weight on each side.
		//Then, when they are visible, we add a left margin that's negative and equal to half
		//their width.
		//Seems like there probably is an easier way, but I haven't found it yet.
		Label civAndGovt = new Label();
		civAndGovt.Text = "Rome - Despotism (5.5.0)";
		civAndGovt.AddColorOverride("font_color", new Color(0, 0, 0));
		civAndGovt.Align = Label.AlignEnum.Center;
		civAndGovt.SetPosition(new Vector2(0, 90));
		civAndGovt.AnchorLeft = 0.5f;
		civAndGovt.AnchorRight = 0.5f;
		boxRightRectangle.AddChild(civAndGovt);
		civAndGovt.MarginLeft = -1 * (civAndGovt.RectSize.x/2.0f);

		yearAndGold.Text = "Turn 0  10 Gold (+0 per turn)";
		yearAndGold.AddColorOverride("font_color", new Color(0, 0, 0));
		yearAndGold.Align = Label.AlignEnum.Center;
		yearAndGold.SetPosition(new Vector2(0, 105));
		yearAndGold.AnchorLeft = 0.5f;
		yearAndGold.AnchorRight = 0.5f;
		boxRightRectangle.AddChild(yearAndGold);
		yearAndGold.MarginLeft = -1 * (yearAndGold.RectSize.x/2.0f);
	}

	public void SetEndOfTurnStatus() {
		lblUnitSelected.Text = "ENTER or SPACEBAR for next turn";
		attackDefenseMovement.Visible = false;
		terrainType.Visible = false;

		toggleEndTurnButton();
		
		if (!timerStarted) {
			blinkingTimer.OneShot = false;
			blinkingTimer.WaitTime = 0.6f;
			blinkingTimer.Connect("timeout", this, "toggleEndTurnButton");
			AddChild(blinkingTimer);
			blinkingTimer.Start();
			GD.Print("Started a timer for blinking");

			timerStarted = true;
		}
	}

	private void toggleEndTurnButton()
	{
		if (nextTurnButton.TextureNormal == nextTurnOnTexture) {
			nextTurnButton.TextureNormal = nextTurnBlinkTexture;
			lblUnitSelected.Visible = true;
		}
		else {
			nextTurnButton.TextureNormal = nextTurnOnTexture;
			lblUnitSelected.Visible = false;
		}
	}

	public void StopToggling() {
		nextTurnButton.TextureNormal = nextTurnOffTexture;
		lblUnitSelected.Text = "Please wait...";
		lblUnitSelected.Visible = true;
		blinkingTimer.Stop();
		timerStarted = false;
	}

	private void turnEnded() {
		GD.Print("Emitting the blinky button pressed signal");
		GetParent().EmitSignal("BlinkyEndTurnButtonPressed");
		
	}

	public void UpdateUnitInfo(MapUnit NewUnit)
	{
		lblUnitSelected.Visible = true;
		attackDefenseMovement.Visible = true;
		terrainType.Visible = true;
		lblUnitSelected.Text = NewUnit.unitType.name;
		attackDefenseMovement.Text = NewUnit.unitType.attack + "." + NewUnit.unitType.defense + " " + NewUnit.unitType.movement + "/" + NewUnit.unitType.movement;
	}

	///This is going to evolve a lot over time.  Probably this info box will need to keep some local state.
	///But for now it'll show the changing turn number, providing some interactivity
	public void SetTurn(int turnNumber)
	{
		yearAndGold.Text = "Turn " + turnNumber + "  10 Gold (+0 per turn)";
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
