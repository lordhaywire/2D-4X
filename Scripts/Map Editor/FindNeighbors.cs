using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayerSpace;

namespace MapEditorSpace
{
    public partial class FindNeighbors : Node
    {
        public static FindNeighbors Instance { get; private set; }

        private List<Sprite2D> countySprites = [];

        public async override void _Ready()
        {
            Instance = this;
            await FindCountyNeighbors();
        }
        public async Task FindCountyNeighbors()
        {
            ulong startTime = Time.GetTicksUsec();
            // Add all the sprites to a list.
            if (Globals.Instance.countiesParent.GetChildren().Count > 0)
            {
                //LogControl.Instance.UpdateLabel("Starting to find county neighbors.");
                foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
                {
                    Sprite2D sprite2D = county.countySprite;
                    countySprites.Add(sprite2D);
                }

                // Go through each county and find its neighbors and add them to its neighbors list.
                foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
                {
                    await Globals.Instance.WaitFrames(1);
                    Sprite2D countySprite = county.countySprite;
                    foreach (Sprite2D sprite in countySprites)
                    {
                        if (countySprite != sprite)
                        {
                            Rect2 rectA = new(countySprite.GlobalTransform.Origin, countySprite.GetRect().Size);
                            Rect2 rectB = new(sprite.GlobalTransform.Origin, sprite.GetRect().Size);

                            Rect2 overlapRect = rectA.Intersection(rectB);

                            if (overlapRect.Area > 0)
                            {
                                //LogControl.Instance.UpdateLabel($"{county.Name} overlaps with {sprite.GetParent().Name}");
                                county.neighborCounties.Add((County)sprite.GetParent());
                            }
                        }
                    }
                }
            }

            ulong endTime = Time.GetTicksUsec();
            //GD.Print("Total time to generate: " + (endTime - startTime));
        }
    }
}