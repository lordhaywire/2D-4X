using Godot;

namespace PlayerSpace
{
    public partial class CameraControls : CharacterBody2D
    {
        [Export] public int Speed { get; set; } = 2500;

        // Zoom parameters
        [Export] private Vector2 zoomSpeed = new(0.1f, 0.1f); // Adjust the speed as needed
        [Export] private Vector2 minZoom = new(.5f, .5f);   // Set your desired minimum zoom
        [Export] private Vector2 maxZoom = new(3.0f, 3.0f);   // Set your desired maximum zoom

        [Export] private Camera2D camera; // Reference to your Camera2D node

        public void GetInput()
        {
            Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

            Velocity = (inputDirection * Speed) / Mathf.Max(Clock.Instance.ModifiedTimeScale, 1);

        }

        public override void _PhysicsProcess(double delta)
        {
            GetInput();
            MoveAndSlide();
        }

        public override void _Input(InputEvent @event)
        {
            if (Globals.Instance.playerControlsEnabled == true)
            {
                if (@event.IsActionPressed("mouse_wheel_up"))
                {
                    ZoomIn();
                }
                else if (@event.IsActionPressed("mouse_wheel_down"))
                {
                    ZoomOut();
                }
            }
        }

        private void ZoomIn()
        {
            Vector2 newZoom = camera.Zoom + zoomSpeed;
            //GD.Print("Zoom In: " + newZoom);

            camera.Zoom = newZoom.Clamp(minZoom, maxZoom);
        }

        private void ZoomOut()
        {
            Vector2 newZoom = camera.Zoom - zoomSpeed;
            //GD.Print("Zoom Out: " + newZoom);
            camera.Zoom = newZoom.Clamp(minZoom, maxZoom);
        }
    }
}
