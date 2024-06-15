using Godot;

namespace PlayerSpace
{
    public partial class AllResources : Node
    {
        public static AllResources Instance { get; private set; }

        [Export] public ResourceData[] allResources;

        public override void _Ready()
        {
            Instance = this;
            CountResources();
        }

        private void CountResources()
        {
            int perishable = 0;
            int nonperishable = 0;

            foreach (ResourceData resourceData in allResources)
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