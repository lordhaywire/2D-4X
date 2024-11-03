using Godot;
using System;

namespace PlayerSpace
{
    public partial class CameraControls : CharacterBody2D
    {
        public static CameraControls Instance { get; private set; }
        [Export] public int Speed { get; set; } = 2500;

        // Zoom parameters
        [Export] private Vector2 zoomSpeed = new(0.1f, 0.1f); // Adjust the speed as needed
        [Export] private Vector2 minZoom = new(.5f, .5f);   // Set your desired minimum zoom
        [Export] private Vector2 maxZoom = new(3.0f, 3.0f);   // Set your desired maximum zoom

        [Export] private Camera2D camera; // Reference to your Camera2D node
        public bool cameraControlsEnabled = true;
        public bool zoomEnabled = true;

        public override void _Ready()
        {
            Instance = this;

        }
        
        public void GetInput()
        {
            if (cameraControlsEnabled && PlayerControls.Instance.playerControlsEnabled)
            {
                Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
                Velocity = inputDirection * Speed / Math.Max((int)Engine.TimeScale, 1);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            GetInput();
            MoveAndSlide();
        }


        public override void _Input(InputEvent @event)
        {
            if (PlayerControls.Instance.playerControlsEnabled == true && cameraControlsEnabled == true)
            {
                if (@event.IsActionPressed("mouse_wheel_up"))
                {
                    if (zoomEnabled == true)
                    {
                        ZoomIn();
                    }
                }
                else if (@event.IsActionPressed("mouse_wheel_down"))
                {
                    if (zoomEnabled == true)
                    {
                        ZoomOut();
                    }
                }
            }
        }

        public void AdjustZoomEnabled()
        {
            zoomEnabled = !zoomEnabled;
            //GD.Print("Zoom Enabled: " + zoomEnabled);
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
