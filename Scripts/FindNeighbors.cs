using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class FindNeighbors : Node
    {
        private List<Sprite2D> countySprites = [];

        public override void _Ready()
        {
            FindCountyNeighbors();
        }
        public void FindCountyNeighbors()
        {
            ulong startTime = Time.GetTicksUsec();
            // Add all the sprites to a list.
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                Sprite2D sprite2D = selectCounty.countySprite;
                countySprites.Add(sprite2D);
            }

            // Go through each county and find its neighbors and add them to its neighbors list.
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
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
                            //GD.Print($"{county.Name} overlaps with {sprite.GetParent().Name}");
                            county.neighborCounties.Add((County)sprite.GetParent());
                        }
                    }
                }
            }

            ulong endTime = Time.GetTicksUsec();
            GD.Print("Total time to generate: " + (endTime - startTime));
        }
    }
}