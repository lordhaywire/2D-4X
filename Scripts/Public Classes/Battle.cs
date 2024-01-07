using Godot;
using System;

namespace PlayerSpace
{
    public class Battle
    {
        public CountyData battleLocation;

        public Battle(CountyData battleLocation)
        {
            this.battleLocation = battleLocation;
        }
    }
}