using Godot;
using System;

namespace PlayerSpace
{
    public partial class LoadMaps : Node
    {
        private string mapDirectory = "Maps/";
        public override void _Ready()
        {
            if (OS.HasFeature("editor"))
            {
                DirAccess directory = DirAccess.Open("res://");
                if (directory.DirExists("res://" + mapDirectory))
                {
                    GD.Print(mapDirectory + " directory exists in pck!");
                    Sprite2D mapSprite = (Sprite2D)GetChild(0);
                    mapSprite.Texture = (Texture2D)GD.Load("res://Maps/map.png");
                }
                else
                {
                    GD.Print(mapDirectory + " directory doesn't exist in pck!");
                }
            }
            else
            {
                string localMapDirectory = OS.GetExecutablePath().GetBaseDir().PathJoin(mapDirectory);
                GD.Print("Local Directory: " + localMapDirectory);
                DirAccess directory = DirAccess.Open(localMapDirectory);
                if (directory.DirExists(localMapDirectory))
                {
                    GD.Print(mapDirectory + " globalized directory exists!");
                    Sprite2D mapSprite = (Sprite2D)GetChild(0);
                    Image map = Image.LoadFromFile(localMapDirectory + "map.png");
                    mapSprite.Texture = ImageTexture.CreateFromImage(map);
                }
                else
                {
                    GD.Print(mapDirectory + " globalized directory doesn't exist!");
                }
            }

        }
        //$TextureRect.texture = ImageTexture.create_from_image(image)
    }
}