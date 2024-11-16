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

        public Globals.ListWithNotify<CountyPopulation> countyPopulationList = [];
        public Globals.ListWithNotify<CountyPopulation> herosInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> armiesInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingHeroList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingArmyList = [];
        public Globals.ListWithNotify<CountyPopulation> deadPeopleList = [];

        List<CountyPopulation> possibleWorkers = []; // List of all the idle, helpful and loyal workers for that day.
        readonly List<CountyPopulation> workersToRemoveFromPossibleWorkers = []; // List to collect county populations to be removed from the possibleWorkers.

        public List<Button> spawnedTokenButtons = [];

        public List<CountyImprovementData> underConstructionCountyImprovements = [];
        public List<CountyImprovementData> completedCountyImprovements = [];
        public List<Battle> battles = [];

        public int population = 0;
        [Export] public int perishableStorage;
        [Export] public int nonperishableStorage;

        [Export] public int scavengableRemnants; // This the total a county has available to scavenge.
        [Export] public int scavengableCannedFood; // This the total a county has available to scavenge.

        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> countyResources = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> yesterdaysCountyResources = [];
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> amountUsedCountyResources = [];

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

        readonly List<CountyPopulation> peopleWhoNeedToDie = [];

        public void CheckIfCountyImprovementsAreDone()
        {
            List<CountyImprovementData> completedImprovements = [];
            completedImprovements.Clear();
            foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovements)
            {
                // If the county improvement is done, make everyone working on it idle.
                // Set their current work to null.
                if (countyImprovementData.CheckIfCountyImprovementDone())
                {
                    foreach (CountyPopulation countyPopulation in countyImprovementData.populationAtImprovement)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                        countyPopulation.UpdateCurrentCountyImprovement(null);
                    }
                    // Set countyImprovement status to producing.  I think this is going to fuck everything up.
                    countyImprovementData.SetCountyImprovementComplete(this);
                    // Clear the people on the county improvement list.
                    countyImprovementData.populationAtImprovement.Clear();
                    completedImprovements.Add(countyImprovementData);

                    // Check to only print the event logs of the players county improvements.
                    if (factionData == Globals.Instance.playerFactionData)
                    {
                        EventLog.Instance.AddLog($"{Tr(countyImprovementData.improvementName)} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");
                    }
                }
            }
            // Move the county improvement to the correct list and remove it from the old list.
            MoveCountyImprovementToCompletedList(completedImprovements);
        }

        public void RemoveResourceFromAvailableCountyTotals(AllEnums.CountyResourceType resourceType, int amount)
        {
            if (resourceType == AllEnums.CountyResourceType.CannedFood)
            {
                scavengableCannedFood -= amount;
            }
            else if (resourceType == AllEnums.CountyResourceType.Remnants)
            {
                scavengableRemnants -= amount;
            }
            else
            {
                // // GD.Print("Your resource type is wrong!");
            }
        }

        private static void UpdateWorkLocation(CountyPopulation countyPopulation, CountyImprovementData countyImprovementData)
        {
            // GD.Print($"{countyPopulation.firstName} is working at {countyImprovementData.improvementName}");
            // This same thing is done multiple times.  We should make it its own method.
            countyPopulation.UpdateCurrentCountyImprovement(countyImprovementData);
            countyImprovementData.AddPopulationToCountyImprovementList(countyPopulation);
        }
        public void CountIdleWorkers()
        {
            int idleWorkers = 0;
            foreach (CountyPopulation person in countyPopulationList)
            {
                if (person.activity == AllEnums.Activities.Idle)
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
            if (!CheckEnoughCountyScavengables(AllEnums.CountyResourceType.CannedFood)
                || CheckResourceStorageFull(countyResources[AllEnums.CountyResourceType.CannedFood]))
            {
                return;
            }

            //int amountOfFood = CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
            //// GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);

            foreach (CountyPopulation countyPopulation in possibleWorkers)
            {
                countyPopulation.UpdateActivity(AllEnums.Activities.Scavenge);
                countyPopulation.UpdateCurrentCountyImprovement(null);
                workersToRemoveFromPossibleWorkers.Add(countyPopulation);
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
            if (!CheckEnoughCountyScavengables(AllEnums.CountyResourceType.Remnants)
                || CheckResourceStorageFull(countyResources[AllEnums.CountyResourceType.Remnants]))
            {
                return;
            }

            //// GD.Print($"{county.countyData.countyName} Amount of remnants: " + county.countyData.resources[AllEnums.CountyResourceType.Remnants].amount);

            foreach (CountyPopulation countyPopulation in possibleWorkers)
            {
                countyPopulation.UpdateActivity(AllEnums.Activities.Scavenge);
                countyPopulation.UpdateCurrentCountyImprovement(null);
                workersToRemoveFromPossibleWorkers.Add(countyPopulation);
            }
            RemoveWorkersFromPossibleWorkers();

        }
        public bool CheckEnoughCountyScavengables(AllEnums.CountyResourceType resourceType)
        {
            if (resourceType == AllEnums.CountyResourceType.CannedFood)
            {
                if (scavengableCannedFood > 0)
                {
                    return true;
                }
                return false;
            }
            else if (resourceType == AllEnums.CountyResourceType.Remnants)
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
        public int CountFactionResourceOfType(AllEnums.FactionResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in countyResources.Values)
            {
                if (resourceData.factionResourceType == resourceType)
                {
                    amount += resourceData.Amount;
                    //// GD.Print($"{countyData.countyName} is counting food: {resourceData.name} {resourceData.amount}");
                }
            }
            return amount;
        }

        public int CountUsedFactionResourceOfType(AllEnums.FactionResourceType resourceType)
        {
            int amount = 0;
            foreach (CountyResourceData resourceData in amountUsedCountyResources.Values)
            {
                if (resourceData.factionResourceType == resourceType)
                {
                    amount += resourceData.Amount;
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
            foreach (CountyPopulation countyPopulation in workersToRemoveFromPossibleWorkers)
            {
                possibleWorkers.Remove(countyPopulation);
            }
            workersToRemoveFromPossibleWorkers.Clear();
        }

        public void FindIdlePopulation()
        {
            possibleWorkers.Clear(); // Clear the list at the start of each county.
            workersToRemoveFromPossibleWorkers.Clear();
            // Go through each person in the county.
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                // Go through everyone and if they are idle, helpful and loyal add them to the possibleWorkers list.
                if (countyPopulation.activity == AllEnums.Activities.Idle
                    && countyPopulation.CheckWillWorkLoyalty() == true
                    && countyPopulation.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    // GD.Print($"{countyName}: {countyPopulation.firstName} is idle, is loyal and is not unhelpful.");
                    possibleWorkers.Add(countyPopulation);
                }
            }
        }

        public void CheckForPreferredWork()
        {
            //GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
            //GD.Print($"Completed County Improvements: {completedCountyImprovements.Count}");
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                GD.Print($"Preferred Work: {countyImprovementData.improvementName}");
                if (countyImprovementData.countyResourceType == AllEnums.CountyResourceType.None
                    || countyImprovementData.CheckIfStorageImprovement() == true
                    || CheckResourceStorageFull(countyResources[countyImprovementData.countyResourceType]) == true)
                {
                    return;
                }
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    // If they have the preferred skill, they are added to the county improvement
                    // and marked for removal from the possibleWorkers list.
                    if (countyPopulation.preferredSkill.skill == countyImprovementData.workSkill)
                    {
                        if (countyImprovementData.populationAtImprovement.Count
                            < countyImprovementData.adjustedMaxWorkers)
                        {
                            countyPopulation.UpdateActivity(AllEnums.Activities.Work);
                            UpdateWorkLocation(countyPopulation, countyImprovementData);

                            /*GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} preferred work is " +
                                $"{countyPopulation.preferredSkill.skillName} and they are " +
                                $"{countyPopulation.GetActivityName()} at " +
                                $"{countyPopulation.currentCountyImprovement.improvementName}");
                            */

                            workersToRemoveFromPossibleWorkers.Add(countyPopulation);
                        }
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }

        private static bool CheckResourceStorageFull(CountyResourceData countyResourceData)
        {
            if (countyResourceData.Amount >= countyResourceData.MaxAmount)
            {
                return true;
            }
            return false;
        }

        public void CheckForAnyWork()
        {
            //GD.Print("Possible Workers List Count: " + possibleWorkers.Count);
            foreach (CountyImprovementData countyImprovementData in completedCountyImprovements)
            {
                if (countyImprovementData.countyResourceType == AllEnums.CountyResourceType.None
                    || countyImprovementData.CheckIfStorageImprovement() == true
                    || CheckResourceStorageFull(countyResources[countyImprovementData.countyResourceType]) == true)
                {
                    return;
                }
                //GD.Print("")
                foreach (CountyPopulation countyPopulation in possibleWorkers)
                {
                    if (countyImprovementData.populationAtImprovement.Count
                        < countyImprovementData.adjustedMaxWorkers)
                    {
                        countyPopulation.UpdateActivity(AllEnums.Activities.Work);
                        UpdateWorkLocation(countyPopulation, countyImprovementData);
                        workersToRemoveFromPossibleWorkers.Add(countyPopulation);
                    }
                }
                RemoveWorkersFromPossibleWorkers();
            }
        }
        public void SubtractCountyResources()
        {
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in countyResources)
            {
                amountUsedCountyResources[keyValuePair.Key].Amount = countyResources[keyValuePair.Key].Amount -
                    yesterdaysCountyResources[keyValuePair.Key].Amount;
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
            PossiblyUseResources(herosInCountyList);
            PossiblyUseResources(armiesInCountyList);
            PossiblyUseResources(countyPopulationList);
        }

        private void PossiblyUseResources(Globals.ListWithNotify<CountyPopulation> peopleUsingResourcesList)
        {
            foreach (CountyPopulation countyPopulation in peopleUsingResourcesList)
            {
                // Go through all of their needs and skill check against it and if they pass, they use the resource
                // that is needed.
                foreach (KeyValuePair<AllEnums.CountyResourceType, int> keyValuePair in countyPopulation.needs)
                {
                    // Check to see if they want the resource.
                    if (SkillData.Check(countyPopulation, keyValuePair.Value, AllEnums.Attributes.MentalStrength, true) == true)
                    {
                        //GD.Print($"Needs Checks: Passed.");
                        if (CheckEnoughOfResource(keyValuePair.Key) == true)
                        {
                            //GD.Print("There are enough resources for the needs of a person.");
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
                        //GD.Print($"Needs Checks: Failed: " + countyPopulation.needs[keyValuePair.Key]);
                    }
                }
            }
        }
        public void RemoveResourceFromCounty(AllEnums.CountyResourceType countyResourceType, int amount)
        {
            countyResources[countyResourceType].Amount -= amount;

            // Update the top bar if the player has a county selected.
            if (Globals.Instance.SelectedLeftClickCounty == countyNode)
            {
                TopBarControl.Instance.UpdateResourceLabels();
            }
        }

        // This counts and compares to a global variable if there is enough of that resource.
        // We currently just have one number for a minimum of a resource, but we probably
        // should figure out a way for each different type.
        // We could actually put it in the resourceData, so each resource would know the minimum amount
        // the county needs.
        public bool CheckEnoughCountyFactionResource(AllEnums.FactionResourceType resourceType)
        {
            int amountOfResource = CountFactionResourceOfType(resourceType);
            return amountOfResource >= Globals.Instance.minimumFood;
        }

        private bool CheckEnoughOfResource(AllEnums.CountyResourceType resourceType)
        {
            bool enoughResource;
            if (countyResources[resourceType].Amount >= Globals.Instance.occationalResourceUsageAmount)
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
            List<CountyResourceData> perishableFoodList = [];
            List<CountyResourceData> nonperishableFoodList = [];
            foreach (CountyResourceData resourceData in countyResources.Values)
            {
                // Is food, and there is some food.
                if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == true && resourceData.Amount > 0)
                {
                    //GD.Print($"Adding to list: {resourceData.name}");
                    perishableFoodList.Add(resourceData);
                }
                else if (resourceData.factionResourceType == AllEnums.FactionResourceType.Food
                    && resourceData.perishable == false && resourceData.Amount > 0)
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
            public List<CountyResourceData> perishableFoodList = [];
            public List<CountyResourceData> nonperishableFoodList = [];
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
                peopleWhoNeedToDie.Clear();
                foreach (CountyPopulation countyPopulation in countyPopulationList)
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
                                // The County Population eats nonperishable food.
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
                            //GD.PrintRich($"[color=red]People Eat Food - Perishable: Starvation![/color]");
                            Starvation(countyPopulation, amount);
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
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} ate {amount}" +
                                $" now that county has {foodLists.nonperishableFoodList[randomNumber].name}" +
                                $" {foodLists.nonperishableFoodList[randomNumber].amount}");
                            */
                        }
                        else
                        {
                            // There is no food so this person starves.
                            Starvation(countyPopulation, amount);
                            //GD.PrintRich($"[color=red]People Eat Food - Nonperishable: Starvation![/color]");
                        }
                    }
                    else
                    {
                        // Starving!
                        Starvation(countyPopulation, amount);
                    }
                    AdjustPopulationHappiness(amount, countyPopulation);
                }
                KillPeopleWhoNeedToDie(peopleWhoNeedToDie);
            }
        }

        private void Starvation(CountyPopulation countyPopulation, int amount)
        {
            //GD.PrintRich($"[rainbow]There is no food at all!");
            //GD.Print($"{countyPopulation.firstName} has starved for {countyPopulation.daysStarving} days.");
            // This will give each population an additonal -1 to their happiness which works for now.
            AdjustPopulationHappiness(amount, countyPopulation);
            if (countyPopulation.daysStarving >= Globals.Instance.daysUntilDamageFromStarvation)
            {
                countyPopulation.hitpoints--;
                // This should be its own method in countyPopulation that kills the population.
                if (countyPopulation.hitpoints < 1)
                {
                    peopleWhoNeedToDie.Add(countyPopulation);
                }
            }
            countyPopulation.daysStarving++;
        }

        private void KillPeopleWhoNeedToDie(List<CountyPopulation> peopleWhoNeedToDie)
        {
            foreach (CountyPopulation countyPopulation in peopleWhoNeedToDie)
            {
                countyPopulationList.Remove(countyPopulation);
                herosInCountyList.Remove(countyPopulation);
                armiesInCountyList.Remove(countyPopulation);
                deadPeopleList.Add(countyPopulation);
                //GD.PrintRich($"[color=red]{countyPopulation.firstName} {countyPopulation.lastName} has croaked.[/color]");
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
            yesterdaysCountyResources = [];
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in countyResources)
            {
                yesterdaysCountyResources.Add(keyValuePair.Key, new CountyResourceData
                {
                    GoodName = keyValuePair.Value.GoodName,
                    description = keyValuePair.Value.description,
                    countyResourceType = keyValuePair.Value.countyResourceType,
                    factionResourceType = keyValuePair.Value.factionResourceType,
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

        public static void CheckForHealing(Globals.ListWithNotify<CountyPopulation> possibleHurtPopulationList)
        {
            foreach (CountyPopulation countyPopulation in possibleHurtPopulationList)
            {
                if (countyPopulation.hitpoints < countyPopulation.maxHitpoints && countyPopulation.daysStarving < 1)
                {
                    countyPopulation.hitpoints++;
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
        }

        private void CheckForPrioritizedImprovements(List<CountyImprovementData> countyImprovements)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovements)
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

        /// <summary>
        /// Assign possible workers to building a county improvement, and remove them from the
        /// possible workers list.
        /// </summary>
        /// <param name="countyImprovementData"></param>
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
            // GD.Print($"{countyImprovementData.improvementName} Population At Improvement: "
                //+ countyImprovementData.populationAtImprovement.Count);

            // Assign sorted workers if there is room
            foreach (CountyPopulation countyPopulation in possibleWorkers.Take(remainingWorkerSlots))
            {
                countyPopulation.RemoveFromCountyImprovement();
                countyPopulation.UpdateActivity(activity);
                UpdateWorkLocation(countyPopulation, countyImprovementData);
                workersToRemoveFromPossibleWorkers.Add(countyPopulation);
            }
            RemoveWorkersFromPossibleWorkers();
        }

        /// <summary>
        /// Get all the people who are helpful and loyal for prioritized construction and work.
        /// </summary>
        private void GetPrioritizedWorkersList()
        {
            // Go through each person in the county.
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                // Go through everyone and if they are helpful and loyal add them to the possibleWorkers list.
                if (countyPopulation.CheckWillWorkLoyalty() == true
                    && countyPopulation.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
                {
                    // GD.Print($"{countyName}: {countyPopulation.firstName} is loyal and is helpful.");
                    possibleWorkers.Add(countyPopulation);
                }
            }
        }
    }
}