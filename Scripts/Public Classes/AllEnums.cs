using System;

namespace PlayerSpace;

public class AllEnums
{
    public enum Activities
    {
        Build,
        Combat,
        Explore,
        Idle,
        Move,
        Recruit,
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
    public enum EquipmentType
    {
        None,
        Reconnaissance,
        Offensive,
        Defensive,
        Medical,
        Auxiliary,
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


    public enum GoodType
    {
        CountyGood,
        FactionGood,
        Both,
    }

    // These are just protype personalities.
    // Player only should apply to the current factionLeader.
    public enum Personality
    {
        Player,
        Defensive,
        Offensive,
    }

    public enum HeroType
    {
        None,
        Aide,
        ArmyLeader,
        FactionLeader,
        FactionLeaderArmyLeader,
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

    public enum LearningSpeed
    {
        Slow,
        Medium,
        Fast,
    }

    public enum Perishable
    {
        Neither,
        Perishable,
        Nonperishable,
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
        Leadership,
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

    public static int GetCorrectEquipmentSlot(AllEnums.EquipmentType equipmentType)
    {
        int equipmentSlot = (int)equipmentType - 1;
        return equipmentSlot;
    }
    /// <summary>
    /// This will be used for loading from disk as well.
    /// </summary>
    /// <param name="personality"></param>
    /// <returns></returns>
    public static IPersonality AssignPersonalityInterfaces(Personality personality)
    {
        IPersonality iPersonality;
        switch (personality)
        {
            case Personality.Defensive:
                iPersonality = new DefensivePersonality();
                return iPersonality;
            case Personality.Offensive:
                iPersonality = new OffensivePersonality();
                return iPersonality;
            case Personality.Player:
                iPersonality = new PlayerPersonality();
                return iPersonality;
            default:
                return null;
        }
    }
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        Random random = new();
        int randomIndex = random.Next(values.Length);
        return (T)values.GetValue(randomIndex);
    }
}