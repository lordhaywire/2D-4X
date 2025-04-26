using System;

namespace PlayerSpace;

// This needs to be moved to AttributeData.
public class Rolls
{
    private readonly Random random = new();
    public bool Attribute(AttributeData attributeToCheck)
    {
        bool success = false;
        int roll = random.Next(1, 101);

        if (roll <= attributeToCheck.attributeLevel)
        {
            success = true;
            //GD.Print("Attribute Roll Success? " + success);
        }
        return success;
    }
}