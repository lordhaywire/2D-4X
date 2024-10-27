using Godot;
using System;

namespace PlayerSpace;

public partial class Clock : Node
{
    public static Clock Instance { get; private set; }

    public event Action SetDay;
    public event Action HourChanged; // This is currently used for battles.
    
    [Export] private Label dayLabel;
    [Export] private Label HourLabel;
    [Export] private Label currentSpeedLabel;
    [Export] private Label pausedLabel;

    [Export] private int ticks;

    //public float foreverTimer; // This will eventually need to be reset.  I think.  It depends on if we run out of numbers.
    public float minutes;
    private int hours;
    public int days = 0;

    [Export] public int oldTimeSpeed;
    private int numberOfThingsPausing = 0; // For when there are multiple panels open.

    [Export]
    public int NumberOfThingsPausing
    {
        get { return numberOfThingsPausing; }
        set
        {
            numberOfThingsPausing = value;
            //GD.Print("Number of things paused: " + numberOfThingsPausing);
        }
    }

    public void PauseTime()
    {
        //GD.Print("Pause Time!");

        if (ModifiedTimeScale > 0)
        {
            oldTimeSpeed = ModifiedTimeScale;
            ModifiedTimeScale = 0;
        }
        NumberOfThingsPausing++;
    }

    public void UnpauseTime()
    {
        //GD.Print("Unpause Time!");
        NumberOfThingsPausing--;
        if (NumberOfThingsPausing == 0)
        {
            ModifiedTimeScale = oldTimeSpeed;
        }
    }

    public int Hours
    {
        get { return hours; }
        set
        {
            hours = value;
            // This will not trigger on day zero.
            if (hours == 1)
            {
                SetDay?.Invoke();
            }
            HourChanged?.Invoke();
            //GD.Print("Hours: " + hours);
        }
    }

    private int modifiedTimeScale;
    [Export]
    public int ModifiedTimeScale
    {
        get
        {
            return modifiedTimeScale;
        }
        set
        {
            modifiedTimeScale = value;
            
            currentSpeedLabel.Text = modifiedTimeScale.ToString();

            if (modifiedTimeScale == 0)
            {
                pausedLabel.Show();
                GetTree().Paused = true;
            }
            else
            {
                pausedLabel.Hide();
                Engine.TimeScale = value;
                GetTree().Paused = false;
            }           
        }
    }

    public override void _Ready()
    {
        Instance = this;

        ModifiedTimeScale = 1;
        oldTimeSpeed = 1;
        
        if (Globals.Instance.startPaused == true)
        {
            PauseTime();
        }
    }

    // Should this be _PhysicsProcess or just _Process
    public override void _PhysicsProcess(double delta)
    {
        //GD.Print("Delta: " + delta);
        TimeKeeper();
    }

    private void TimeKeeper() // Used to calculate sec, min and Hours
    {
        double fixedDeltaTime = GetPhysicsProcessDeltaTime(); // I dont even know if this is equivlent to fixed delta time Unity.
        minutes += (float)fixedDeltaTime * ticks; // multiply time between fixed update by tick.
        //foreverTimer += Engine.fixedDeltaTime * ticks;

        if (minutes >= 60) // 60 min = 1 hr
        {
            minutes = 0;
            Hours += 1;
        }

        if (Hours >= 24) // 24 hr = 1 day
        {
            Hours = 0;
            days += 1;
        }
        // To show Days, Hours and minutes.
        dayLabel.Text = days.ToString();
        HourLabel.Text = string.Format("{0:00}:{1:00}", Hours, minutes);
    }

    public string GetDateAndTime()
    {
        string dateAndTime = $"{days} {Hours:00}:{minutes:00}";
        return dateAndTime;
    }
    public void PauseandUnpause()
    {
        //GD.Print("Keyboard has been pressed!");
        if (ModifiedTimeScale > 0)
        {
            NumberOfThingsPausing++;
            oldTimeSpeed = ModifiedTimeScale;
            ModifiedTimeScale = 0;
        }
        else
        {
            (ModifiedTimeScale, oldTimeSpeed) = (oldTimeSpeed, ModifiedTimeScale);
            NumberOfThingsPausing--;
        }
        //GD.Print($"Modified Time: {ModifiedTimeScale} and Old Time Speed: {oldTimeSpeed}.");
    }

    public void ChangeSpeed(int speed)
    {
        if (speed > 0) 
        {
            numberOfThingsPausing = 0;
        }
        else
        {
            numberOfThingsPausing = 1;
        }
        oldTimeSpeed = ModifiedTimeScale;
        ModifiedTimeScale = speed;
    }
}