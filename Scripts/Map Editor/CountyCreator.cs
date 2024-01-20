using Godot;
using PlayerSpace;
using GlobalSpace;
using System.Threading.Tasks;

namespace MapEditorSpace
{
    public partial class CountyCreator : Node
    {
        private int startCountyWidth;
        private int startCountyHeight;
        private int countyWidth;
        private int countyHeight;
        int countyID = 0;

        public async void GenerateAll()
        {
            await GenerateMasks();
            await GenerateCounties();
        }
        private async Task GenerateMasks()
        {
            Image colorCodedMapImage = MapEditorGlobals.Instance.mapColorCoded.GetImage();
            Image mapImage = MapEditorGlobals.Instance.map.GetImage();
            Vector2I mapSize = (Vector2I)MapEditorGlobals.Instance.mapColorCoded.GetSize();
            Image countyMaskImage = Image.Create(mapSize.X, mapSize.Y, false, Image.Format.Rgba8);

            foreach (CountyData countyData in CountyResourcesAutoLoad.Instance.countyDatas)
            {
                await RootNode.Instance.WaitFrames(1);

                Image countyMapImage = (Image)mapImage.Duplicate(); // This has to be down here because it needs a new copy every for each county.
                countyMapImage.Convert(Image.Format.Rgba8); // Convert the format so that it works with Blit Rect.

                countyData.countyId = countyID;
                countyID++;

                GD.Print(countyMapImage.GetFormat());
                GD.Print("Mask format: " + countyMaskImage.GetFormat());
                startCountyWidth = mapSize.X;
                startCountyHeight = mapSize.Y;

                countyHeight = 0;
                countyWidth = 0;

                GD.Print("Creating mask for: " + countyData.countyName);
                int mapWidth = mapSize.X;
                int mapHeight = mapSize.Y;

                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Color color = colorCodedMapImage.GetPixel(x, y);

                        if (color.IsEqualApprox(countyData.color))
                        {
                            countyMaskImage.SetPixel(x, y, new Color(1, 1, 1, 0.345f)); // The float is for the a so the county isn't covered in white.

                            if (x < startCountyWidth)
                            {
                                startCountyWidth = x;
                            }
                            if (y < startCountyHeight)
                            {
                                startCountyHeight = y;
                            }

                            if (x > countyWidth)
                            {
                                countyWidth = x;
                            }
                            if (y > countyHeight)
                            {
                                countyHeight = y;
                            }
                        }
                        else
                        {
                            countyMaskImage.SetPixel(x, y, new Color(0, 0, 0, 0));
                            countyMapImage.SetPixel(x, y, new Color(0, 0, 0, 0));
                        }
                    }
                }

                Vector2I startVector2I = new(startCountyWidth, startCountyHeight);
                Vector2I endVector2I = new(countyWidth, countyHeight);
                Vector2I sizeVector2I = new(endVector2I.X - startVector2I.X, endVector2I.Y - startVector2I.Y);
                Rect2I rect2I = new(startVector2I, sizeVector2I);
                rect2I.End += Vector2I.One;

                // Crop the mask image after it has been created.
                Image croppedCountyMaskImage = Image.Create(rect2I.Size.X, rect2I.Size.Y, false, Image.Format.Rgba8);
                croppedCountyMaskImage.BlitRect(countyMaskImage, rect2I, Vector2I.Zero); //startVector2I

                countyData.maskTexture = ImageTexture.CreateFromImage(croppedCountyMaskImage);
                countyData.startMaskPosition = startVector2I;

                // Crop the top map image after it has been created.                
                Image croppedCountyMapImage = Image.Create(rect2I.Size.X, rect2I.Size.Y, false, Image.Format.Rgba8);
                GD.Print("Cropped County Map Top Image Size: " + croppedCountyMapImage.GetSize());

                croppedCountyMapImage.BlitRect(countyMapImage, rect2I, Vector2I.Zero); //startVector2I
                countyData.mapTexture = ImageTexture.CreateFromImage(croppedCountyMapImage);
            }

        }

        private async Task GenerateCounties()
        {
            foreach (CountyData countyData in CountyResourcesAutoLoad.Instance.countyDatas)
            {
                SelectCounty county = (SelectCounty)MapEditorGlobals.Instance.countyPackedScene.Instantiate();

                county.countyData = countyData;
                county.countyData.countyNode = county;
                MapEditorGlobals.Instance.countiesParent.AddChild(county);
                county.Name = $"{countyData.countyId} {countyData.countyName}";
                county.maskSprite.Texture = countyData.maskTexture;
                //county.maskSprite.Position = countyData.startMaskPosition;
                county.maskSprite.Visible = false;

                county.countySprite.Texture = countyData.mapTexture;
                county.countySprite.SelfModulate = Colors.LightGreen;
                county.Position = countyData.startMaskPosition;
                Vector2 countySize = county.maskSprite.Texture.GetSize();
                
                // This takes the counties position and gets the center and added the manual County Overlay Local Position
                // so that it shows up in the right place.
                county.countyOverlayNode2D.Position = county.countyData.countyOverlayLocalPosition + countySize/2;
                county.countySprite.Visible = true;

                // Clear out this data so it isn't keeping extra images.
                countyData.maskTexture = null;
                countyData.mapTexture = null;

                await RootNode.Instance.WaitFrames(1);
            }
            MapEditorControls.Instance.controlsEnabled = true;
        }


    }
}