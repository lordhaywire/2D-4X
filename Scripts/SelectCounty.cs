using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Sprite2D
    {
        public void MouseClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if(eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false) 
                {
                    GD.Print("Mouse Left Click/Unclick at: ", Name);
                    Globals.Instance.countyInfoControl.Show();
                    Globals.Instance.countyNameLabel.Text = Name;
                }
            }
        }
    }
}

