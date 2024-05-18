using Godot;

namespace PlayerSpace
{
	public partial class AllAttributes : Node
	{
		public static AllAttributes Instance { get; private set; }

        [Export] public AttributeData[] allAttributes;
		public override void _Ready()
		{
			Instance = this;
		}
	}
}