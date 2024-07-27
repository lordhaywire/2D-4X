using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class SkillData : Resource
    {
        [Export] public AllEnums.Skills skill;
        [Export] public string skillName;
        [Export] public string skillDescription;
        [Export] public int skillLevel;
        [Export] public int amountUntilLearned;
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

        // ChatGPT refactored this...
        public void CheckLearning(CountyPopulation countyPopulation, SkillData skillData, AllEnums.LearningSpeed learningSpeed)
        {
            // Increment the amount learned.
            skillData.amountUntilLearned++;

            // Dictionary to map learning speeds to the required amount.
            Dictionary<AllEnums.LearningSpeed, int> learningSpeedMap = new()
            {
                { AllEnums.LearningSpeed.slow, Globals.Instance.slowLearningNeeded },
                { AllEnums.LearningSpeed.medium, Globals.Instance.mediumLearningNeeded },
                { AllEnums.LearningSpeed.fast, Globals.Instance.fastLearningNeeded }
            };

            if (!learningSpeedMap.TryGetValue(learningSpeed, out int learningNeeded))
            {
                GD.Print("Something horrible has gone wrong in the Skill Data Check Learning.");
                return;
            }

            if (skillData.amountUntilLearned >= learningNeeded)
            {
                int failRoll = Globals.Instance.random.Next(0, 101);

                if (failRoll > skillData.skillLevel)
                {
                    int experienceLearnedRandom = Globals.Instance.random.Next(1, Globals.Instance.maxXPRoll);
                    int experienceLearned = Mathf.Max(1, experienceLearnedRandom +
                        AttributeData.ApplyAttributeBonuses(countyPopulation.attributes[AllEnums.Attributes.Intelligence].attributeLevel, true));

                    skillData.skillLevel += experienceLearned;

                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned {experienceLearned} in {TranslationServer.Translate(skillData.skillName)}");
                    }
                }
                else
                {
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned nothing in {TranslationServer.Translate(skillData.skillName)}");
                    }
                }

                skillData.amountUntilLearned = 0;
            }
        }

        /*
        public void CheckLearning(CountyPopulation countyPopulation, SkillData skillData
            , AllEnums.LearningSpeed learningSpeed)
        {
            // Every time a skill is used the amount learned goes up.
            skillData.amountUntilLearned++;
            int learningNeeded = 0;

            switch (learningSpeed)
            {
                case AllEnums.LearningSpeed.slow:
                    learningNeeded = Globals.Instance.slowLearningNeeded;
                    break;
                case AllEnums.LearningSpeed.medium:
                    learningNeeded = Globals.Instance.mediumLearningNeeded;
                    break;
                case AllEnums.LearningSpeed.fast:
                    learningNeeded = Globals.Instance.fastLearningNeeded;
                    break;
                default:
                    GD.Print("Something horrible has gone wrong in the Skill Data Check Learning.");
                    break;
            }

            if (skillData.amountUntilLearned >= learningNeeded)
            {
                //GD.Print("Skill Amount to Fail:" + skillData.skillLevel);

                int failRoll = Globals.Instance.random.Next(0, 101);
                //GD.Print("Fail Roll: " + failRoll);

                if (failRoll > skillData.skillLevel)
                {
                    int experienceLearnedRandom = Globals.Instance.random.Next(1, Globals.Instance.maxXPRoll);
                    // Make sure the experience learned has the attribute bonus (set to return ones, not tens)
                    // added and it isn't below one.
                    int experienceLearned = Mathf.Max(1, experienceLearnedRandom
                        + AttributeData.ApplyAttributeBonuses(countyPopulation.attributes[AllEnums.Attributes.Intelligence].attributeLevel, true));
                    skillData.skillLevel += experienceLearned;
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned {experienceLearned} in " +
                            $"{TranslationServer.Translate(skillData.skillName)}");
                    }
                }
                else
                {
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned nothing in" +
                            $" {TranslationServer.Translate(skillData.skillName)}");
                    }
                }
                skillData.amountUntilLearned = 0;
            }
            else
            {
                //GD.Print($"{countyPopulation.firstName} skill is not ready to level up.");
            }
        }
        */
    }
}