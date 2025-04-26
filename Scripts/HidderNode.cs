using Godot;

namespace PlayerSpace;

public partial class HidderNode : Node
{
    [Export] Control[] nodesToHide;
    public override void _Ready()
    {
        //GD.Print("Hidder Node!");
        foreach(Control control in nodesToHide)
        {
            control.Hide();
        }
    }


}