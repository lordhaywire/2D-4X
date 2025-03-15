using Godot;

namespace PlayerSpace;

public class Recruiter
{
    public static void CheckForRecruitment(CountyData countyData)
    {
        foreach(PopulationData populationData in countyData.heroesInCountyList)
        {
            if(populationData.heroSubordinates.Count != populationData.numberOfSubordinates)
            {
                GD.Print("Number of subordinates doesn't match.");
                populationData.UpdateActivity(AllEnums.Activities.Recruit);
            }
            else
            {
                GD.Print("Number of subordinates does match.");
            }
        }
    }
}
