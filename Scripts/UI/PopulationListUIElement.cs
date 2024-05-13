using Godot;

namespace PlayerSpace
{
    public partial class PopulationListUIElement : MarginContainer
    {
        [Export] private VBoxContainer populationListParent;
        [Export] private PackedScene populationRowButtonPackedScene;
        [Export] private Label populationListTitle;
        
        private void OnVisibilityChanged()
        {
            DestroyPopulationRows(); // Clears out the population, so it doesn't duplicate.
            if (Visible)
            {
                //CountyInfoControl.Instance.DisableSpawnHeroCheckButton(true);
                CountyInfoControl.Instance.populationDescriptionControl.Hide();
                CountyInfoControl.Instance.countyImprovementsPanelControl.Hide();
                PlayerUICanvas.Instance.BattleLogControl.Hide();

                PlayerControls.Instance.AdjustPlayerControls(false);
                Clock.Instance.PauseTime();
                
                if(Globals.Instance.isVisitorList == false)
                {
                    populationListTitle.Text = $"{Globals.Instance.CurrentlySelectedCounty.countyData.countyName} " +
                        $"{AllText.Titles.POPLIST}";
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.herosInCountyList);
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.countyPopulationList);
                }
                else
                {
                    populationListTitle.Text = $"{Globals.Instance.CurrentlySelectedCounty.countyData.countyName} " + 
                        AllText.Titles.VISITORLIST;
                    GeneratePopulationRows(Globals.Instance.CurrentlySelectedCounty.countyData.visitingHeroList);
                }
            }
            else
            {
                //CountyInfoControl.Instance.DisableSpawnHeroCheckButton(false);
                PlayerControls.Instance.AdjustPlayerControls(true);
                Clock.Instance.UnpauseTime();
            }
        }

        private void DestroyPopulationRows()
        {
            for (int i = 2; i < populationListParent.GetChildCount(); i++)
            {
                populationListParent.GetChild(i).QueueFree();
            }
        }

        private void GeneratePopulationRows(Globals.ListWithNotify<CountyPopulation> countyPopulation)
        {
            foreach (CountyPopulation person in countyPopulation)
            {
                PopulationRowButton populationRow = (PopulationRowButton)populationRowButtonPackedScene.Instantiate();

                populationRow.populationNameLabel.Text = $"{person.firstName} {person.lastName}";
                populationRow.ageLabel.Text = person.age.ToString();
                if (person.isMale == true)
                {
                    populationRow.sexLabel.Text = "Male";
                }
                else
                {
                    populationRow.sexLabel.Text = "Female";
                }

                UpdateUnHelpfulPerk(populationRow, person);
                UpdateAttributes(populationRow, person);
                UpdateSkills(populationRow, person);

                if(person.currentImprovement != null)
                {
                    populationRow.currentActivityLabel.Text = $"{person.currentActivity} {person.currentImprovement.improvementName}";
                }
                else
                {
                    populationRow.currentActivityLabel.Text = person.currentActivity;
                }
                if(person.nextImprovement != null)
                {
                    populationRow.nextActivityLabel.Text = $"{person.nextActivity} {person.nextImprovement.improvementName}";
                }
                else
                {
                    populationRow.nextActivityLabel.Text = person.nextActivity;
                }
                populationListParent.AddChild(populationRow);
                PopulationRowButton populationRowButton = populationRow;
                populationRowButton.countyPopulation = person;
            }
        }

        // This will not be shown in to the player eventually.  It is just shown right now for testing.
        // We might need a second testing bar to show these things later.
        private static void UpdateUnHelpfulPerk(PopulationRowButton populationRow, CountyPopulation person)
        {
            foreach (PerkData perkData in person.perks) 
            {
                if(AllPerks.Instance.allPerks[(int)AllEnums.Perks.Unhelpful].perkName == perkData.perkName)
                {
                    populationRow.UnhelpfulLabel.Text = $"{perkData.perkName}";
                }
                else
                {
                    populationRow.UnhelpfulLabel.Text = "Helpful";
                }
            }
        }

        private static void UpdateAttributes(PopulationRowButton populationRow, CountyPopulation person)
        {
            populationRow.loyaltyAttributeLabel.Text = $"{person.loyaltyAttribute}";
        }
        private static void UpdateSkills(PopulationRowButton populationRow, CountyPopulation person)
        {
            populationRow.constructionSkillLabel.Text = $"{person.skills[AllEnums.Skills.Construction].skillLevel}";
            populationRow.coolSkillLabel.Text = $"{person.skills[AllEnums.Skills.Cool].skillLevel}";
            populationRow.farmSkillLabel.Text = $"{person.skills[AllEnums.Skills.Farm].skillLevel}";
            populationRow.fishSkillLabel.Text = $"{person.skills[AllEnums.Skills.Fish].skillLevel}";
            populationRow.lumberjackSkillLabel.Text = $"{person.skills[AllEnums.Skills.Lumberjack].skillLevel}";
            populationRow.researchSkillLabel.Text = $"{person.skills[AllEnums.Skills.Research].skillLevel}";
            populationRow.rifleSkillLabel.Text = $"{person.skills[AllEnums.Skills.Rifle].skillLevel}";
            populationRow.scavengeSkillLabel.Text = $"{person.skills[AllEnums.Skills.Scavenge].skillLevel}";

        }
    }
}