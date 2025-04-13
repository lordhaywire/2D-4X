using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;

[GlobalClass]
public partial class CountyData : Resource
{
    [ExportGroup("MapEditor")] public County countyNode; // See if we can get rid of this somehow.
    [Export] public Color color;
    public Vector2I startMaskPosition; // I think this is the local position....
    [Export] public Vector2I countyOverlayLocalPosition;

    [ExportGroup("County other somethings")] [Export]
    public int countyId; // This is used all over the place.

    [Export] public string countyName;

    [Export]
    public bool
        isPlayerCapital; // We need to differentiate between player chosen capitals and AI capitals for generation after player creation.

    [Export] public bool isAiCapital;

    //[Export] public AllEnums.Factions faction;
    [Export] public FactionData factionData;

    [Export] public AllEnums.Province province;
    [Export] public AllEnums.Terrain biomePrimary;
    [Export] public AllEnums.Terrain biomeSecondary;
    [Export] public AllEnums.Terrain biomeTertiary;

    [ExportGroup("Population Lists")] [Export]
    public Godot.Collections.Array<PopulationData> populationDataList = [];

    [Export] public Godot.Collections.Array<PopulationData> heroesInCountyList = [];
    [Export] public Godot.Collections.Array<PopulationData> armiesInCountyList = [];
    [Export] public Godot.Collections.Array<PopulationData> visitingHeroList = [];
    [Export] public Godot.Collections.Array<PopulationData> visitingArmyList = [];
    [Export] public Godot.Collections.Array<PopulationData> deadPeopleList = [];

    [ExportGroup("Construction and Work Lists")] [Export]
    public Godot.Collections.Array<PopulationData> heroBuildersList = [];

    [Export] public Godot.Collections.Array<PopulationData> heroWorkersList = [];

    [Export]
    public Godot.Collections.Array<PopulationData>
        workersList = []; // List of all the idle, helpful and loyal workers for that day.

    [Export] public Godot.Collections.Array<PopulationData> prioritizedHeroBuildersList = [];
    [Export] public Godot.Collections.Array<PopulationData> prioritizedHeroWorkersList = [];
    [Export] public Godot.Collections.Array<PopulationData> prioritizedBuildersList = [];
    [Export] public Godot.Collections.Array<PopulationData> prioritizedWorkersList = [];
    [Export] public Godot.Collections.Array<PopulationData> scavengingHeroesList = []; // We don't need this list.

    [Export]
    public Godot.Collections.Array<PopulationData>
        workersToRemoveFromLists = []; // List to collect county populations to be removed from the possibleWorkers.

    [Export] public Godot.Collections.Array<CountyImprovementData> prioritizedConstructionImprovementList = [];
    [Export] public Godot.Collections.Array<CountyImprovementData> prioritizedWorkImprovementList = [];

    public readonly List<Button> spawnedTokenButtons = [];

    [Export] public Godot.Collections.Array<CountyImprovementData> underConstructionCountyImprovementList = [];
    [Export] public Godot.Collections.Array<CountyImprovementData> completedCountyImprovementList = [];
    public readonly List<Battle> battles = [];

    public int population = 0;
    [Export] public int perishableStorage;
    [Export] public int nonperishableStorage;

    [Export] public int scavengeableRemnants; // This the total a county has available to scavenge.
    [Export] public int scavengeableCannedFood; // This the total a county has available to scavenge.

    [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> goods = [];
    [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> yesterdaysGoods = [];
    [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> amountOfGoodsUsed = [];

    // These are used just to pass some data around.  Probably I should find a better way to do this.
    public Texture2D maskTexture;
    public Texture2D mapTexture;

    // We will have to see if this is still used.
    public event Action<bool> CountySelected;

    private bool selected;

    private bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            OnCountySelected(selected);
        }
    }

    private void OnCountySelected(bool isSelected)
    {
        CountySelected?.Invoke(isSelected);
    }

    int idleWorkers;

    public int IdleWorkers
    {
        get => idleWorkers;
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
        foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovementList)
        {
            // If the county improvement is done, make everyone working on it idle.
            // Set their current work to null.
            if (countyImprovementData.CheckIfCountyImprovementDone())
            {
                foreach (PopulationData populationData in countyImprovementData.populationAtImprovement)
                {
                    // The population would be on this list because they would be doing construction.
                    if (populationData.isHero != true)
                    {
                        populationData.UpdateActivity(AllEnums.Activities.Idle);
                        populationData.UpdateCurrentCountyImprovement(null);
                    }
                    else
                    {
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
                    EventLog.Instance.AddLog(
                        $"{Tr(countyImprovementData.GetCountyImprovementName())} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");
                }

                //GD.Print($"Under Construction Improvements - Checking if done : " +
                //    $"{countyImprovementData.improvementName} : {countyImprovementData.status}");
            }
        }

        // Move the county improvement to the correct list and remove it from the old list.
        MoveCountyImprovementToCompletedList(completedImprovements);
        // Refresh the panel so that heroes checkboxes are enabled or disabled.
        CountyInfoControl.Instance.UpdateEverything();
    }

    /// <summary>
    /// This removes goods from county scavengeable goods.
    /// </summary>
    /// <param name="goodType"></param>
    /// <param name="amount"></param>
    public void RemoveGoodsFromAvailableCountyTotals(AllEnums.CountyGoodType goodType, int amount)
    {
        if (goodType == AllEnums.CountyGoodType.CannedFood)
        {
            scavengeableCannedFood -= amount;
        }
        else if (goodType == AllEnums.CountyGoodType.Remnants)
        {
            scavengeableRemnants -= amount;
        }
        else
        {
            // // GD.Print("Your resource type is wrong!");
        }
    }

    public void CountIdleWorkers()
    {
        int currentIdleWorkers = 0;
        foreach (PopulationData populationData in populationDataList)
        {
            if (populationData.activity == AllEnums.Activities.Idle)
            {
                currentIdleWorkers++;
            }
        }

        IdleWorkers = currentIdleWorkers;
    }

    /// <summary>
    /// If there isn't enough food then have the idle people start scavenging & if there is enough
    /// scavengeables in this county. It is set to zero because that is easy.  Don't worry about it.
    /// It also checks to see if there already is enough stored in the county.
    /// </summary>
    public void CheckForScavengingFood()
    {
        // Population won't scavenge if the storage is full, or if the county is out of scavengeables.
        if (!CheckEnoughCountyScavengeables(AllEnums.CountyGoodType.CannedFood)
            || CheckGoodStorageFull(goods[AllEnums.CountyGoodType.CannedFood]))
        {
            return;
        }

        //int amountOfFood = CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
        //// GD.Print($"{county.countyData.countyName} Amount of food: " + amountOfFood);

        foreach (PopulationData populationData in workersList)
        {
            populationData.UpdateActivity(AllEnums.Activities.Scavenge);
            populationData.UpdateCurrentCountyImprovement(null);
            workersToRemoveFromLists.Add(populationData);
        }

        RemovePopulationFromLists(workersList);
    }

    /// <summary>
    /// If there isn't enough remnants then have the idle people start scavenging & if there is enough
    /// scavengeables in this county. It is set to zero because that is easy.  Don't worry about it.
    /// It also checks to see if there already is enough stored in the county.
    /// </summary>
    public void CheckForScavengingRemnants()
    {
        // Population won't scavenge if the storage is full, or if the county is out of scavengeables.
        if (!CheckEnoughCountyScavengeables(AllEnums.CountyGoodType.Remnants)
            || CheckGoodStorageFull(goods[AllEnums.CountyGoodType.Remnants]))
        {
            return;
        }

        //// GD.Print($"{county.countyData.countyName} Amount of remnants: " + county.countyData.resources[AllEnums.CountyResourceType.Remnants].amount);

        foreach (PopulationData populationData in workersList)
        {
            populationData.UpdateActivity(AllEnums.Activities.Scavenge);
            populationData.UpdateCurrentCountyImprovement(null);
            workersToRemoveFromLists.Add(populationData);
        }

        RemovePopulationFromLists(workersList);
    }

    public bool CheckEnoughCountyScavengeables(AllEnums.CountyGoodType resourceType)
    {
        if (resourceType == AllEnums.CountyGoodType.CannedFood)
        {
            if (scavengeableCannedFood > 0)
            {
                return true;
            }

            return false;
        }
        else if (resourceType == AllEnums.CountyGoodType.Remnants)
        {
            if (scavengeableRemnants > 0)
            {
                return true;
            }

            return false;
        }
        else
        {
            // GD.Print($"[color=red]Something in Check Enough County Scavengeables has gone horribly wrong.[/color]");
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

    public void AssignEveryoneToConstruction()
    {
        foreach (CountyImprovementData countyImprovementData in underConstructionCountyImprovementList)
        {
            if (prioritizedHeroBuildersList.Count > 0)
            {
                countyImprovementData.RemoveEveryoneFromCountyImprovement(this);
                HeroWorkStart.AssignHeroesToBuildImprovement(this, countyImprovementData);
            }

            PopulationWorkStart.AssignPopulationToBuildImprovement(this, countyImprovementData, workersList);
        }
    }

    public void RemovePopulationFromLists(Godot.Collections.Array<PopulationData> listToRemovePeopleFrom)
    {
        // Remove the collected items from the possibleWorkers list
        foreach (PopulationData populationData in workersToRemoveFromLists)
        {
            listToRemovePeopleFrom.Remove(populationData);
        }

        workersToRemoveFromLists.Clear();
    }

    public void FindIdlePopulation()
    {
        // Go through each person in the county.
        foreach (PopulationData populationData in populationDataList)
        {
            // Go through everyone and if they are idle, helpful and loyal add them to the workers list.
            if (populationData.activity == AllEnums.Activities.Idle
                && populationData.CheckWillWorkLoyalty()
                && populationData.CheckForPerk(AllEnums.Perks.Unhelpful) == false)
            {
                //GD.Print($"{countyName}: {populationData.firstName} is idle, is loyal and is not unhelpful.");
                workersList.Add(populationData);
            }
        }
    }

    public void ClearIdlePopulationList()
    {
        workersList.Clear(); // Clear the list at the start of each day.
    }

    private bool CheckIfImprovementTypeNeedsWorkers(CountyImprovementData countyImprovementData)
    {
        if (countyImprovementData.CheckIfResearchImprovement())
        {
            return true;
        }

        if (countyImprovementData.CheckIfStorageImprovement()
            || CheckGoodStorageFull(goods[countyImprovementData.countyResourceType]))
        {
            return false;
        }

        return true;
    }

    public void CheckForPreferredWork()
    {
        //GD.Print($"{county.countyData.countyName}: Checking for Preferred Work!");
        //GD.Print($"Completed County Improvements: {completedCountyImprovements.Count}");
        foreach (CountyImprovementData countyImprovementData in completedCountyImprovementList)
        {
            if (CheckIfImprovementTypeNeedsWorkers(countyImprovementData) == false
                || countyImprovementData.CheckIfStatusLowStockpiledGoods())
            {
                continue;
            }

            LookForPopulationPreferredWork(countyImprovementData, prioritizedHeroWorkersList);
            LookForPopulationPreferredWork(countyImprovementData, workersList);
        }
    }

    private void LookForPopulationPreferredWork(CountyImprovementData countyImprovementData
        , Godot.Collections.Array<PopulationData> populationList)
    {
        foreach (PopulationData populationData in populationList)
        {
            // If they have the preferred skill, they are added to the county improvement
            // and marked for removal from the possibleWorkers list.
            // It needs to check if workers full here, so that it doesn't add extra people to the 
            // county improvement.
            if (countyImprovementData.CheckIfWorkersFull() == false)
            {
                if (populationData.preferredSkill.skill == countyImprovementData.workSkill)
                {
                    AddPersonToWorkImprovement(countyImprovementData, populationData);
                }
            }
        }

        RemovePopulationFromLists(populationList);
    }

    private void AddPersonToWorkImprovement(CountyImprovementData countyImprovementData, PopulationData populationData)
    {
        populationData.RemoveFromCountyImprovement();
        populationData.UpdateActivity(AllEnums.Activities.Work);
        populationData.UpdateCurrentCountyImprovement(countyImprovementData);
        countyImprovementData.AddPopulationToPopulationAtImprovementList(populationData);
        workersToRemoveFromLists.Add(populationData);
    }

    public void CheckForAnyWork()
    {
        GD.Print("Possible Workers List Count: " + workersList.Count);
        GD.Print("Prioritized Hero Workers List Count: " + prioritizedHeroWorkersList.Count);

        foreach (CountyImprovementData countyImprovementData in completedCountyImprovementList)
        {
            if (CheckIfImprovementTypeNeedsWorkers(countyImprovementData) == false
                || countyImprovementData.CheckIfStatusLowStockpiledGoods())
            {
                GD.Print("Low Stockpiled Goods, or the improvement is storage or research.");
                continue;
            }

            if (prioritizedHeroWorkersList.Count > 0)
            {
                countyImprovementData.RemoveEveryoneFromCountyImprovement(this);
                LookForPopulationToWorkImprovement(countyImprovementData, prioritizedHeroWorkersList);
            }

            LookForPopulationToWorkImprovement(countyImprovementData, workersList);
        }
    }

    private void LookForPopulationToWorkImprovement(CountyImprovementData countyImprovementData
        , Godot.Collections.Array<PopulationData> anyWorkersList)
    {
        foreach (PopulationData populationData in anyWorkersList)
        {
            // It needs to check if county improvement's workers are full,
            // so that it doesn't add extra people to the 
            // county improvement.
            if (countyImprovementData.CheckIfWorkersFull() == false)
            {
                AddPersonToWorkImprovement(countyImprovementData, populationData);
            }
        }

        RemovePopulationFromLists(anyWorkersList);
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
        // Do the math for amount used. Subtract yesterdays from today's and that is how much we have used.
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in goods)
        {
            amountOfGoodsUsed[keyValuePair.Key].Amount = goods[keyValuePair.Key].Amount -
                                                         yesterdaysGoods[keyValuePair.Key].Amount;
        }
        /*
        if (factionData.isPlayer)
        {
            GD.Print("After subtraction yesterday's vegetables is: "
                + yesterdaysCountyResources[AllEnums.CountyResourceType.Vegetables].Amount);
        }
        */
    }

    public void OccasionalNeeds()
    {
        PossiblyUseResources(this, heroesInCountyList);
        PossiblyUseResources(this, armiesInCountyList);
        PossiblyUseResources(this, populationDataList);
    }

    private void PossiblyUseResources(CountyData countyData,
        Godot.Collections.Array<PopulationData> peopleUsingResourcesList)
    {
        foreach (PopulationData populationData in peopleUsingResourcesList)
        {
            // Go through all of their needs and skill check against it and if they pass
            // , they use the resource that is needed.
            foreach (KeyValuePair<AllEnums.CountyGoodType, int> keyValuePair in populationData.needs)
            {
                int attributeLevel = populationData.attributes[AllEnums.Attributes.MentalStrength].attributeLevel;
                // This is a negative bonus because we want the person to fail it, thus not needing stuff.
                int attributeBonus
                    = AttributeData.GetAttributeBonus(attributeLevel, false, true);
                // Check to see if they want the resource.
                if (SkillData.CheckWithBonuses(keyValuePair.Value
                        , attributeBonus, 0, 0)) // TODO: Perk bonus
                {
                    //GD.Print($"Needs Checks: Passed.");
                    if (CheckEnoughOfResource(keyValuePair.Key))
                    {
                        //GD.Print("There are enough resources for the needs of a person.");
                        // Use good, which is why this is negative.
                        Haulmaster.AdjustCountyGoodAmount(countyData, keyValuePair.Key
                            , -Globals.Instance.occasionalResourceUsageAmount);

                        // Add happiness.
                        populationData.AddRandomHappiness(1);

                        // Set need back to zero.
                        populationData.needs[keyValuePair.Key] = 0;
                    }
                    else
                    {
                        // Reduce this populations' happiness.
                        populationData.RemoveRandomHappiness(1);
                    }
                }
                else
                {
                    // Gain 1d4 amount to the need for the next day.
                    Random random = new();
                    int needIncrease = random.Next(1, Globals.Instance.occasionalNeedIncreaseAmount);
                    populationData.needs[keyValuePair.Key] += needIncrease;
                    //GD.Print($"Needs Checks: Failed: " + populationData.needs[keyValuePair.Key]);
                }
            }
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
        bool enoughResource = goods[resourceType].Amount >= Globals.Instance.occasionalResourceUsageAmount;

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
        FoodLists foodLists = new()
        {
            perishableFoodList = perishableFoodList,
            nonperishableFoodList = nonperishableFoodList
        };
        return foodLists;
    }

    public class FoodLists
    {
        public List<GoodData> perishableFoodList = [];
        public List<GoodData> nonperishableFoodList = [];
    }

    public void PopulationEatsFood(Godot.Collections.Array<PopulationData> hungryPopulationDataList, int amount)
    {
        FoodLists foodLists = GetListsOfFood();
        //GD.Print("Population List count: " + populationDataList.Count());
        if (hungryPopulationDataList.Count < 1)
        {
            //GD.PrintRich($"[pulse freq=5.0 color=green]Population Eats Food: " +
            //    $"A population list such as hero list or army list is empty.[/pulse]");
            return;
        }

        peopleWhoNeedToDie.Clear();
        foreach (PopulationData populationData in hungryPopulationDataList)
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
                // Check to see if there is enough nonperishable food.
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

                // If the amount of food left is greater than zero they eat something.
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

    private void Starvation(PopulationData populationData, int amount)
    {
        //GD.PrintRich($"[rainbow]There is no food at all!");
        //GD.Print($"{populationData.firstName} has starved for {populationData.daysStarving} days.");
        // This will give each population an additional -1 to their happiness which works for now.
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

    private void KillPeopleWhoNeedToDie(List<PopulationData> peopleWhoNeedToDieSoon)
    {
        foreach (PopulationData populationData in peopleWhoNeedToDieSoon)
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
        workersList.Add(populationData);
    }

    public void AddPopulationDataToPrioritizedWorkersList(PopulationData populationData)
    {
        prioritizedWorkersList.Add(populationData);
    }

    private void MoveCountyImprovementToCompletedList(List<CountyImprovementData> countyImprovementDataToRemove)
    {
        foreach (CountyImprovementData countyImprovementData in countyImprovementDataToRemove)
        {
            completedCountyImprovementList.Add(countyImprovementData);
            underConstructionCountyImprovementList.Remove(countyImprovementData);
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

    public static void CheckForHealing(IEnumerable<PopulationData> possibleHurtPopulationList)
    {
        foreach (PopulationData populationData in possibleHurtPopulationList)
        {
            if (populationData.hitpoints < populationData.maxHitpoints && populationData.daysStarving < 1)
            {
                populationData.hitpoints++;
            }
        }
    }

    public void AddImprovementToPrioritizedConstructionImprovementList(CountyImprovementData countyImprovementData)
    {
        prioritizedConstructionImprovementList.Add(countyImprovementData);
    }

    public void AddImprovementToPrioritizedWorkImprovementList(CountyImprovementData countyImprovementData)
    {
        prioritizedWorkImprovementList.Add(countyImprovementData);
    }

    public void AddPopulationDataToPrioritizedBuildersList(PopulationData populationData)
    {
        prioritizedBuildersList.Add(populationData);
    }
    /*
    public void AddPopulationBuildersList(PopulationData populationData)
    {
        buildersList.Add(populationData);
    }
    */

    private void AddPopulationWorkersList(PopulationData populationData)
    {
        workersList.Add(populationData);
    }

    public void AddHeroToPrioritizedHeroBuildersList(PopulationData populationData)
    {
        prioritizedHeroBuildersList.Add(populationData);
    }

    public void AddHeroToPrioritizedHeroWorkersList(PopulationData populationData)
    {
        prioritizedHeroWorkersList.Add(populationData);
    }

    private void AddHeroToHeroWorkersList(PopulationData populationData)
    {
        heroWorkersList.Add(populationData);
    }

    private void AddHeroToHeroBuildersList(PopulationData populationData)
    {
        heroBuildersList.Add(populationData);
    }
}