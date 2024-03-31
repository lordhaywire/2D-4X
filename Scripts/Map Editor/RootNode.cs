using Godot;
using System.Threading.Tasks;

namespace MapEditorSpace
{


    public partial class RootNode : Node
    {
        public static RootNode Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
            GetTree().Paused = false;
        }

        public async Task WaitFrames(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                await ToSignal(GetTree(), "process_frame");
            }
        }
    }
}