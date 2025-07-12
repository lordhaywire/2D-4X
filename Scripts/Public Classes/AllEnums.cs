using System;
using System.Collections.Generic;
using Godot;

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
        Recruited,
        Research,
        Scavenge,
        Service,
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
        AwaitingPlayerAssignment, // I think this will go away.  This for research, and we won't do research this way.
        Producing,
        Researching,
        LowStockpiledGoods,
        UnderConstruction,
        InResearchPanel, // I think this will go away.  This for research, and we won't do research this way.
    }

    public enum CountyImprovementType
    {
        Standard,
        Research,
        Storage, // I am not sure if this will end up being used.  It is getting used, but not well.
    }

    public enum CountyGoodType
    {
        None,
        CannedFood,
        Cloth,
        Fish,
        Fruit,
        Glass,
        Iron,
        IronOre,
        Paper,
        PrimitiveScoutingEquipment,
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
        RawMaterial,
        Remnants,
        Research,
    }

    public enum FactionStatus
    {
        Alive,
        Dead,
    }

    public enum GoodType
    {
        CountyGood,
        FactionGood,
        Both,
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

    // These are just prototype personalities.
    // Player only should apply to the current factionLeader.
    public enum Personality
    {
        Player,
        Defensive,
        Offensive,
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
        Scout,
    }

    public enum StoryEventRewardType
    {
        CountyImprovement,
        Instant,
        People,
        Raider,
        Scavengeable,
    }
    public enum Terrain
    {
        Coast,
        Desert,
        Forest,
        Glacier,
        Hill,
        Lake,
        Mountain,
        Oasis,
        Plain,
        Plateau,
        River,
        Ruin,
        Swamp,
        Tundra,
    }

    public static string GetTerrainName(Terrain terrain)
    {
        List<string> terrainStrings =
        [
            $"{TranslationServer.Translate("TERRAIN_NAME_COAST")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_DESERT")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_FOREST")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_GLACIER")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_HILL")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_LAKE")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_MOUNTAIN")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_OASIS")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_PLAIN")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_PLATEAU")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_RIVER")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_RUIN")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_SWAMP")}",
            $"{TranslationServer.Translate("TERRAIN_NAME_TUNDRA")}",
        ];
        return terrainStrings[(int)terrain];
    }
    
    /// <summary>
    /// Since there is no equipment slot of none, but there is an equipment slot for all the other equipment types, we need to subtract one from the equipment type.
    /// </summary>
    /// <param name="equipmentType"></param>
    /// <returns></returns>
    public static int GetCorrectEquipmentSlot(EquipmentType equipmentType)
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