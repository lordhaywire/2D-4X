namespace PlayerSpace
{
    public class AllEnums
    {
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

        public enum Perks
        {
            LeaderofPeople,
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