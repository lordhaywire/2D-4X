namespace PlayerSpace;

public class SkillDto
{
    public string Skill { get; set; }       // store enum as string (e.g., "Rifle", "Construction")
    public int SkillLevel { get; set; }
    public int AmountUntilLearned { get; set; }
}