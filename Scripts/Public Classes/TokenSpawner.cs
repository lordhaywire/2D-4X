using Godot;

namespace PlayerSpace
{
    public class TokenSpawner
    {
        // We could change this so it doesn't return a PopluationData since that is weird.
        public static PopulationData Spawn(County county, PopulationData populationData)
        {
            // Spawning the token.
            Node2D tokenSpawnParent = county.heroSpawn;
            HeroToken spawnedToken = (HeroToken)Globals.Instance.heroToken.Instantiate();
            GD.Print("Hero token button: " + spawnedToken.spawnedTokenButton);
            tokenSpawnParent.AddChild(spawnedToken);

            spawnedToken.populationData = populationData;

            AllTokenTextures.Instance.AssignTokenTextures(spawnedToken);

            populationData.token = spawnedToken;
            populationData.location = county.countyData.countyId; // The populationData should have already have the location.
            spawnedToken.Name = $"{populationData.firstName} {populationData.lastName}";

            // Update the token's name label
            spawnedToken.tokenNameLabel.Text = $"{populationData.firstName} {populationData.lastName}";

            // Spawning the Spawned Token Button
            SpawnedTokenButton spawnedTokenButton 
                = (SpawnedTokenButton)Globals.Instance.spawnedTokenButton.Instantiate();

            if (populationData.IsThisAnArmy() == false)
            {
                county.heroesHBox.AddChild(spawnedTokenButton);
                county.heroesHBox.Show();
            }
            else
            {
                county.armiesHBox.AddChild(spawnedTokenButton);
                county.armiesHBox.Show();
            }
            spawnedTokenButton.populationData = populationData;

            county.countyData.spawnedTokenButtons.Add(spawnedTokenButton);

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
            DecidedIfSelected(county, spawnedToken);

            spawnedTokenButton.UpdateTokenTextures(); // This has to be below the populationData assignment.

            GD.Print($"Is {populationData.firstName} an army leader?" + populationData.IsThisAnArmy());
            return populationData;
        }

        public static void Unspawn(County county, PopulationData populationData)
        {
            county.countyData.spawnedTokenButtons.Remove(populationData.token.spawnedTokenButton);
            populationData.token.QueueFree();
        }

        // This is so that the AI token spawning doesn't make the player select it.
        private static void DecidedIfSelected(County selectCounty, HeroToken spawnedToken)
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
