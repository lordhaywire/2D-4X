using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class AllSkills : Node
{
    public static AllSkills Instance { get; private set; }

    private string skillDirectory = "res://Resources/Skills/";
    public List<SkillData> allSkillData;

    public override void _Ready()
    {
        Instance = this;
        
        allSkillData = Globals.Instance.ReadResourcesFromDisk(skillDirectory).Cast<SkillData>().ToList();
    }
}