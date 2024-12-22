using Godot;

namespace PlayerSpace
{
    public class TokenSpawner
    {
        public PopulationData Spawn(County selectCounty, PopulationData populationData)
        {
            // Spawning the token.
            Node2D tokenSpawnParent = selectCounty.heroSpawn;
            SelectToken spawnedToken = (SelectToken)Globals.Instance.heroToken.Instantiate();
            tokenSpawnParent.AddChild(spawnedToken);

            spawnedToken.populationData = populationData;

            AllTokenTextures.Instance.AssignTokenTextures(spawnedToken);

            populationData.token = spawnedToken;
            populationData.location = selectCounty.countyData.countyId;
            spawnedToken.Name = $"{populationData.firstName} {populationData.lastName}";

            // Update the token's name label
            spawnedToken.tokenNameLabel.Text = $"{populationData.firstName} {populationData.lastName}";

            // Spawning the Spawned Token Button
            SpawnedTokenButton spawnedTokenButton = (SpawnedTokenButton)Globals.Instance.spawnedTokenButton.Instantiate();
            if (populationData.IsArmyLeader == false)
            {
                selectCounty.heroesHBox.AddChild(spawnedTokenButton);
                selectCounty.heroesHBox.Show();
            }
            else
            {
                selectCounty.armiesHBox.AddChild(spawnedTokenButton);
                selectCounty.armiesHBox.Show();
            }
            spawnedTokenButton.populationData = populationData;

            selectCounty.countyData.spawnedTokenButtons.Add(spawnedTokenButton);

            // The token needs to keep track of this button.
            spawnedToken.spawnedTokenButton = spawnedTokenButton;

            // Add separators depending on if there are more then 1 hero or army.
            /*
            if (selectCounty.heroesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.heroSeparator.Show();
            }

            if (selectCounty.armiesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.armySeparator.Show();
            }
            */

            // This is at the bottom just in case the Getter Setter is fired too fast.
            DecidedIfSelected(selectCounty, spawnedToken);

            spawnedTokenButton.UpdateTokenTextures(); // This has to be below the populationData assignment.

            GD.Print($"Is {populationData.firstName} an army leader?" + populationData.IsArmyLeader);
            return populationData;
        }


        // This is so that the AI token spawning doesn't make the player select it.
        private static void DecidedIfSelected(County selectCounty, SelectToken spawnedToken)
        {
            GD.Print($"{selectCounty.countyData.factionData.factionName} vs {Globals.Instance.playerFactionData.factionName}");
            if(selectCounty.countyData.factionData == Globals.Instance.playerFactionData)
            {
                spawnedToken.IsSelected = true;
                GD.Print("Spawned Token Button Token's Name: " + spawnedToken.populationData.firstName + spawnedToken.IsSelected);
            }
        }
    }
}

//ChangeTokenColorToFactionColor(spawnedToken, selectCounty);

// This was an idea.
/*
private void ChangeTokenColorToFactionColor(SelectToken spawnedToken, SelectCounty selectCounty)
{
    spawnedToken.sprite.SelfModulate = selectCounty.countyData.factionData.factionColor;
    spawnedToken.spawnedTokenButton.tokenIconTextureRect.SelfModulate = selectCounty.countyData.factionData.factionColor;
}
*/