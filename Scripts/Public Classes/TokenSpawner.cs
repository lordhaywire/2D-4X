
using Godot;

namespace PlayerSpace
{
    public class TokenSpawner
    {
        public SelectToken Spawn(SelectCounty spawnLocation, CountyPopulation countyPopulation)
        {
            SelectCounty selectCounty = spawnLocation;
            HeroStacker heroSpawnParent = selectCounty.heroSpawn;
            CharacterBody2D spawnedHero = (CharacterBody2D)Globals.Instance.heroToken.Instantiate();
            heroSpawnParent.AddChild(spawnedHero);
            countyPopulation.token = spawnedHero;
            SelectToken selectToken = (SelectToken)spawnedHero;
            selectToken.countyPopulation = countyPopulation;
            selectToken.countyPopulation.location = selectCounty.countyData.countyId;
            spawnedHero.Name = $"{selectToken.countyPopulation.firstName} {selectToken.countyPopulation.lastName}";

            // Update the token's name label
            selectToken.tokenNameLabel.Text = $"{countyPopulation.firstName} {countyPopulation.lastName}";

            // Add heroToken to counties spawned hero list
            selectCounty.heroSpawn.spawnedTokenList.Insert(0, selectToken);
            return selectToken;
        }
    }
}