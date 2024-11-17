using Godot;
using System;
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
        [Export] public AllEnums.Attributes attribute;

        /// <summary>
        /// This does a skill check with an attribute bonus already added.
        /// Since an attribute bonus is checked in this method, we are passing in an attribute, and if the bonus is a negative.
        /// </summary>
        /// <param name="countyPopulation"></param>
        /// <param name="skillAmount"></param>
        /// <param name="attribute"></param>
        /// <param name="negativeBonus"></param>
        /// <returns></returns>
        public static bool Check(CountyPopulation countyPopulation, int skillAmount, AllEnums.Attributes attribute, bool negativeBonus)
        {
            int skillCheckRoll = Globals.Instance.random.Next(1, 101);
            //GD.PrintRich("[rainbow]Attribute: " + attribute);
            int attributeBonus = AttributeData.ApplyAttributeBonuses(countyPopulation.attributes[attribute].attributeLevel, false, negativeBonus);
            //GD.PrintRich($"[color=yellow]Attribute Bonus: {attributeBonus}[/color]");
            // Make sure the skill level with the attribute bonus is not below 1.
            int finalSkillAmount = Math.Max(skillAmount + attributeBonus, 1);
            // Rolling a 1 is always a success.
            if (skillCheckRoll <= finalSkillAmount)
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

        // ChatGPT refactored this.
        // The defending combat bool is temporary until we rewrite how combat works.
        public static void CheckLearning(CountyPopulation countyPopulation, bool defendingCombat)
        {
            AllEnums.LearningSpeed learningSpeed;
            SkillData skillData;
            switch (countyPopulation.activity)
            {
                case AllEnums.Activities.Scavenge:
                    skillData = countyPopulation.skills[AllEnums.Skills.Scavenge];
                    learningSpeed = AllEnums.LearningSpeed.fast;
                    break;
                case AllEnums.Activities.Build:
                    skillData = countyPopulation.skills[AllEnums.Skills.Construction];
                    learningSpeed = AllEnums.LearningSpeed.medium;
                    break;
                case AllEnums.Activities.Work:
                    skillData = countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill];
                    learningSpeed = AllEnums.LearningSpeed.medium;
                    break;
                case AllEnums.Activities.Research:
                    skillData = countyPopulation.skills[AllEnums.Skills.Research];
                    learningSpeed = AllEnums.LearningSpeed.slow;
                    break;
                case AllEnums.Activities.Combat:
                    if (defendingCombat)
                    {
                        skillData = countyPopulation.skills[AllEnums.Skills.Cool];
                    }
                    else
                    {
                        skillData = countyPopulation.skills[AllEnums.Skills.Rifle];
                    }
                    learningSpeed = AllEnums.LearningSpeed.slow;
                    break;
                default:
                    GD.Print($"{countyPopulation.firstName} activity is getting a skill check when it " +
                        $"shouldn't. SkillData.CheckLearning.");
                    return;
            }

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
                        AttributeData.ApplyAttributeBonuses(countyPopulation.attributes[AllEnums.Attributes.Intelligence].attributeLevel, true, false));

                    skillData.skillLevel += experienceLearned;

                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} - {TranslationServer.Translate(skillData.skillName)}" +
                            $" {TranslationServer.Translate("WORD_LEARNED")} {experienceLearned}");
                    }
                }
                /* We don't really need to put in the event log that someone didn't learn something.
                else
                {
                    if (countyPopulation.factionData.isPlayer)
                    {
                        EventLog.Instance.AddLog($"{countyPopulation.firstName} learned nothing in {TranslationServer.Translate(skillData.skillName)}");
                    }
                }
                */

                skillData.amountUntilLearned = 0;
            }
        }
    }
}