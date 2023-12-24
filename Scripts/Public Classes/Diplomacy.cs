using Godot;

namespace PlayerSpace
{
    public class Diplomacy
    {
        public void DeclareWar(War war)
        {
            GD.Print($"{war.attackerFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            EventLog.Instance.AddLog($"{war.attackerFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            Globals.Instance.selectedRightClickCounty.countyData
                .factionData.diplomacy.RespondToDeclarationOfWar(war, Globals.Instance.selectedRightClickCounty);
        }

        public void RespondToDeclarationOfWar(War war, SelectCounty initialCounty)
        {
            GD.Print($"{war.defenderFactionData.factionName} is responding to the declaration of war.");
            EventLog.Instance.AddLog($"{war.defenderFactionData.factionName} is raising armies!");

            DefenderSpawnArmies(war.defenderFactionData, initialCounty);
            // Add the wars to the factions so they know what wars they are in.
            war.attackerFactionData.wars.Add(war);
            war.defenderFactionData.wars.Add(war);
        }

        public void DefenderSpawnArmies(FactionData defenderFactionData, SelectCounty spawnLocation)
        {
            defenderFactionData.factionLeader.isArmyLeader = true;
            defenderFactionData.factionLeader.token = defenderFactionData.tokenSpawner.Spawn(spawnLocation, defenderFactionData.factionLeader);
        }
    }
}