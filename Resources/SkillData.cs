using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class SkillData : Resource
    {
        [Export] public AllEnums.Skills skill;
        [Export] public string skillName;
        [Export] public string skillDescription;
        [Export] public int skillLevel;
        [Export] public int amountLearned;
        [Export] public bool isCombatSkill;
        [Export] public AllEnums.Attributes skillType;

        public bool Check(int skillAmount)
        {
            int skillCheckRoll = Globals.Instance.random.Next(1, 101);
            if (skillCheckRoll <= skillAmount)
            {
                //GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is less then or equal {skillAmount}");
                return (true);
            }
            else
            {
                //GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is greater then {skillAmount}");
                return (false);
            }
        }

        public void CheckLearning(CountyPopulation countyPopulation, SkillData skillData)
        {
            // Every time a skill is used the amount learned goes up.
            if (countyPopulation.factionData.isPlayer)
            {
                GD.Print($"{countyPopulation.firstName} has amount learned: {skillData.amountLearned}");
            }

            skillData.amountLearned++;
            int learningNeeded;

            if (skillData.isCombatSkill)
            {
                learningNeeded = Globals.Instance.combatSkillLearningNeeded;
            }
            else
            {
                learningNeeded = Globals.Instance.maxLearningNeeded;
            }
            if (skillData.amountLearned == learningNeeded)
            {
                GD.Print("Skill Amount to Fail:" + skillData.skillLevel);

                int failRoll = Globals.Instance.random.Next(0, 101);
                //GD.Print("Fail Roll: " + failRoll);

                if (failRoll > skillData.skillLevel)
                {
                    int experienceLearned = Globals.Instance.random.Next(1, Globals.Instance.maxXPRoll);
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned {experienceLearned} in " +
                            $"{TranslationServer.Translate(skillData.skillName)}");
                    }
                    skillData.skillLevel += experienceLearned;
                }
                else
                {
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned nothing in" +
                            $" {TranslationServer.Translate(skillData.skillName)}");
                    }
                }
                skillData.amountLearned = 0;
            }
            else
            {
                GD.Print($"{countyPopulation.firstName} skill is not ready to level up.");
            }
        }
    }
}