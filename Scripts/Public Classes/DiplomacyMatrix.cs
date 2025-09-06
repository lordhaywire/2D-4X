namespace PlayerSpace;

public class DiplomacyMatrix(int factionId, string factionName, bool atWar)
{
    public int FactionId { get; set; } = factionId;
    public string FactionName { get; set; } = factionName;
    public bool AtWar { get; set; } = atWar;
}