using Godot;

namespace PlayerSpace
{

    public class TokenSpawner
    {
        public SelectToken Spawn(SelectCounty selectCounty, CountyPopulation countyPopulation)
        {
            // Spawning the token.
            Sprite2D tokenSpawnParent = selectCounty.capitalSprite;
            SelectToken spawnedToken = (SelectToken)Globals.Instance.heroToken.Instantiate();
            tokenSpawnParent.AddChild(spawnedToken);
            spawnedToken.Hide();

            // This is bizarre.  I have a feeling this shouldn't be done this way.
            countyPopulation.token = spawnedToken;
            //SelectToken selectToken = (SelectToken)spawnedToken;
            //selectToken.countyPopulation = countyPopulation;
            countyPopulation.location = selectCounty.countyData.countyId;
            spawnedToken.Name = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Update the token's name label
            spawnedToken.tokenNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Spawning the Spawned Token Button
            SpawnedTokenButton spawnedTokenButton = (SpawnedTokenButton)Globals.Instance.spawnedTokenButton.Instantiate();
            if (countyPopulation.isArmyLeader == false)
            {
                selectCounty.heroesHBox.AddChild(spawnedTokenButton);
                selectCounty.heroesHBox.Show();
            }
            else
            {
                selectCounty.armiesHBox.AddChild(spawnedTokenButton);
                selectCounty.armiesHBox.Show();
            }
            //spawnedTokenButton.Icon = spawnedToken.sprite.Texture;
            spawnedTokenButton.tokenIconTextureRect.Texture = spawnedToken.sprite.Texture;
            spawnedTokenButton.countyPopulation = countyPopulation;

            if(selectCounty.heroesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.heroSeparator.Show();
            }

            if (selectCounty.armiesHBox.GetChildren().Count > 1)
            {
                selectCounty.heroTokensControl.armySeparator.Show();
            }

            // Add heroToken to counties spawned hero list
            //selectCounty.heroSpawn.spawnedTokenList.Insert(0, selectToken);
            return spawnedToken;
        }
    }
}