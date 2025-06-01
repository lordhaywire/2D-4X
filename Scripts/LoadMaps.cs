using Godot;

namespace PlayerSpace;

public partial class LoadMaps : Node
{
    private string mapDirectory = "Maps/";
    string editorMapDirectory = "res://Maps/map.png";
    string editorColorCodedMapDirectory = "res://Maps/colorcodedmap.png";

    public override void _Ready()
    {
        if (OS.HasFeature("editor"))
        {
            DirAccess directory = DirAccess.Open("res://");
            if (directory.DirExists("res://" + mapDirectory))
            {
                EditorMapLoader(editorColorCodedMapDirectory, (Sprite2D)GetChild(0));
                EditorMapLoader(editorMapDirectory, (Sprite2D)GetChild(1));
            }
            else
            {
                //GD.Print(mapDirectory + "directory doesn't exist in pck!");
            }
                
        }
        else
        {
            string localMapDirectory = OS.GetExecutablePath().GetBaseDir().PathJoin(mapDirectory);
            //GD.Print("Local Directory: " + localMapDirectory);
            DirAccess directory = DirAccess.Open(localMapDirectory);
            if (directory.DirExists(localMapDirectory))
            {
                //GD.Print(mapDirectory + " globalized directory exists!");
                Sprite2D mapSprite = (Sprite2D)GetChild(0);
                Image map = Image.LoadFromFile(localMapDirectory + "map.png");
                mapSprite.Texture = ImageTexture.CreateFromImage(map);
            }
            else
            {
                //GD.Print(mapDirectory + "globalized directory doesn't exist!");
            }
        }
    }

    private void EditorMapLoader(string mapDirectory, Sprite2D mapSprite)
    {
        //GD.Print($"Map has been loaded from {mapDirectory}");
        mapSprite.Texture = (Texture2D)GD.Load(mapDirectory);
    }
}