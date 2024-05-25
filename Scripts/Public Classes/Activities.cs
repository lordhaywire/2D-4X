using Godot;

namespace PlayerSpace
{
    public class Activities
    {
        public void UpdateNext(CountyPopulation countyPopulation, AllEnums.Activities activity)
        {
            countyPopulation.nextActivity = activity;
        }

        public void UpdateCurrent(CountyPopulation countyPopulation, AllEnums.Activities activity)
        {
            countyPopulation.currentActivity = activity;
        }

        public string GetActivityName(AllEnums.Activities activity)
        {
            string name = TranslationServer.Translate(AllActivities.Instance.allActivityData[(int)activity].name);
            return name;
        }
    }
}