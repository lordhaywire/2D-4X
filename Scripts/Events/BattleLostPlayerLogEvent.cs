namespace PlayerSpace;

public class BattleLostPlayerLogEvent(string factionName, Battle battle)
{
    public string factionName = factionName;
    public Battle battle = battle;
}