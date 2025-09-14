using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public class TokenSpawner
{
    // We could change this so it doesn't return a PopluationData since that is weird.
    public static PopulationData Spawn(County county, PopulationData populationData)
    {
        // Spawning the token.
        Node2D tokenSpawnParent = county.heroSpawn;
        HeroToken spawnedToken = (HeroToken)Globals.Instance.heroToken.Instantiate();
        GD.Print("Global Hero Token: " + Globals.Instance.heroToken);
        GD.Print("Spawned token button: " + spawnedToken.spawnedTokenButton);
        tokenSpawnParent.AddChild(spawnedToken);

        spawnedToken.populationData = populationData;

        AllTokenTextures.Instance.AssignTokenTextures(spawnedToken);

        populationData.heroToken = spawnedToken;
        populationData.Location =
            county.countyData.countyId; // The populationData should have already have the location.
        spawnedToken.Name = $"{populationData.firstName} {populationData.lastName}";

        // Update the token's name label
        spawnedToken.tokenNameLabel.Text = $"{populationData.firstName} {populationData.lastName}";

        // Spawning the Spawned Token Button
        SpawnedTokenButton spawnedTokenButton
            = (SpawnedTokenButton)Globals.Instance.spawnedTokenButton.Instantiate();

        county.heroesHBox.AddChild(spawnedTokenButton);
        county.heroesHBox.Show();

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

        return populationData;
    }

    public static void Unspawn(County county, PopulationData populationData)
    {
        county.countyData.spawnedTokenButtons.Remove(populationData.heroToken.spawnedTokenButton);
        GD.Print("Unspawn Spawned Token Buttons Count: " + county.countyData.spawnedTokenButtons.Count);
        populationData.heroToken.spawnedTokenButton.QueueFree();
        populationData.heroToken.QueueFree();
        populationData.heroToken = null;
        Globals.Instance.SelectedCountyPopulation = null;
        GD.Print("Token Spawner: " + populationData?.heroToken);
    }

    // This is so that the AI token spawning doesn't make the player select it.
    private static void DecidedIfSelected(County county, HeroToken spawnedToken)
    {
        FactionData factionData =
            SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(county.countyData.factionId);
        GD.Print($"{factionData.factionName} vs {Autoload.Instance.playerFactionData.factionName}");
        if (Globals.CheckIfPlayerFaction(factionData))
        {
            spawnedToken.IsSelected = true;
            GD.Print($"Spawned Token Button Token's Name: {spawnedToken.populationData.firstName} {spawnedToken.IsSelected}");
        }
    }
}