using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public partial class DeclareWarConfirmationControl : Control
{
    public static DeclareWarConfirmationControl Instance { get; private set; }

    [Export] public Label declareWarConfirmationTitleLabel;

    public FactionData aggressorFactionData;
    public FactionData defenderFactionData;

    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        VisibilityChanged += OnDeclareWarControlVisibilityChanged;
    }

    private void OnDeclareWarControlVisibilityChanged()
    {
        if (Visible)
        {
            UpdateTitle();
        }
    }

    int numApples = 5;

    private void UpdateTitle()
    {
        if (!Diplomacy.IsFactionAtWar(aggressorFactionData, defenderFactionData))
        {
            string factionName = defenderFactionData.factionName;
            string translated = Tr("PHRASE_DO_YOU_WANT_TO_DECLARE_WAR");
            string result = string.Format(translated, factionName);
            declareWarConfirmationTitleLabel.Text = result;
        }
        else
        {
            string factionName = defenderFactionData.factionName;
            string translated = Tr("PHRASE_DO_YOU_WANT_TO_END_WAR");
            string result = string.Format(translated, factionName);
            declareWarConfirmationTitleLabel.Text = result;
        }
    }

    // Once yes has been hit, this is the actual declaration of war.
    private void YesButton()
    {
        Hide();
        if (!Diplomacy.IsFactionAtWar(aggressorFactionData, defenderFactionData))
        {
            Diplomacy.CreateWar(aggressorFactionData, defenderFactionData);
            DiplomacyControl.Instance.GenerateWarMatrix();
        }
        else
        {
            Diplomacy.EndWar(aggressorFactionData, defenderFactionData);
            DiplomacyControl.Instance.GenerateWarMatrix();
        }
    }

    private void NoButton()
    {
        Hide();
    }
}