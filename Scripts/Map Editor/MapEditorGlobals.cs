using Godot;
using PlayerSpace;

namespace MapEditorSpace
{
	public partial class MapEditorGlobals : Node
	{
        public static MapEditorGlobals Instance { get; private set; }

        [Export] public Texture2D map;
        [Export] public Texture2D mapColorCoded;

        [Export] public PackedScene countyPackedScene;

        [Export] public Node2D countiesParent;
        [Export] public CountyData[] countyDatas;

        [Export] public string pathToCounties = "res://Counties/";
        public override void _Ready()
        {
            Instance = this;
        }
    }
}