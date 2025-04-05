using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class PopulationRowButton : Button
{
    public PopulationData populationData;
    [Export] public Label populationNameLabel;
    [Export] public Label ageLabel;
    [Export] public Label sexLabel;
    [Export] public Label unhelpfulLabel;
    [Export] public Label loyaltyAttributeLabel;
    public List<Label> skillLabels = [];

    [Export] public Label currentActivityLabel;

    public override void _Ready()
    {
        GetAllSkillLabels();
    }

    /// <summary>
    /// This is setup this way so that it skills the first 5 labels and the last label.
    /// </summary>
    private void GetAllSkillLabels()
    {
        skillLabels = GetChild(0).GetChildren().Skip(5).Cast<Label>().ToList();
        skillLabels.RemoveAt(skillLabels.Count - 1);
    }

    /*
    private void GetAllSkillLabels()
    {
        foreach(Label label in GetChild(0).GetChildren().Skip(5).Cast<Label>())
        {
            skillLabels.Add(label);
        }
    }
    */

    private void OnButtonClick()
    {
        CountyInfoControl.Instance.populationDescriptionControl.Show();
        CountyInfoControl.Instance.populationListMarginContainer.Hide();
        PopulationDescriptionControl.Instance.populationData = populationData;
    }
}