using System;
using Godot;
using ConvertCiv3Media;

public class Util
{
	public class Civ3FileDialog : FileDialog
	// Use this instead of a scene-based FileDialog to avoid it saving the local dev's last browsed folder in the repo
	// While instantiated it will return to the last-accessed folder when reopened
	{
		public string RelPath= "";
		public override void _Ready()
		{
			Mode = ModeEnum.OpenFile;
			Access = AccessEnum.Filesystem;
			CurrentDir = Util.GetCiv3Path() + "/" + RelPath;
			Resizable = true;
			MarginRight = 550;
			MarginBottom = 750;
			base._Ready();
		}
		
	}
	static public string GetCiv3Path()
	{
		// Use CIV3_HOME env var if present
		string path = System.Environment.GetEnvironmentVariable("CIV3_HOME");
		if (path != null) return path;

		// Look up in Windows registry if present
		path = Civ3PathFromRegistry("");
		if (path != "") return path;

		// TODO: Maybe check an array of hard-coded paths during dev time?
		return "/civ3/path/not/found";
	}

	static public string Civ3PathFromRegistry(string defaultPath = "D:/Civilization III")
	{
		// Assuming 64-bit platform, get vanilla Civ3 install folder from registry
		return (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Infogrames Interactive\Civilization III", "install_path", defaultPath);
	}
	static public string Civ3MediaPath(string relPath, string relModPath = "")
	// Pass this function a relative path (e.g. Art/Terrain/xpgc.pcx) and it will grab the correct version
	// Assumes Conquests/Complete
	{
		string Civ3Root = GetCiv3Path();
		string [] TryPaths = new string [] {
			relModPath,
			// Needed for some reason as Steam version at least puts some mod art in Extras instead of Scenarios
			//  Also, the case mismatch is intentional. C3C makes a capital C path, but it's lower-case on the filesystem
			// NOTE: May need another replace for case-sensitive filesystmes (Mac/Linux)
			relModPath.Replace(@"\Civ3PTW\Scenarios\", @"\civ3PTW\Extras\"),
			"Conquests",
			"civ3PTW",
			""
		};
		for(int i = 0; i < TryPaths.Length; i++)
		{
			// If relModPath not set, skip that check
			if(i == 0 && relModPath == "") { continue; }
			string pathCandidate = Civ3Root + "/" + TryPaths[i] + "/" + relPath;
			if(System.IO.File.Exists(pathCandidate)) { return pathCandidate; }
		}
		throw new ApplicationException("Media path not found: " + relPath);
	}

	//Send this function a path (e.g. Art/title.pcx) and it will load it up and convert it to a texture for you.
	static public ImageTexture LoadTextureFromPCX(string relPath)
	{
		Pcx NewPCX = new Pcx(Util.Civ3MediaPath(relPath));
		return PCXToGodot.getImageTextureFromPCX(NewPCX);
	}
	
	
	//Send this function a path (e.g. Art/exitBox-backgroundStates.pcx), and the coordinates of the extracted image you need from that PCX
	//file, and it'll load it up and return you what you need.
	static public ImageTexture LoadTextureFromPCX(string relPath, int leftStart, int topStart, int width, int height)
	{
		Pcx NewPCX = new Pcx(Util.Civ3MediaPath(relPath));
		return PCXToGodot.getImageTextureFromPCX(NewPCX, leftStart, topStart, width, height);
	}
}
