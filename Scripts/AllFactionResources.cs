using Godot;

namespace PlayerSpace;

	public partial class AllFactionResources : Node
	{
		public static AllFactionResources Instance { get; private set; }

		[Export] public FactionResourceData[] factionResourceDatas;
		public override void _Ready()
		{
			Instance = this;
		}

    /// <summary>
    /// Since there is an enum of None (which is zero when parsed to an int), we need to subtract
    /// 1 from the CountyResourceType when getting the resource with the AllEnums from the allResources
    /// array.
    /// </summary>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    public FactionResourceData GetFactionResourceData(AllEnums.FactionResourceType resourceType)
    {
        FactionResourceData factionResourceData = factionResourceDatas[(int)resourceType - 1];
        return factionResourceData;
    }
}