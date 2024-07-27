using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    [GlobalClass]

    public partial class AttributeData : Resource
    {
        [Export] public AllEnums.Attributes attribute;
        [Export] public string attributeAbbreviation;
        [Export] public string attributeName;
        [Export] public string attributeDescription;
        [Export] public int attributeLevel;

        public static int ApplyAttributeBonuses(int attribute, bool ones)
        {
            // List of all of the ranges for attribute bonuses.
            List<(int min, int max, int bonus)> attributeBonuses =
            [
                (1, 10, -20),
                (11, 20, -15),
                (21, 30, -10),
                (31, 40, -5),
                (41, 60, 0),
                (61, 70, 5),
                (71, 80, 10),
                (81, 90, 15),
                (91, 100, 20)
            ];
            int bonus = 0;
            // If the number is between the min and the max it gets the bonus.
            foreach ((int min, int max, int bonusValue) in attributeBonuses)
            {
                if (attribute >= min && attribute <= max)
                {
                    bonus = bonusValue;
                    break;
                }
            }
            if(ones == true)
            {
                bonus /= 5;
            }
            return bonus;
        }
    }
}

/*
switch (number)
{
    case int n when n > 90:
        // Do something for numbers above 90
        break;
    case int n when n > 80:
        // Do something for numbers above 80
        break;
    case int n when n > 70:
        // Do something for numbers above 70
        break;
    case int n when n > 60:
        // Do something for numbers above 60
        break;
    case int n when n > 40:
        // Do nothing, number is between 41 and 59.
        break;
    case int n when n > 30:
        // Do something for numbers above 30
        break;
    case int n when n > 20:
        // Do something for numbers above 20
        break;
    case int n when n > 10:
        // Do something for numbers above 10
        break;
    default:
        // Number is suck.
        break;
}
*/
