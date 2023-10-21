using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Sprite2D
    {
        public void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if(eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false) 
                {
                    GD.Print("Mouse Left Click/Unclick at: ", Name); // What is this comma doing here?
                    Globals.Instance.countyInfoControl.Show();
                    Globals.Instance.countyNameLabel.Text = Name;
                }
            }
        }
    }
}

