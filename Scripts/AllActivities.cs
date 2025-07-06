using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class AllActivities : Node
{
    //public static AllActivities Instance { get; private set; }

    private string activitiesDirectory = "res://Resources/Activities/";
    public List<ActivityData> allActivityData = [];

    public override void _Ready()
    {
       // Instance = this;

        //allActivityData = Globals.Instance.ReadResourcesFromDisk(activitiesDirectory).Cast<ActivityData>().ToList();
    }
}