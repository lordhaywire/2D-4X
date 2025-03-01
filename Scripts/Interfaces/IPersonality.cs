using Godot;

namespace PlayerSpace;
public interface IPersonality
{
    public void EquipmentAssignment(PopulationData populationData);
}
public partial class DefensivePersonality : IPersonality
{
    /// Assign oldest equipment.
    void IPersonality.EquipmentAssignment(PopulationData populationData)
    {
        GD.Print($"{populationData.firstName} Defensive Personality!");
        populationData.useNewestEquipment = false;
    }
}

public partial class OffensivePersonality : IPersonality
{
    /// Assign newest equipment.
    void IPersonality.EquipmentAssignment(PopulationData populationData)
    {
        GD.Print($"{populationData.firstName} Offensive Personality!");
        populationData.useNewestEquipment = true;
    }
}
public partial class PlayerPersonality : IPersonality
{
    /// Assign nothing because the player does that.
    void IPersonality.EquipmentAssignment(PopulationData populationData)
    {
        GD.Print($"{populationData.firstName} Player Personality!");
    }
}



