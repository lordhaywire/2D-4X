using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class DiplomacyControl : Control
{
    public static DiplomacyControl Instance { get; private set; }

    [Export] private PackedScene diplomacyMatrixHBoxContainerScene;
    [Export] private VBoxContainer diplomacyMatrixVBoxContainerParent;
    [Export] private Button closeButton;

    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }

    private void OnDiplomacyControlVisibilityChanged()
    {
        if (Visible)
        {
            PlayerControls.Instance.AdjustPlayerControls(false);
            Clock.Instance.PauseTime();
            GenerateWarMatrix();
        }
        else
        {
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
        }
    }

    public void GenerateWarMatrix()
    {
        ClearWarMatrix();
        foreach (DiplomacyMatrix diplomacyMatrix in Autoload.Instance.playerFactionData.diplomacyMatrices)
        {
            if (diplomacyMatrix.FactionId != Autoload.Instance.playerFactionData.factionId)
            {
                DiplomacyMatrixHBoxContainer diplomacyMatrixHBoxContainer =
                    (DiplomacyMatrixHBoxContainer)diplomacyMatrixHBoxContainerScene.Instantiate();
                diplomacyMatrixVBoxContainerParent.AddChild(diplomacyMatrixHBoxContainer);
                diplomacyMatrixHBoxContainer.targetFactionData =
                    FactionData.GetFactionDataFromId(diplomacyMatrix.FactionId);
                diplomacyMatrixHBoxContainer.factionNameButton.Text = diplomacyMatrix.FactionName;
                diplomacyMatrixHBoxContainer.factionWarButton.Text = diplomacyMatrix.AtWar.ToString();
                diplomacyMatrixHBoxContainer.declareWarButton.Text =
                    Tr(diplomacyMatrix.AtWar == false ? "PHRASE_DECLARE_WAR" : "PHRASE_END_WAR");
                diplomacyMatrixHBoxContainer.declareWarButton.Pressed += () =>
                    ChangeWarStatus(diplomacyMatrixHBoxContainer.targetFactionData);
            }
        }
    }

    private void ChangeWarStatus(FactionData targetFactionData)
    {
        DeclareWarConfirmationControl.Instance.aggressorFactionData = Autoload.Instance.playerFactionData;
        DeclareWarConfirmationControl.Instance.defenderFactionData = targetFactionData;
        DeclareWarConfirmationControl.Instance.Show();
    }

    private void ClearWarMatrix()
    {
        foreach (HBoxContainer hBoxContainer in diplomacyMatrixVBoxContainerParent.GetChildren().Skip(1)
                     .Cast<HBoxContainer>())
        {
            hBoxContainer.QueueFree();
        }
    }

    /*
    private void ConnectWarButtonSignals(Button)
    {

    }
    */

    private void ConnectSignals()
    {
        VisibilityChanged += OnDiplomacyControlVisibilityChanged;
        closeButton.Pressed += OnCloseButtonPressed;
    }

    private void OnCloseButtonPressed()
    {
        Hide();
    }
}