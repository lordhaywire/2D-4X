using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace PlayerSpace;

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
    /// int attributeBonus 
    ///        = AttributeData.ApplyAttributeBonuses(populationData.attributes[attribute].attributeLevel
    ///        , false, negativeBonus);
    /// This does a skill check with an attribute bonus already added.
    /// The reason why we aren't passing in the actual skill is because sometimes we are testing just some random 1-100 number.
    /// Since an attribute bonus is checked in this method, we are passing in an attribute, and if the bonus is a negative.
    /// An additional bonus can be added.
    /// TODO: We also need to add any perk bonuses.
    /// </summary>
    /// <param name="skillLevel"></param>
    /// <param name="attributeBonus"></param>
    /// <param name="additionalBonus"></param>
    /// <param name="perkBonus"></param>
    /// <returns></returns>
    public static bool CheckWithBonuses(int skillLevel, int attributeBonus, int additionalBonus, int perkBonus)
    {
        int skillCheckRoll = Globals.Instance.random.Next(1, 101);
        //GD.PrintRich($"[rainbow]Skill Check Roll: " + skillCheckRoll);
        // Make sure the skill level with the attribute bonus is not below 1.
        int finalSkillAmount = Math.Max(skillLevel + attributeBonus + additionalBonus + perkBonus, 1);
        // Rolling a 1 is always a success.
        if (skillCheckRoll <= finalSkillAmount)
        {
            //GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is less then or equal {finalSkillAmount}");
            return (true);
        }
        else
        {
            //GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is greater then {finalSkillAmount}");
            return (false);
        }
    }


    // ChatGPT refactored this.
    // The defending combat bool is temporary until we rewrite how combat works.
    public static void LearningCheck(PopulationData populationData, bool defendingCombat)
    {
        AllEnums.LearningSpeed learningSpeed;
        SkillData skillData;
        switch (populationData.activity)
        {
            case AllEnums.Activities.Scavenge:
                skillData = populationData.skills[AllEnums.Skills.Scavenge];
                learningSpeed = AllEnums.LearningSpeed.Fast;
                break;
            case AllEnums.Activities.Build:
                skillData = populationData.skills[AllEnums.Skills.Construction];
                learningSpeed = AllEnums.LearningSpeed.Medium;
                break;
            case AllEnums.Activities.Work:
                skillData = populationData.skills[populationData.currentCountyImprovement.workSkill];
                learningSpeed = AllEnums.LearningSpeed.Medium;
                break;
            case AllEnums.Activities.Research:
                skillData = populationData.skills[AllEnums.Skills.Research];
                learningSpeed = AllEnums.LearningSpeed.Slow;
                break;
            case AllEnums.Activities.Combat:
                if (defendingCombat)
                {
                    skillData = populationData.skills[AllEnums.Skills.Cool];
                }
                else
                {
                    skillData = populationData.skills[AllEnums.Skills.Rifle];
                }
                learningSpeed = AllEnums.LearningSpeed.Slow;
                break;
            default:
                GD.Print($"{populationData.firstName} activity is getting a skill check when it " +
                    $"shouldn't. SkillData.CheckLearning.");
                return;
        }

        // Increment the amount learned.
        skillData.amountUntilLearned++;

        // Dictionary to map learning speeds to the required amount.
        Dictionary<AllEnums.LearningSpeed, int> learningSpeedMap = new()
        {
            { AllEnums.LearningSpeed.Slow, Globals.Instance.slowLearningNeeded },
            { AllEnums.LearningSpeed.Medium, Globals.Instance.mediumLearningNeeded },
            { AllEnums.LearningSpeed.Fast, Globals.Instance.fastLearningNeeded }
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
                    AttributeData.GetAttributeBonus(populationData.attributes[AllEnums.Attributes.Intelligence].attributeLevel, true, false));

                skillData.skillLevel += experienceLearned;

                if (populationData.factionData.isPlayer)
                {
                    EventLog.Instance.AddLog($"{populationData.firstName} - {TranslationServer.Translate(skillData.skillName)}" +
                        $" {TranslationServer.Translate("WORD_LEARNED")} {experienceLearned}");
                }
            }
            /* We don't really need to put in the event log that someone didn't learn something.
            else
            {
                if (populationData.factionData.isPlayer)
                {
                    EventLog.Instance.AddLog($"{populationData.firstName} learned nothing in {TranslationServer.Translate(skillData.skillName)}");
                }
            }
            */

            skillData.amountUntilLearned = 0;
        }
    }
}