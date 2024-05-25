namespace PlayerSpace
{
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

        public enum Skills
        {
            Construction,
            Cool,
            Farm,
            Fish,
            Labor, // Possibly make two skills, one of them hauling.
            Lumberjack,
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
    }
}