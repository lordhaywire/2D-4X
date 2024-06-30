using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        [ExportGroup("MapEditor")]
        public County countyNode; // See if we can get rid of this somehow.
        [Export] public Color color;
        public Vector2I startMaskPosition; // I think this is the local position....
        [Export] public Vector2I countyOverlayLocalPosition;

        [ExportGroup("County other somethings")]
        [Export] public int countyId;
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        //[Export] public AllEnums.Factions faction;
        [Export] public FactionData factionData;

        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;

        public Globals.ListWithNotify<CountyPopulation> countyPopulationList = [];
        public Globals.ListWithNotify<CountyPopulation> herosInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> armiesInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingHeroList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingArmyList = [];
        public Globals.ListWithNotify<CountyPopulation> deadPeopleList = [];

        public List<Button> spawnedTokenButtons = [];

        public List<CountyImprovementData> allCountyImprovements = []; // This includes all county improvements, even possible ones.
        public List<CountyImprovementData> underConstructionCountyImprovements = [];
        public List<CountyImprovementData> completedCountyImprovements = [];
        public List<Battle> battles = [];

        public int population = 0;
        [Export] public int perishableStorage;
        [Export] public int nonperishableStorage;

        [Export] public int scavengableScrap; // This the total a county has available to scavenge.
        [Export] public int scavengableFood; // This the total a county has available to scavenge.

        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> yesterdaysResources = [];

        // These are used just to pass some data around.  Probably I should find a better way to do this.
        public Texture2D maskTexture;
        public Texture2D mapTexture;

        // We will have to see if this is still used.
        public event Action<bool> CountySelected;

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                if (selected)
                {
                    OnCountySelected(true);
                }
                else
                {
                    OnCountySelected(false);
                }
            }
        }

        private void OnCountySelected(bool isSelected)
        {
            CountySelected?.Invoke(isSelected);
        }

        int idleWorkers;

        public int IdleWorkers
        {
            get { return idleWorkers; }
            set
            {
                idleWorkers = value;
                //GD.Print("Idle workers: " + idleWorkers);
                if (Globals.Instance.SelectedLeftClickCounty?.countyData == this)
                {
                    CountyInfoControl.Instance.UpdateIdleWorkersLabel();
                    GD.Print("Why didn't you update?! " + idleWorkers);
                }
            }
        }

        public void OccationalNeeds()
        {
            PossiblyUseResources(herosInCountyList);
            PossiblyUseResources(armiesInCountyList);
            PossiblyUseResources(countyPopulationList);
        }

        private void PossiblyUseResources(Globals.ListWithNotify<CountyPopulation> peopleUsingResourcesList)
        {
            foreach (CountyPopulation countyPopulation in peopleUsingResourcesList)
            {
                // Check to see if they want the resource, then check if there is enough.
                foreach (KeyValuePair<AllEnums.CountyResourceType, int> keyValuePair in countyPopulation.needs)
                {
                    SkillData skillData = new();
                    // Check to see if they want the resource.

                    if (skillData.Check(keyValuePair.Value) == true)
                    {
                        GD.Print($"Needs Checks: Passed.");
                        if (CheckEnoughOfResource(keyValuePair.Key) == true)
                        {
                            GD.Print("There are enough resources for the needs of a person.");
                            // Use resource.
                            RemoveResourceFromCounty(keyValuePair.Key, Globals.Instance.occationalResourceUsageAmount);

                            // Add happiness.
                            countyPopulation.AddRandomHappiness(1);

                            // Set need back to zero.
                            countyPopulation.needs[keyValuePair.Key] = 0;
                        }
                        else
                        {
                            // Reduce this populations happiness.
                            countyPopulation.RemoveRandomHappiness(1);
                        }
                    }
                    else
                    {
                        // Gain 1d4 amount to the need for the next day.
                        Random random = new();
                        int needIncrease = random.Next(1, Globals.Instance.occationalNeedIncreaseAmount);
                        countyPopulation.needs[keyValuePair.Key] += needIncrease;
                        GD.Print($"Needs Checks: Failed: " + countyPopulation.needs[keyValuePair.Key]);
                    }
                }
            }
        }
        public void RemoveResourceFromCounty(AllEnums.CountyResourceType countyResourceType, int amount)
        {
            resources[countyResourceType].amount -= amount;

            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == countyNode)
            {
                TopBarControl.UpdateTopBarWithCountyResources();
            }
        }
        private bool CheckEnoughOfResource(AllEnums.CountyResourceType resourceType)
        {
            bool enoughResource;
            if (resources[resourceType].amount >= Globals.Instance.occationalResourceUsageAmount)
            {
                enoughResource = true;
            }
            else
            {
                enoughResource = false;
            }
            return enoughResource;
        }

        private FoodLists
            GetListsOfFood()
        {
            List<ResourceData> perishableFoodList = [];
            List<ResourceData> nonperishableFoodList = [];
            foreach (ResourceData resourceData in resources.Values)
            {
                // Is food, and there is some food.
                if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == true && resourceData.amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    perishableFoodList.Add(resourceData);
                }
                else if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == false && resourceData.amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    nonperishableFoodList.Add(resourceData);
                }
            }
            // This a temporary list.
            FoodLists foodlists = new()
            {
                perishableFoodList = perishableFoodList,
                nonperishableFoodList = nonperishableFoodList
            };
            return foodlists;
        }

        public class FoodLists
        {
            public List<ResourceData> perishableFoodList = [];
            public List<ResourceData> nonperishableFoodList = [];
        }
        public void PopulationEatsFood(Globals.ListWithNotify<CountyPopulation> countyPopulationList, int amount)
        {
            Random random = new();
            FoodLists foodLists = GetListsOfFood();
            //GD.Print("Population List count: " + countyPopulationList.Count());
            if (countyPopulationList.Count() < 1)
            {
                //GD.PrintRich($"[pulse freq=5.0 color=green]Population Eats Food: A county population list is empty.[/pulse]");
                return;
            }
            else
            {
                List<CountyPopulation> peopleWhoNeedToDie = [];
                foreach (CountyPopulation countyPopulation in countyPopulationList)
                {
                    if (foodLists.perishableFoodList.Count > 0)
                    {
                        int randomNumber = random.Next(0, foodLists.perishableFoodList.Count);
                        // If the amount of food left is greater then zero they eat something.
                        if (foodLists.perishableFoodList[randomNumber].amount > 2)
                        {
                            foodLists.perishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.perishableFoodList[randomNumber].name}" +
                                $" {foodLists.perishableFoodList[randomNumber].amount}");
                            */
                        }
                        // Person eats first, then the food is removed from the list.
                        else if (foodLists.perishableFoodList[randomNumber].amount == 1)
                        {
                            foodLists.perishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.perishableFoodList[randomNumber].name}" +
                                $" {foodLists.perishableFoodList[randomNumber].amount}");
                            */
                            foodLists.perishableFoodList.Remove(foodLists.perishableFoodList[randomNumber]);
                        }
                        else
                        {
                            GD.Print($"Something is seriously fucked up.");
                        }
                    }
                    else if (foodLists.nonperishableFoodList.Count > 0)
                    {
                        int randomNumber = random.Next(0, foodLists.nonperishableFoodList.Count);
                        // If the amount of food left is greater then zero they eat something.
                        if (foodLists.nonperishableFoodList[randomNumber].amount > 2)
                        {
                            foodLists.nonperishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                        }
                        // Person eats first, then the food is removed from the list.
                        else if (foodLists.nonperishableFoodList[randomNumber].amount == 1)
                        {
                            foodLists.nonperishableFoodList[randomNumber].amount -= amount;
                            /*
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                            foodLists.nonperishableFoodList.Remove(foodLists.nonperishableFoodList[randomNumber]);
                        }
                        else
                        {
                            GD.Print($"Something is seriously fucked up.");
                        }
                    }
                    else
                    {
                        // Starving!
                        //GD.PrintRich($"[rainbow]There is no food at all!");
                        countyPopulation.daysStarving++;
                        //GD.Print($"{countyPopulation.firstName} has starved for {countyPopulation.daysStarving} days.");
                        // This will give each population an additonal -1 to their happiness which works for now.
                        AdjustPopulationHappiness(amount, countyPopulation);
                        if (countyPopulation.daysStarving >= Globals.Instance.daysUntilDamageFromStarvation)
                        {
                            countyPopulation.Hitpoints--;
                            if (countyPopulation.Hitpoints < 1)
                            {
                                peopleWhoNeedToDie.Add(countyPopulation);
                            }
                        }
                    }
                    AdjustPopulationHappiness(amount, countyPopulation);
                }
                KillPeopleWhoNeedToDie(peopleWhoNeedToDie);
            }
        }

        private void KillPeopleWhoNeedToDie(List<CountyPopulation> peopleWhoNeedToDie)
        {
            foreach (CountyPopulation countyPopulation in peopleWhoNeedToDie)
            {
                countyPopulationList.Remove(countyPopulation);
                herosInCountyList.Remove(countyPopulation);
                armiesInCountyList.Remove(countyPopulation);
                deadPeopleList.Add(countyPopulation);
                GD.PrintRich($"[rainbow]{countyPopulation.firstName} {countyPopulation.lastName} has croaked.");
            }
        }

        // This is sort of duplicate code.  It is almost the same as the countyPopulation.AddRandomHappiness.
        private static void AdjustPopulationHappiness(int amount, CountyPopulation countyPopulation)
        {
            //GD.Print($"{countyPopulation.firstName} happiness: {countyPopulation.Happiness}");
            if (amount == Globals.Instance.foodToGainHappiness)
            {
                countyPopulation.Happiness++;
            }
            else if (amount == Globals.Instance.foodToLoseHappiness)
            {
                countyPopulation.Happiness--;
            }
            //GD.Print($"{countyPopulation.firstName} happiness: {countyPopulation.Happiness}");
        }

        public void MoveCountyImprovementToCompletedList(CountyImprovementData countyImprovementData)
        {
            completedCountyImprovements.Add(countyImprovementData);
            underConstructionCountyImprovements.Remove(countyImprovementData);
        }

        internal void CopyResourcesToYesterday()
        {
            // This is a "deep" copy.
            yesterdaysResources = resources.Duplicate(true);
        }
    }
}

