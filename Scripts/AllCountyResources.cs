using Godot;

namespace PlayerSpace
{
    public partial class AllCountyResources : Node
    {
        public static AllCountyResources Instance { get; private set; }

        [Export] public CountyResourceData[] allResources;

        public override void _Ready()
        {
            Instance = this;
            CountResources();
        }

        /// <summary>
        /// Since there is an enum of None (which is zero when parsed to an int), we need to subtract
        /// 1 from the CountyResourceType when getting the resource with the AllEnums from the allResources
        /// array.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public CountyResourceData GetCountyResourceData(AllEnums.CountyResourceType resourceType)
        {
            CountyResourceData countyResourceData = allResources[(int)resourceType - 1];
            return countyResourceData;
        }
        private void CountResources()
        {
            int perishable = 0;
            int nonperishable = 0;

            foreach (CountyResourceData resourceData in allResources)
            {
                if (resourceData.perishable)
                {
                    perishable++;
                }
                else
                {
                    nonperishable++;
                }
            }
            Globals.Instance.numberOfPerishableResources = perishable;
            Globals.Instance.numberOfNonperishableResources = nonperishable;
        }
    }
}