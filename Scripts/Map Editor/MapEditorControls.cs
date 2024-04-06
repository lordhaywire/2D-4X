using Godot;
using PlayerSpace;
using GlobalSpace;

namespace MapEditorSpace
{
    public partial class MapEditorControls : StaticBody2D
    {
        public static MapEditorControls Instance { get; private set; }

        private Image mapImage;
        [Export] private RectangleShape2D collisionRectangleShape;
        private Color backgroundColor = new(220, 220, 220);

        private int mapWidth;
        private int mapHeight;

        public bool controlsEnabled = false;

        public override void _Ready()
        {
            Instance = this;

            mapImage = MapEditorGlobals.Instance.mapColorCoded.GetImage();

            mapWidth = mapImage.GetWidth();
            mapHeight = mapImage.GetHeight();

        }

        public override void _Input(InputEvent @event)
        {
            int x = (int)GetGlobalMousePosition().X;
            int y = (int)GetGlobalMousePosition().Y;

            collisionRectangleShape.Size = new Vector2(mapWidth, mapHeight);
            // First check to make sure it is inside the map (a tiny bit more then the size of the map.)
            if (x > 0 && y > 0 && x < mapWidth - 5 && y < mapHeight - 5 && controlsEnabled == true)
            {
                Color countyColor = mapImage.GetPixel(x, y);

                //GD.Print("Color: " + countyColor);
                // Check every countyData to find the color it finds.  If it finds that color then it turns on the 
                // grey overlay.
                foreach (CountyData countyData in CountyResourcesAutoLoad.Instance.countyDatas)
                {
                    //GD.Print("County Color: " + countyData.color);
                    County county = countyData.countyNode;
                    Sprite2D maskSprite = county.maskSprite;

                    if (countyData.color.IsEqualApprox(countyColor))
                    {
                        //GD.Print("County clicked on is: " + countyData.countyName);
                        maskSprite.Show();
                    }
                    else
                    {
                        maskSprite.Hide();
                    }
                }
            }
        }
    }
}