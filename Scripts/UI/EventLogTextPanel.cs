using Godot;

namespace PlayerSpace;

public partial class EventLogTextPanel : Panel
{
    [Export] public Label logText;

    public static void OnEventLogTextPanelMouseEntered()
    {
        //GD.PrintRich("[tornado][wave][shake]Mouse Entered: Event Log.");
        PlayerControls.Instance.AdjustClickThrough();
        CameraControls.Instance.AdjustZoomEnabled();
    }

    public static void OnEventLogMouseTextPanelExited()
    {
        //GD.PrintRich("[tornado][wave][shake]Mouse Exited: Event Log.");
        PlayerControls.Instance.AdjustClickThrough();
        CameraControls.Instance.AdjustZoomEnabled();
    }
}