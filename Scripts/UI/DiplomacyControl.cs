using Godot;
using System;
using System.Collections.Generic;
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

    private void GenerateWarMatrix()
    {
        foreach (DiplomacyMatrix diplomacyMatrix in Autoload.Instance.playerFactionData.diplomacyMatrices)
        {
            DiplomacyMatrixHBoxContainer diplomacyMatrixHBoxContainer = (DiplomacyMatrixHBoxContainer)diplomacyMatrixHBoxContainerScene.Instantiate();
            diplomacyMatrixVBoxContainerParent.AddChild(diplomacyMatrixHBoxContainer);
            diplomacyMatrixHBoxContainer.factionNameButton.Text = diplomacyMatrix.FactionName;
            diplomacyMatrixHBoxContainer.factionWarButton.Text = diplomacyMatrix.AtWar.ToString();
            diplomacyMatrixHBoxContainer.declareWarButton.Text = Tr(diplomacyMatrix.AtWar == false ? "PHRASE_DECLARE_WAR" : "PHRASE_END_WAR");
            //ConnectWarButtonSignals(diplomacyMatrixHBoxContainer.declareWarButton);
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
