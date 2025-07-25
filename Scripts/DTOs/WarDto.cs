namespace PlayerSpace;
public class WarDto
{
    public int AggressorFactionId { get; set; }
    public int DefenderFactionId { get; set; }
    // If you ever add battles later, you can include a List<BattleDto> here
}