using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [Export] public int countyId; // This is used all over the place.
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        //[Export] public AllEnums.Factions faction;
        [Export] public FactionData factionData;

        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;

        [Export] public Godot.Collections.Array<PopulationData> populationDataList = [];
        [Export] public Godot.Collections.Array<PopulationData> heroesInCountyList = [];
        [Export] public Godot.Collections.Array<PopulationData> armiesInCountyList = [];
        [Export] public Godot.Collections.Array<PopulationData> visitingHeroList = [];
        [Export] public Godot.Collections.Array<PopulationData> visitingArmyList = [];
        [Export] public Godot.Collections.Array<PopulationData> deadPeopleList = [];

        List<PopulationData> possibleWorkers = []; // List of all the idle, helpful and loyal workers for that day.
        [Export] public Godot.Collections.Array<PopulationData> prioritizedWorkers = [];
        readonly List<PopulationData> workersToRemoveFromPossibleWorkers = []; // List to collect county populations to be removed from the possibleWorkers.

        public List<Button> spawnedTokenButtons = [];

        [Export] public Godot.Collections.Array<CountyImprovementData> underConstructionCountyImprovements = [];
        [Export] public Godot.Collections.Array<CountyImprovementData> completedCountyImprovements = [];
        public List<Battle> battles = [];

        public int population = 0;
        [Export] public int perishableStorage;
        [Export] public int nonperishableStorage;

        [Export] public int scavengableRemnants; // This the total a county has available to scavenge.
        [Export] public int scavengableCannedFood; // This the total a county has available to scavenge.

        [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> goods = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> yesterdaysGoods = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> amountOfGoodsUsed = [];

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
                }
            }
        }

        readonly List<PopulationData> peopleWhoNeedToDie = [];

        public void CheckIfCountyImprovementsAreDone()
        {
            List<CountyImprovementData> completedImprovements = [];
            completedImprovements.Clear(); // Why is this here?
            foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovements)
            {
                // If the county improvement is done, make everyone working on it idle.
                // Set their current work to null.
                if (countyImprovementData.CheckIfCountyImprovementDone())
                {
                    foreach (PopulationData populationData in countyImprovementData.populationAtImprovement)
                    {
                        // The population would be on this list because they would be doing construction.
                        if(populationData.isHero != true)
                        {
                            populationData.UpdateActivity(AllEnums.Activities.Idle);
                            populationData.UpdateCurrentCountyImprovement(null);
                        }
                        else
                        {
                            populationData.UpdateActivity(AllEnums.Activities.Work);
                            populationData.UpdateCurrentCountyImprovement(null);

                        }
                    }
                    // Set countyImprovement status to producing.
                    countyImprovementData.SetCountyImprovementComplete(this);
                    // Clear the people building on the county improvement.
                    countyImprovementData.populationAtImprovement.Clear();
                    completedImprovements.Add(countyImprovementData);

                    // Check to only print the event logs of the players county improvements.
                    if (factionData == Globals.Instance.playerFactionData)
                    {
                        EventLog.Instance.AddLog($"{Tr(countyImprovementData.improvementName)} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");
                    }

                    //GD.Print($"Under Construction Improvements - Checking if done : " +
                    //    $"{countyImprovementData.improvementName} : {countyImprovementData.status}");
                }
            }
            // Move the county improvement to the correct list and remove it from the old list.
            MoveCountyImprovementToCompletedList(completedImprovements);
            // Refresh the panel so that heros checkboxes are enabled or disabled.
            CountyInfoControl.Instance.UpdateEverything();
        }

        public void RemoveResourceFromAvailableCountyTotals(AllEnums.CountyGoodType resourceType, int amount)
        {
            if (resourceType == AllEnums.CountyGoodType.CannedFood)
            {
                scavengableCannedFood -= amount;
            }
            else if (resourceType == AllEnums.CountyGoodType.Remnants)
            {
                scavengableRemnants -= amount;
            }
            else
            {
                // // GD.Print("Your resource type is wrong!");
            }
        }

        private static void UpdateWorkLocation(PopulationData populationData, CountyImprovementData countyImprovementData)
        {
            // GD.Print($"{populationData.firstName} is working at {countyImprovementData.improvementName}");
            // This same thing is done multiple times.  We should make it its own method.
            populationData.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.AddPopulationToCountyImprovementList(populationData);
        }
        public void CountIdleWorkers()
        {
            int idleWorkers = 0;
            foreach (PopulationData populationData in populationDataList)
            {
                if (populationData.activity == AllEnums.Activities.Idle)
                {
                    idleWorkers++;
                }
            }
            IdleWorkers = idleWorkers;
        }

        /// <summary>
        /// If there isn't enough food then have the idle people start scavenging & if there is enough
        /// scavengables in this county. It is set to zero because that is easy.  Don't worry about it.
        /// It also checks to see if there already is enough stored in the county.
        /// </summary>
        public void CheckForScavengingFood()
        {
            // Population won't scavenge if the storage is full, or if the county is out of scavengables.
            if (!CheckEnoughCountyScavengables(AllEnums.CountyGoodType.CannedFood)
                || CheckGoodStorageFull(goods[AllEnums.CountyGoodType.CannedFood]))
            {
                return;
            }

            //int amountOfFood = CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
            //// GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);

            foreach (PopulationData populationData in possibleWorkers)
            {
                populationData.UpdateActivity(AllEnums.Activities.Scavenge);
                populationData.UpdateCurrentCountyImprovement(null);
                workersToRemoveFromPossibleWorkers.Add(populationData);
            }
            RemoveWorkersFromPossibleWorkers();
        }

        /// <summary>
        /// If there isn't enough remnants then have the idle people start scavenging & if there is enough
        /// scavengables in this county. It is set to zero because that is easy.  Don't worry about it.
        /// It also checks to see if there already is enough stored in the county.
        /// </summary>
        public void CheckForScavengingRemnants()
        {
            // Population won't scavenge if the storage is full, or if the county is out of scavengables.
            if (!CheckEnoughCountyScavengables(AllEnums.CountyGoodType.Remnants)
                || CheckGoodStorageFull(goods[AllEnums.CountyGoodType.Remnants]))
            {
                return;
            }

            //// GD.Print($"{county.countyData.countyName} Amount of remnants: " + county.countyData.resources[AllEnums.CountyResourceType.Remnants].amount);

            foreach (PopulationData populationData in possibleWorkers)
            {
                populationData.UpdateActivity(AllEnums.Activities.Scavenge);
                populationData.UpdateCurrentCountyImprovement(null);
                workersToRemoveFromPossibleWorkers.Add(populationData);
            }
            RemoveWorkersFromPossibleWorkers();

        }
        public bool CheckEnoughCountyScavengables(AllEnums.CountyGoodType resourceType)
        {
            if (resourceType == AllEnums.CountyGoodType.CannedFood)
            {
                if (scavengableCannedFood > 0)
                {
                    return true;
                }
                return false;
            }
            else if (resourceType == AllEnums.CountyGoodType.Remnants)
            {
                if (scavengableRemnants > 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                // GD.Print($"[color=red]Something in Check Enough County Scavengables has gone horribly wrong.[/color]");
                return false;
            }
        }
        public int CountFactionResourceOfType(AllEnums.FactionGoodType resourceType)
        {
            int amount = 0;
            foreach (GoodData resourceData in goods.Values)
            {
                if (resourceData.factionGoodType == resourceType)
                {
                    amount += resourceData.Amount;
                    //// GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        public int CountUsedFactionResourceOfType(AllEnums.FactionGoodType goodType)
        {
            int amount = 0;
            foreach (GoodData goodData in amountOfGoodsUsed.Values)
            {
                if (goodData.factionGoodType == goodType)
                {
                    amount += goodData.Amount;
                    //// GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        private static bool EnoughStored(int amountOfStored, int resourceBeforeScavenge)
        {
            if (amountOfStored < resourceBeforeScavenge)
            {
                return false;
            }
            return true;
        }
        public void CheckForConstruction()
        {
            foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovements)
            {
                AssignWorkersToCountyImprovement(countyImprovementData);
            }
        }

        private void RemoveWorkersFromPossibleWorkers()
        {
            // Remove the collected items from the possibleWorkers list
            foreach (PopulationData populationData in workersToRemoveFromPossibleWorkers)
            {
                possibleWorkers.Remove(populationData);
            }
            workersToRemoveFromPossibleWorkers.Clear();
        }

        public void FindIdlePopulation()
        {
            // Why is this here?
            workersToRemoveFromPossibleWorkers.Clear();
            // Go through each person in the county.
            foreach (PopulationData populationData in populationDataList)
            {
                // Go through everyone and if they are idle, helpful and loyal add them to the possibleWorkers list.
                if (populationData.activity == AllEnums.Activities.Idle
                    && populationData.CheckWillWorkLoyalty() == true
                    && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    GD.Print($"{countyName}: {populationData.firstName} is idle, is loyal and is not unhelpful.");
                    possibleWorkers.Add(populationData);
                }
            }
        }

        public void ClearPossibleWorkersList()
        {
            possibleWorkers.Clear(); // Clear the list at the start of each day.
        }

        public bool CheckIfImprovementTypeNeedsWorkers(CountyImprovementData countyImprovementData)
        {
            if (countyImprovementData.CheckIfResearchImprovement() == true)
            {
                return true;
            }
            if (countyImprovementData.CheckIfStorageImprovement() == true
               || CheckGoodStorageFull(goods[countyImprovementData.countyResourceType]) == true)
            {
                return false;
            }
            return true;
        }
        public void CheckForPreferredWork()
        {
            //GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
            //GD.Print($"Completed County Improvements: {completedCountyImprovements.Count}");
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                if (CheckIfImprovementTypeNeedsWorkers(countyImprovementData) == false
                    || countyImprovementData.CheckIfStatusLowStockpiledGoods() == true)
                {
                    continue;
                }

                foreach (PopulationData populationData in possibleWorkers)
                {
                    // If they have the preferred skill, they are added to the county improvement
                    // and marked for removal from the possibleWorkers list.
                    // It needs to check if workers full here, so that it doesn't add extra people to the 
                    // county improvement.
                    if (countyImprovementData.CheckIfWorkersFull() == false)
                    {
                        if (populationData.preferredSkill.skill == countyImprovementData.workSkill)
                        {
                            populationData.UpdateActivity(AllEnums.Activities.Work);
                            UpdateWorkLocation(populationData, countyImprovementData);

                            /*GD.Print($"{populationData.firstName} {populationData.lastName} preferred work is " +
                                $"{populationData.preferredSkill.skillName} and they are " +
                                $"{populationData.GetActivityName()} at " +
                                $"{populationData.currentCountyImprovement.improvementName}");
                            */

                            workersToRemoveFromPossibleWorkers.Add(populationData);
                        }
                    }

                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        public void CheckForAnyWork()
        {
            //GD.Print("Possible Workers List Count: " + possibleWorkers.Count);
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                if (CheckIfImprovementTypeNeedsWorkers(countyImprovementData) == false
                    || countyImprovementData.CheckIfStatusLowStockpiledGoods() == true)
                {
                    GD.Print("Low Stockpiled Goods, or the improvement is storage or research.");
                    continue;
                }
                foreach (PopulationData populationData in possibleWorkers)
                {
                    // It needs to check if county improvement's workers are full,
                    // so that it doesn't add extra people to the 
                    // county improvement.
                    if (countyImprovementData.CheckIfWorkersFull() == false)
                    {
                        populationData.UpdateActivity(AllEnums.Activities.Work);
                        UpdateWorkLocation(populationData, countyImprovementData);
                        workersToRemoveFromPossibleWorkers.Add(populationData);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private static bool CheckGoodStorageFull(GoodData countyResourceData)
        {

            if (countyResourceData.Amount >= countyResourceData.MaxAmount)
            {
                return true;
            }
            return false;
        }
        public void SubtractCountyResources()
        {
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in goods)
            {
                amountOfGoodsUsed[keyValuePair.Key].Amount = goods[keyValuePair.Key].Amount -
                    yesterdaysGoods[keyValuePair.Key].Amount;
            }
            /*
            if (factionData.isPlayer)
            {
                GD.Print("After subtraction yesterdays vegetables is: "
                    + yesterdaysCountyResources[AllEnums.CountyResourceType.Vegetables].Amount);
            }
            */
        }

        public void OccationalNeeds()
        {
            PossiblyUseResources(heroesInCountyList);
            PossiblyUseResources(armiesInCountyList);
            PossiblyUseResources(populationDataList);
        }

        private void PossiblyUseResources(Godot.Collections.Array<PopulationData> peopleUsingResourcesList)
        {
            foreach (PopulationData populationData in peopleUsingResourcesList)
            {
                // Go through all of their needs and skill check against it and if they pass
                // , they use the resource that is needed.
                foreach (KeyValuePair<AllEnums.CountyGoodType, int> keyValuePair in populationData.needs)
                {
                    // Check to see if they want the resource.
                    if (SkillData.Check(populationData, keyValuePair.Value
                        , AllEnums.Attributes.MentalStrength, true) == true)
                    {
                        //GD.Print($"Needs Checks: Passed.");
                        if (CheckEnoughOfResource(keyValuePair.Key) == true)
                        {
                            //GD.Print("There are enough resources for the needs of a person.");
                            // Use resource.
                            RemoveResourceFromCounty(keyValuePair.Key, Globals.Instance.occationalResourceUsageAmount);

                            // Add happiness.
                            populationData.AddRandomHappiness(1);

                            // Set need back to zero.
                            populationData.needs[keyValuePair.Key] = 0;
                        }
                        else
                        {
                            // Reduce this populations happiness.
                            populationData.RemoveRandomHappiness(1);
                        }
                    }
                    else
                    {
                        // Gain 1d4 amount to the need for the next day.
                        Random random = new();
                        int needIncrease = random.Next(1, Globals.Instance.occationalNeedIncreaseAmount);
                        populationData.needs[keyValuePair.Key] += needIncrease;
                        //GD.Print($"Needs Checks: Failed: " + populationData.needs[keyValuePair.Key]);
                    }
                }
            }
        }
        public void RemoveResourceFromCounty(AllEnums.CountyGoodType countyResourceType, int amount)
        {
            goods[countyResourceType].Amount -= amount;

            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == countyNode)
            {
                TopBarControl.Instance.UpdateTopBarGoodLabels();
            }
        }

        // This counts and compares to a global variable if there is enough of that resource.
        // We currently just have one number for a minimum of a resource, but we probably
        // should figure out a way for each different type.
        // We could actually put it in the resourceData, so each resource would know the minimum amount
        // the county needs.
        public bool CheckEnoughCountyFactionResource(AllEnums.FactionGoodType resourceType)
        {
            int amountOfResource = CountFactionResourceOfType(resourceType);
            return amountOfResource >= Globals.Instance.minimumFood;
        }

        private bool CheckEnoughOfResource(AllEnums.CountyGoodType resourceType)
        {
            bool enoughResource;
            if (goods[resourceType].Amount >= Globals.Instance.occationalResourceUsageAmount)
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
            List<GoodData> perishableFoodList = [];
            List<GoodData> nonperishableFoodList = [];
            foreach (GoodData goodData in goods.Values)
            {
                // Is food, and there is some food.
                if (goodData.factionGoodType == AllEnums.FactionGoodType.Food
                    && goodData.perishable == AllEnums.Perishable.Perishable && goodData.Amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    perishableFoodList.Add(goodData);
                }
                else if (goodData.factionGoodType == AllEnums.FactionGoodType.Food
                    && goodData.perishable == AllEnums.Perishable.Nonperishable && goodData.Amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    nonperishableFoodList.Add(goodData);
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
            public List<GoodData> perishableFoodList = [];
            public List<GoodData> nonperishableFoodList = [];
        }
        public void PopulationEatsFood(Godot.Collections.Array<PopulationData> populationDataList, int amount)
        {
            Random random = new();
            FoodLists foodLists = GetListsOfFood();
            //GD.Print("Population List count: " + populationDataList.Count());
            if (populationDataList.Count < 1)
            {
                GD.PrintRich($"[pulse freq=5.0 color=green]Population Eats Food: " +
                    $"A population list such as herolist or armylist is empty.[/pulse]");
                return;
            }
            else
            {
                peopleWhoNeedToDie.Clear();
                foreach (PopulationData populationData in populationDataList)
                {
                    if (foodLists.perishableFoodList.Count > 0)
                    {
                        // Sort the lists to the food with the most is used first.
                        foodLists.perishableFoodList.Sort((x, y) => y.Amount.CompareTo(x.Amount));
                        foodLists.nonperishableFoodList.Sort((x, y) => y.Amount.CompareTo(x.Amount));

                        // Have the people eat the perishable food that has the most amount.
                        if (foodLists.perishableFoodList[0].Amount > amount)
                        {
                            foodLists.perishableFoodList[0].Amount -= amount;
                            // If there is not enough food left for the next person, remove the food from the list.
                            if (foodLists.perishableFoodList[0].Amount < amount)
                            {
                                foodLists.perishableFoodList.Remove(foodLists.nonperishableFoodList[0]);
                            }
                        }
                        // Check to see if there is enough nonparishable food.
                        else if (foodLists.nonperishableFoodList.Count > 0)
                        {
                            if (foodLists.nonperishableFoodList[0].Amount > amount)
                            {
                                // The Population eats nonperishable food.
                                foodLists.nonperishableFoodList[0].Amount -= amount;

                                // Check to see if there is enough nonperishable food for the next person,
                                // otherwise remove it.
                                if (foodLists.nonperishableFoodList[0].Amount < amount)
                                {
                                    foodLists.nonperishableFoodList.Remove(foodLists.nonperishableFoodList[0]);
                                }
                            }
                        }
                        else
                        {
                            // There is no food so this person starves.
                            GD.PrintRich($"[color=red]People Eat Food - Perishable: Starvation![/color]");
                            Starvation(populationData, amount);
                        }
                    }
                    else if (foodLists.nonperishableFoodList.Count > 0)
                    {
                        foodLists.nonperishableFoodList.Sort((x, y) => y.Amount.CompareTo(x.Amount));

                        // If the amount of food left is greater then zero they eat something.
                        if (foodLists.nonperishableFoodList[0].Amount > amount)
                        {
                            // The County Population eats nonperishable food.
                            foodLists.nonperishableFoodList[0].Amount -= amount;

                            // Check to see if there is enough nonperishable food for the next person,
                            // otherwise remove it.
                            if (foodLists.nonperishableFoodList[0].Amount < amount)
                            {
                                foodLists.nonperishableFoodList.Remove(foodLists.nonperishableFoodList[0]);
                            }
                            /*
                            GD.Print($"{populationData.firstName} {populationData.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                        }
                        else
                        {
                            // There is no food so this person starves.
                            Starvation(populationData, amount);
                            GD.PrintRich($"[color=red]People Eat Food - Nonperishable: Starvation![/color]");
                        }
                    }
                    else
                    {
                        // Starving!
                        Starvation(populationData, amount);
                    }
                    AdjustPopulationHappiness(amount, populationData);
                }
                KillPeopleWhoNeedToDie(peopleWhoNeedToDie);
            }
        }

        private void Starvation(PopulationData populationData, int amount)
        {
            //GD.PrintRich($"[rainbow]There is no food at all!");
            //GD.Print($"{populationData.firstName} has starved for {populationData.daysStarving} days.");
            // This will give each population an additonal -1 to their happiness which works for now.
            AdjustPopulationHappiness(amount, populationData);
            if (populationData.daysStarving >= Globals.Instance.daysUntilDamageFromStarvation)
            {
                populationData.hitpoints--;
                // This should be its own method in populationData that kills the population.
                if (populationData.hitpoints < 1)
                {
                    peopleWhoNeedToDie.Add(populationData);
                }
            }
            populationData.daysStarving++;
        }

        private void KillPeopleWhoNeedToDie(List<PopulationData> peopleWhoNeedToDie)
        {
            foreach (PopulationData populationData in peopleWhoNeedToDie)
            {
                populationData.factionData.RemoveHeroFromAllHeroesList(populationData);
                populationDataList.Remove(populationData);
                heroesInCountyList.Remove(populationData);
                armiesInCountyList.Remove(populationData);
                deadPeopleList.Add(populationData);
                //GD.PrintRich($"[color=red]{populationData.firstName} {populationData.lastName} has croaked.[/color]");
            }
        }

        // This is sort of duplicate code.  It is almost the same as the populationData.AddRandomHappiness.
        private static void AdjustPopulationHappiness(int amount, PopulationData populationData)
        {
            //GD.Print($"{populationData.firstName} happiness: {populationData.Happiness}");
            if (amount == Globals.Instance.foodToGainHappiness)
            {
                populationData.Happiness++;
            }
            else if (amount == Globals.Instance.foodToLoseHappiness)
            {
                populationData.Happiness--;
            }
            //GD.Print($"{populationData.firstName} happiness: {populationData.Happiness}");
        }

        public void AddPopulationDataToPossibleWorkersList(PopulationData populationData)
        {
            possibleWorkers.Add(populationData);
        }
        public void MoveCountyImprovementToCompletedList(List<CountyImprovementData> countyImprovementDataToRemove)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDataToRemove)
            {
                completedCountyImprovements.Add(countyImprovementData);
                underConstructionCountyImprovements.Remove(countyImprovementData);
            }
        }

        public void CopyCountyResourcesToYesterday()
        {
            // Creating a deep copy of the dictionary
            yesterdaysGoods = [];
            foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in goods)
            {
                yesterdaysGoods.Add(keyValuePair.Key, new GoodData
                {
                    goodName = keyValuePair.Value.goodName,
                    description = keyValuePair.Value.description,
                    countyGoodType = keyValuePair.Value.countyGoodType,
                    factionGoodType = keyValuePair.Value.factionGoodType,
                    perishable = keyValuePair.Value.perishable,
                    Amount = keyValuePair.Value.Amount,
                    MaxAmount = keyValuePair.Value.MaxAmount,
                });
            }
            /*
            if (factionData.isPlayer)
            {
                GD.Print("Yesterday's Vegetables: " + yesterdaysCountyResources[AllEnums.CountyResourceType.Vegetables].Amount);
                GD.Print("This Vegetables should be the same as yesterdays: " + countyResources[AllEnums.CountyResourceType.Vegetables].Amount);
            }
            */
        }

        public static void CheckForHealing(Godot.Collections.Array<PopulationData> possibleHurtPopulationList)
        {
            foreach (PopulationData populationData in possibleHurtPopulationList)
            {
                if (populationData.hitpoints < populationData.maxHitpoints && populationData.daysStarving < 1)
                {
                    populationData.hitpoints++;
                }
            }
        }

        public void AssignPeopleToPrioritizedImprovements()
        {
            GetPrioritizedWorkersList();
            // Check under construction improvements
            CheckForPrioritizedImprovements(underConstructionCountyImprovements);
            // Check finished improvements
            CheckForPrioritizedImprovements(completedCountyImprovements);

            ClearPrioritizedWorkers();
        }

        private void ClearPrioritizedWorkers()
        {
            prioritizedWorkers.Clear();
        }

        public void CheckForPrioritizedImprovements(Godot.Collections.Array<CountyImprovementData> countyImprovements)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovements)
            {
                // If there are low goods stockpiled then don't assign workers.
                if (countyImprovementData.CheckIfStatusLowStockpiledGoods() == false)
                {
                    int maxWorkers = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                        ? countyImprovementData.adjustedMaxBuilders
                        : countyImprovementData.adjustedMaxWorkers;
                    if (countyImprovementData.prioritize == true
                        && countyImprovementData.populationAtImprovement.Count < maxWorkers)
                    {
                        AssignWorkersToCountyImprovement(countyImprovementData);
                    }
                }
            }
        }

        /// <summary>
        /// Assign possible workers to building a county improvement, and remove them from the
        /// possible workers list.
        /// </summary>
        /// <param name="countyImprovementData"></param>
        /// 
        private void AssignWorkersToCountyImprovement(CountyImprovementData countyImprovementData)
        {
            // Determine activity and max workers based on status
            AllEnums.Activities activity = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? AllEnums.Activities.Build
                : AllEnums.Activities.Work;

            int maxWorkers = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? countyImprovementData.adjustedMaxBuilders
                : countyImprovementData.adjustedMaxWorkers;

            // Separate heroes from the general population
            List<PopulationData> heroes = possibleWorkers.Where(pd => pd.isHero).ToList();
            List<PopulationData> nonHeroes = possibleWorkers.Except(heroes).ToList();

            // Sort non-hero workers by relevant skill
            nonHeroes = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? nonHeroes.OrderByDescending(cp => cp.skills[AllEnums.Skills.Construction].skillLevel).ToList()
                : nonHeroes.OrderByDescending(cp => cp.skills[countyImprovementData.workSkill].skillLevel).ToList();

            // Calculate remaining worker slots
            int remainingWorkerSlots = maxWorkers - countyImprovementData.populationAtImprovement.Count;

            // Assign heroes first
            foreach (PopulationData hero in heroes.Take(remainingWorkerSlots))
            {
                hero.RemoveFromCountyImprovement();
                hero.UpdateActivity(activity);
                UpdateWorkLocation(hero, countyImprovementData);
                workersToRemoveFromPossibleWorkers.Add(hero);
                remainingWorkerSlots--;
            }

            // Assign non-hero workers if there are remaining slots
            foreach (PopulationData populationData in nonHeroes.Take(remainingWorkerSlots))
            {
                populationData.RemoveFromCountyImprovement();
                populationData.UpdateActivity(activity);
                UpdateWorkLocation(populationData, countyImprovementData);
                workersToRemoveFromPossibleWorkers.Add(populationData);
            }

            RemoveWorkersFromPossibleWorkers();

            GD.Print($"{countyName} : {countyImprovementData.improvementName} Population At Improvement: "
                + countyImprovementData.populationAtImprovement.Count);
        }

        /*
        private void AssignWorkersToCountyImprovement(CountyImprovementData countyImprovementData)
        {
            // Determine activity and max workers based on status
            AllEnums.Activities activity = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? AllEnums.Activities.Build
                : AllEnums.Activities.Work;

            int maxWorkers = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? countyImprovementData.adjustedMaxBuilders
                : countyImprovementData.adjustedMaxWorkers;

            // Sort possibleWorkers by relevant skill
            possibleWorkers = countyImprovementData.status == AllEnums.CountyImprovementStatus.UnderConstruction
                ? [.. possibleWorkers.OrderByDescending(cp => cp.skills[AllEnums.Skills.Construction].skillLevel)]
                : [.. possibleWorkers.OrderByDescending(cp => cp.skills[countyImprovementData.workSkill].skillLevel)];

            int remainingWorkerSlots = maxWorkers - countyImprovementData.populationAtImprovement.Count;
            GD.Print($"{countyName} : {countyImprovementData.improvementName} Population At Improvement: "
           + countyImprovementData.populationAtImprovement.Count);

            // Assign sorted workers if there is room
            // This is removing them from any possible county improvement they where assigned to.
            foreach (PopulationData populationData in possibleWorkers.Take(remainingWorkerSlots))
            {
                populationData.RemoveFromCountyImprovement();
                populationData.UpdateActivity(activity);
                UpdateWorkLocation(populationData, countyImprovementData);
                workersToRemoveFromPossibleWorkers.Add(populationData);
            }
            RemoveWorkersFromPossibleWorkers();
        }
        */

        /// <summary>
        /// Get all the people who are helpful and loyal for prioritized construction and work.
        /// </summary>
        private void GetPrioritizedWorkersList()
        {
            // Go through each person in the county.
            foreach (PopulationData populationData in populationDataList)
            {
                // Go through everyone and if they are helpful and loyal add them to the possibleWorkers list.
                if (populationData.CheckWillWorkLoyalty() == true
                    && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    GD.Print($"Prioritized: {countyName}: {populationData.firstName} is loyal and is helpful.");
                    prioritizedWorkers.Add(populationData);
                }
            }

            foreach (PopulationData populationData in possibleWorkers)
            {
                if (populationData.isHero)
                {
                    GD.Print($"{populationData.firstName} is a hero and in the possible workers list.");
                }
            }
        }

        public void AssignWorkingHeroes()
        {
            foreach (PopulationData populationData in heroesInCountyList)
            { 
                if(populationData.activity == AllEnums.Activities.Work 
                    && populationData.currentCountyImprovement == null)
                {
                    AddPopulationDataToPossibleWorkersList(populationData);
                    GD.Print($"{populationData.firstName} has added to the possbile workers list.");
                }
            }
        }
    }
}