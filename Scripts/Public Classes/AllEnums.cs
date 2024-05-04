namespace PlayerSpace
{
    public class AllEnums
    {
        /*
        public enum Factions
        {
            PlayerFaction,
            AIFaction1,
            AIFaction2,
            AIFaction3,
            AIFaction4,
            AIFaction5,
            AIFaction6,
        }
        */

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

        public enum CountyResourceType
        {
            Fish,
            Scrap,
            Vegetables,
            Wood,
        }

        // Scrap and wood should be combined into building materials.
        public enum FactionResourceType
        {
            BuildingMaterial,
            Food,
            Influence,
            Money,
            Scrap,
        }

        public enum SkillType
        {
            PhysicalStrength,
            Agility,
            Endurance,
            Intelligence,
            MentalStrength,
            Awareness,
            Charisma,
            Looks,
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
    }
}