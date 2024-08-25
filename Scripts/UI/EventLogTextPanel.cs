using Godot;

namespace PlayerSpace;

public partial class EventLogTextPanel : Panel
{
    [Export] public Label logText;

    public static void OnMouseEntered()
    {
        //GD.PrintRich("[tornado][wave][shake]Mouse Entered: Event Log.");
        PlayerControls.Instance.AdjustClickThrough();
        CameraControls.Instance.AdjustZoomEnabled();
    }

    public static void OnMouseExited()
    {
        //GD.PrintRich("[tornado][wave][shake]Mouse Exited: Event Log.");
        PlayerControls.Instance.AdjustClickThrough();
        CameraControls.Instance.AdjustZoomEnabled();
    }
}