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

        /// <summary>
        /// If a -1 is put in the attribute value then there is no bonus given.
        /// Return an int that is an attribute bonus.  It can return a negative, 10, or 1.
        /// The negative bool is there so that the bonuses are reversed, thus a good bonus would be a bad
        /// bonus.  This is used for such things like needs where a high mental strength would give a
        /// bonus which needs to be a resistance.
        /// 
        /// When the game runs loyalty is set which applies the attribute bonus to the check.
        /// </summary>
        /// <param name="attributeLevel"></param>
        /// <param name="ones"></param>
        /// <param name="negative"></param>
        /// <returns></returns>
        public static int GetAttributeBonus(int attributeLevel, bool ones, bool negative)
        {
            if(attributeLevel == -1)
            {
                return 0;
            }
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
                if (attributeLevel >= min && attributeLevel <= max)
                {
                    bonus = bonusValue;
                    break;
                }
            }
            if(ones == true)
            {
                bonus /= 5;
            }
            // We are making the number negative if the passed in bool negative is true.
            if (negative == true)
            {
                bonus *= -1;
            }
            //GD.PrintRich($"[rainbow]Attribute Bonus: {bonus}");
            return bonus;
        }


        public static Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> NewCopy()
        {
            Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> newAttributes = [];
            foreach (AttributeData attributeData in AllAttributes.Instance.allAttributes)
            {
                newAttributes.Add(attributeData.attribute, new AttributeData
                {
                    attribute = attributeData.attribute,
                    attributeName = attributeData.attributeName,
                    attributeAbbreviation = attributeData.attributeAbbreviation,
                    attributeDescription = attributeData.attributeDescription,
                    attributeLevel = attributeData.attributeLevel,
                });
            }
            return newAttributes;
        }
    }
}


