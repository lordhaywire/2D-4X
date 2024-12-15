using System;

namespace PlayerSpace;

public class AllEnums
{
    public enum Activities
    {
        Build,
        Combat,
        Idle,
        Move,
        Research,
        Scavenge,
        Work,
    }
    public enum Attributes
    {
        Agility,
        Awareness,
        Charisma,
        Endurance,
        Intelligence,
        Looks,
        MentalStrength,
        PhysicalStrength,
    }

    public enum CountyImprovementStatus
    {
        None,
        AwaitingPlayerAssignment, // I think this will go away.  This for research and we won't do research this way.
        Producing,
        Researching,
        LowStockpiledGoods,
        UnderConstruction,
        InResearchPanel, // I think this will go away.  This for research and we won't do research this way.
    }

    public enum CountyImprovementType
    {
        Standard,
        Research,
        Storage, // I am not sure this will end up being used.  It is getting used, but not in a good way.
    }

    public enum CountyGoodType
    {
        None,
        CannedFood,
        Fish,
        PrimativeScoutingEquipment,
        Remnants,
        StorageNonperishable,
        StoragePerishable,
        Vegetables,
        Wood,
    }

    // Scrap and wood should be combined into building materials.
    public enum FactionGoodType
    {
        None,
        BuildingMaterial,
        Equipment,
        Food,
        Influence,
        Money,
        Remnants,
        Research,
    }

    public enum Perishable
    {
        Neither,
        Perishable,
        Nonperishable,
    }
    public enum GoodType
    {
        CountyGood,
        FactionGood,
        Both,
    }

    public enum LearningSpeed
    {
        slow,
        medium,
        fast,
    }

    public enum InterestType
    {
        Biology,
        Botany,
        Engineering,
        Humanities,
        Information,
        Politics,
        Warfare,
        // Maybe medicine?  Is biology too broad?
    }
    public enum Perks
    {
        LeaderOfPeople,
        Unhelpful,
    }

    public enum Province
    {
        Oregon,
        Washington,
    }

    public enum ResearchTiers
    {
        One,
        Two,
        Three,
    }

    public enum Skills
    {
        Construction,
        Cool,
        Farm,
        Fish,
        Labor, // Possibly make two skills, one of them hauling.
        Lumberjack,
        Production,
        Research,
        Rifle,
        Scavenge,
    }

    public enum Terrain
    {
        Coast,
        Desert,
        Forest,
        Mountain,
        Plain,
        River,
        Ruin,
    }

    public static T GetRandomEnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        Random random = new();
        int randomIndex = random.Next(values.Length);
        return (T)values.GetValue(randomIndex);
    }
}