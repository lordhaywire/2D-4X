using Godot;
using PlayerSpace;
using System;

public partial class Camera : Camera2D
{
    [Export] public int Speed { get; private set; } = 2000;

    [Export] private int MinSpeed = 500;
    [Export] private int MaxSpeed = 2000;


    // Zoom parameters
    [Export] private Vector2 zoomSpeed = new(0.1f, 0.1f); // Adjust the speed as needed
    [Export] private Vector2 minZoom = new(.5f, .5f);   // Set your desired minimum zoom
    [Export] private Vector2 maxZoom = new(3.0f, 3.0f);   // Set your desired maximum zoom

    public bool cameraControlsEnabled = true;
    public bool zoomEnabled = true;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 dir = Vector2.Zero;

        if (cameraControlsEnabled) 
        {
            dir = Input.GetVector("camera_move_left", "camera_move_right", "camera_move_up", "camera_move_down");
        }


        // this makes it so that at the min zoom (zoomed out) is at the min pan speed
        // and at the 50% zoom percentage it is at the max zoom speed
        // and finally at the max zoom (zoomed in) it is again at the min pan speed


        float zoomInPercentage = Zoom.Length() / maxZoom.Length();
        float zoomOutPercentage = Zoom.Length() / minZoom.Length();

        float panSpeed = Speed;

        if (zoomInPercentage < 0.5f)
        {
            panSpeed = Mathf.Clamp(zoomInPercentage * Speed, MinSpeed, MaxSpeed);
        }
        else
        {
            panSpeed = Mathf.Clamp(1 / zoomOutPercentage * Speed, MinSpeed, MaxSpeed);
        }


        GD.Print(panSpeed);


        Position += dir * panSpeed * (float)delta;


    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (cameraControlsEnabled == true)
        {
            // may need to check if @event is a mouse wheel button for it to work as a unhandled input
            // godot quirk 

            if (@event is InputEventMouseButton) 
            {
                if (zoomEnabled == true)
                {

                    if (@event.IsActionPressed("camera_zoom_in"))
                    {

                        ZoomIn();

                    }
                    else if (@event.IsActionPressed("camera_zoom_out"))
                    {
                        ZoomOut();

                    }
                }
            }
        }
    }

    public void AdjustZoomEnabled()
    {
        zoomEnabled = !zoomEnabled;
    }

    private void ZoomIn()
    {
        Vector2 newZoom = Zoom + zoomSpeed;
        Zoom = newZoom.Clamp(minZoom, maxZoom);
    }

    private void ZoomOut()
    {
        Vector2 newZoom = Zoom - zoomSpeed;
        Zoom = newZoom.Clamp(minZoom, maxZoom);
    }
}

