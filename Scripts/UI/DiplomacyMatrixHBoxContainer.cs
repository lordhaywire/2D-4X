using Godot;

namespace PlayerSpace;
public partial class DiplomacyMatrixHBoxContainer : HBoxContainer
{
    public Button factionNameButton;
    public Button factionWarButton;
    public Button declareWarButton;

    public FactionData targetFactionData;
    public override void _Ready()
    {
        factionNameButton = (Button)GetChild(0);
        factionWarButton = (Button)GetChild(1);
        declareWarButton = (Button)GetChild(2);
    }
}
