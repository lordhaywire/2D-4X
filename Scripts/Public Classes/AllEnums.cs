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
        Complete,
        UnderConstruction,
    }

    public enum CountyResourceType
    {
        None,
        CannedFood,
        Fish,
        PrimativeScoutingEquipment,
        Remnants,
        Vegetables,
        Wood,
    }

    // Scrap and wood should be combined into building materials.
    public enum FactionResourceType
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

    public enum LearningSpeed
    {
        slow,
        medium,
        fast,
    }

    public enum Interests
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