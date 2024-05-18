using Godot;
using System;

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
    }
}