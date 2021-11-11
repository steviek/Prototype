using Godot;

public class DisbandConfirmation : TextureRect
{

	public DisbandConfirmation() 
	{

	}

	public override void _Ready()
	{
		base._Ready();

		//Dimensions in-game are 530x320
		//The top 110px are for the advisor leaderhead, Domestic in this case.
		//For some reason it uses the Happy graphics.

		ImageTexture AdvisorHappy = Util.LoadTextureFromPCX("Art/SmallHeads/popupDOMESTIC.pcx", 1, 40, 149, 110);
		TextureRect AdvisorHead = new TextureRect();
		AdvisorHead.Texture = AdvisorHappy;
		//Appears at 400, 110 in game, but leftmost 25px are transparent with default graphics
		AdvisorHead.SetPosition(new Vector2(375, 0));
		AddChild(AdvisorHead);

		//The pop-up part is the tricky part
		ImageTexture topLeftPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 250, 0, 62, 45);
		ImageTexture topCenterPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 312, 0, 62, 45);
		ImageTexture topRightPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 374, 0, 62, 45);
		ImageTexture middleLeftPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 250, 45, 62, 45);
		ImageTexture middleCenterPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 312, 45, 62, 45);
		ImageTexture middleRightPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 374, 45, 62, 45);
		ImageTexture bottomLeftPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 250, 90, 62, 45);
		ImageTexture bottomCenterPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 312, 90, 62, 45);
		ImageTexture bottomRightPopup = Util.LoadTextureFromPCX("Art/popupborders.pcx", 374, 90, 62, 45);

		//Dimensions are 530x320.  The leaderhead takes up 110.  So the popup is 530x210.
		//We have multiples of... 62? For the horizontal dimension, 45 for vertical.
		//45 does not fit into 210.  90, 135, 180, 215.  Well, 215 is sorta closeish.
		//62, we got 62, 124, 248, 496, 558.  Doesn't match up at all.
		//Which means that partial textures can be used.  Lovely.

		//Let's try adding some helper functions so this can be refactored later into a more general-purpose popup popper
		drawRowOfPopup(110, 530, topLeftPopup, topCenterPopup, topRightPopup);
		drawRowOfPopup(155, 530, middleLeftPopup, middleCenterPopup, middleRightPopup);
		drawRowOfPopup(200, 530, bottomLeftPopup, bottomCenterPopup, bottomRightPopup);

	}

	private void drawRowOfPopup(int vOffset, int width, ImageTexture left, ImageTexture center, ImageTexture right)
	{
		//Okay, at least we only need one function for all three rows, it can be SIMD (single instruction, multiple data) by analogy
		TextureRect leftRectangle = new TextureRect();
		leftRectangle.SetPosition(new Vector2(0, vOffset));
		leftRectangle.Texture = left;
		AddChild(leftRectangle);

		int leftOffset = 62;    //yes, it will always be 62.  at least with Civ graphics.  so like WildWeazel, it will be hard coded
		for (;leftOffset < width - 62; leftOffset += 62)
		{
			TextureRect middleRectangle = new TextureRect();
			middleRectangle.SetPosition(new Vector2(leftOffset, vOffset));
			middleRectangle.Texture = center;
			AddChild(middleRectangle);
		}

		leftOffset = width - 62;
		TextureRect rightRectangle = new TextureRect();
		rightRectangle.SetPosition(new Vector2(leftOffset, vOffset));
		rightRectangle.Texture = right;
		AddChild(rightRectangle);
	}
}
