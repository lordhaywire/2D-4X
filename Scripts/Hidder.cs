using Godot;
using System;

namespace PlayerSpace
{
	public partial class Hidder : Node
	{
		[Export] Control[] nodesThatNeedClosing;
		public override void _Ready()
		{
			foreach(Control node in nodesThatNeedClosing)
			{
				node.Hide();
			}
		}
	}
}