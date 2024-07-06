using Godot;

namespace PlayerSpace
{
	public partial class AllFactionResources : Node
	{
		public static AllFactionResources Instance { get; private set; }

		[Export] public FactionResourceData[] factionResourceDatas;
		public override void _Ready()
		{
			Instance = this;
		}
	}
}