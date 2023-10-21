using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Sprite2D
    {
        [Export] public CountyData countyData;


        public override void _Ready()
        {
            TestSomething();
        }
        private void TestSomething()
        {
            countyData.countyPopulation.Add(0, new CountyPopulation("Fred", "Dickface", true, 25, true, false, false, true, 50, "None", "None", false));
        }
        public void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if(eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false) 
                {
                    //GD.Print("Mouse Left Click/Unclick at: ", Name); // What is this comma doing here?
                    Globals.Instance.countyInfoControl.Show();
                    Globals.Instance.countyNameLabel.Text = Name;
                    GD.Print($"First Name: {countyData.countyPopulation[0].firstName} Last Name: {countyData.countyPopulation[0].lastName}");
                }
            }
        }
    }
}

