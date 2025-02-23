using Godot;

namespace PlayerSpace;
public interface IHeroPersonalities
{
    public void EquipmentAssignment();
}


public partial class OffensivePersonality : Resource, IHeroPersonalities
{
    void IHeroPersonalities.EquipmentAssignment()
    {
        GD.Print("Offensive Personality!");
    }
}

public partial class DefensivePersonality : Resource, IHeroPersonalities
{
    void IHeroPersonalities.EquipmentAssignment()
    {
        GD.Print("Defensive Personality!");
    }
}
