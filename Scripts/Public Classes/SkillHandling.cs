using Godot;
using System;

namespace PlayerSpace
{
    public class SkillHandling
    {
        public bool Check(int skillAmount)
        {
            int skillCheckRoll = Globals.Instance.random.Next(1, 101);
            if (skillCheckRoll <= skillAmount)
            {
                GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is less then or equal {skillAmount}");
                return(true);
            }
            else
            {
                GD.Print($"Skill Checks: rolled a {skillCheckRoll} which is greater then {skillAmount}");
                return(false);
            }
        }
    }
}