namespace PlayerSpace;

public class ArmyLostPlayerLogEvent(string factionName, string countyName)
{
    public string factionName = factionName;
    public string countyName = countyName;
}
