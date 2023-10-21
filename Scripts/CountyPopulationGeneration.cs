using Godot;
using Godot.Collections;

namespace PlayerSpace
{
    public partial class CountyPopulationGeneration : Node
    {
        [Export] Node2D countiesParent;

        private void CreatePopulation()
        {
            
            /*
            // Create various county specific data.
            for (int i = 0; i < countiesParent.GetChildCount(); i++)
            {
                // Initilizes the List in the Dictionaries for Counties.
                
                   countyPopulationDictionary[countyName] = new List<CountyPopulation>();

                   if (counties[countyName].isCapital == true)
                   {
                       GeneratePopulation(countyName, totalCapitolPop);
                       counties[countyName].population = totalCapitolPop;
                       counties[countyName].IdleWorkers = totalCapitolPop;
                       countyPopulationDictionary[countyName][0].isHero = true;
                       counties[countyName].faction.factionLeader = countyPopulationDictionary[countyName][0];
                       countyHeroes[countyName] = new List<CountyPopulation>
                   {
                       countyPopulationDictionary[countyName][0]
                   };
                       //Debug.Log($"County Heroes:  {countyName} {countyHeroes[countyName][0].firstName}");
                   }
                   else if (counties[countyName].isIndependentCapital == true)
                   {
                       int normalPopulation = UnityEngine.Random.Range(minimumCountyPop, maximumCountyPop);
                       GeneratePopulation(countyName, normalPopulation);
                       counties[countyName].population = normalPopulation;
                       counties[countyName].IdleWorkers = normalPopulation;
                       countyPopulationDictionary[countyName][0].isHero = true;
                       counties[countyName].faction.factionLeader = countyPopulationDictionary[countyName][0];
                       countyHeroes[countyName] = new List<CountyPopulation>
                   {
                       countyPopulationDictionary[countyName][0]
                   };
                       //Debug.Log($"County Heroes:  {countyName} {countyHeroes[countyName][0].firstName}");
                   }
                   else if (counties[countyName].isCapital == false && counties[countyName].isIndependentCapital == false)
                   {
                       int normalPopulation = UnityEngine.Random.Range(minimumCountyPop, maximumCountyPop);
                       GeneratePopulation(countyName, normalPopulation);
                       counties[countyName].population = normalPopulation;
                       counties[countyName].IdleWorkers = normalPopulation;
                       countyHeroes[countyName] = new List<CountyPopulation>();
                   }
                   counties[countyName].countyPopulation = countyPopulationDictionary[countyName];
               }
           }


           private void GeneratePopulation(string countyName, int totalPopulation)
           {
               for (int i = 0; i < totalPopulation; i++)
               {
                   var countyPopulation = countyPopulationDictionary[countyName];
                   string firstName;
                   string lastName;
                   bool isMale;
                   int age;
                   bool isFactionLeader = false;
                   bool leaderOfPeoplePerk = false;

                   // Generates Persons Last Name
                   int randomLastNameNumber = UnityEngine.Random.Range(0, lastNames.Length);
                   lastName = lastNames[randomLastNameNumber];

                   // Determine the persons sex and first name
                   int randomSexNumber = UnityEngine.Random.Range(0, 2);
                   int randomFemaleNameNumber = UnityEngine.Random.Range(0, femaleNames.Length);
                   int randomMaleNameNumber = UnityEngine.Random.Range(0, maleNames.Length);

                   if (randomSexNumber == 0)
                   {
                       isMale = true;
                       firstName = maleNames[randomMaleNameNumber];
                   }
                   else
                   {
                       isMale = false;
                       firstName = femaleNames[randomFemaleNameNumber];
                   }

                   // Determine the person's age.
                   age = UnityEngine.Random.Range(18, 61);

                   if (i == 0)
                   {
                       isFactionLeader = true;

                       leaderOfPeoplePerk = true;
                   }

                   // Generate random skill level for each population.
                   int constructionSkill = UnityEngine.Random.Range(20, 81);

                   // This adds to the Dictionary List a new person.
                   countyPopulation.Add(new CountyPopulation(null, null, counties[countyName].faction.factionNameAndColor.name,
                       firstName, lastName, isMale, age, isFactionLeader, false, false, leaderOfPeoplePerk, constructionSkill,
                       AllText.Jobs.IDLE, null, AllText.Jobs.IDLE, null, false));
               }
                
            }
            */

        }
    }
}