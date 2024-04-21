using Godot;
using System;

namespace PlayerSpace
{
    public partial class SkillData : Resource
    {
        public string skillName;
        public int skillLevel;
        public int amountLearned;
        public bool isCombatSkill;
        public AllEnums.SkillType skillType;
    }
}