using Godot;

namespace PlayerSpace
{
    public partial class AllSkills : Node
    {
        public static AllSkills Instance { get; private set; }

        [Export] public SkillData[] allSkills;

        public override void _Ready()
        {
            Instance = this;
        }
    }
}