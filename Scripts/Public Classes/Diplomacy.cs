using Godot;

namespace PlayerSpace
{
	public class Diplomacy
	{
		public void DeclareWar(FactionData attackerFactionData, FactionData defenderFactionData)
		{
			GD.Print($"{attackerFactionData.factionName} has declared war on {defenderFactionData.factionName}.");
			
			Globals.Instance.selectedRightClickCounty.countyData
				.factionData.diplomacy.RespondToDeclarationOfWar(defenderFactionData);
		}

		public void RespondToDeclarationOfWar(FactionData defenderFactionData)
		{
			GD.Print($"{defenderFactionData.factionName} is responding to the declaration of war.");
		}
	}
}