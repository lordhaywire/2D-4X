using Godot;
using System;
using System.Globalization;

namespace PlayerSpace;

public partial class Clock : Node
{
    public static Clock Instance { get; private set; }

    public event Action DailyHourOne;
    public event Action DailyHourTwo;
    public event Action DailyHourThree;
    public event Action DailyHourFour;

    public event Action Weekly;
    public event Action HourChanged; // This is currently used for battles.

    private int weeklyEvent = 7;

    [Export] private Label dayLabel;
    [Export] private Label hourLabel;
    [Export] private Label currentSpeedLabel;
    [Export] private Label pausedLabel;

    private float tickInterval = .1f; // This is where we are going to mess with the speed of the game to match movement speed.
    private float tickTimer; 

    private float timeMultiplier;

    public float TimeMultiplier
    {
        get => timeMultiplier;
        set
        {
            timeMultiplier = value;

            currentSpeedLabel.Text = timeMultiplier.ToString(CultureInfo.InvariantCulture);

            if (timeMultiplier == 0)
            {
                pausedLabel.Show();
            }
            else
            {
                pausedLabel.Hide();
            }
        }
    }

    private int minutes;
    private int hours;
    public int Hours
    {
        get => hours;
        private set
        {
            hours = value;
            // This will not trigger on day zero.
            if (hours == 1)
            {
                DailyHourOne?.Invoke();
            }
            if (hours == 2)
            {
                // This will happen on day zero as well.
                if (days % weeklyEvent == 0)
                {
                    Weekly?.Invoke();
                }
                DailyHourTwo?.Invoke();
            }
            if (hours == 3)
            {
                DailyHourThree?.Invoke();
            }
            if (hours == 4)
            {
                DailyHourFour?.Invoke();
            }

            HourChanged?.Invoke();
            //GD.Print("Hours: " + hours);
        }
    }

    private int days;

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

    public override void _Ready()
    {
        Instance = this;

        timeMultiplier = 1;
        oldTimeSpeed = 1;

        if (Globals.Instance.startPaused == true)
        {
            PauseTime();
        }
    }

    public override void _Process(double delta)
    {
        Ticker(delta);
    }

    private void Ticker(double delta)
    {
        tickTimer += (float)delta * timeMultiplier;
        while (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            DoTick();
        }
    }

    private void DoTick()
    {
        minutes += 15;
        GD.Print(minutes);
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
        PopulateTimeLabels();
    }

    private void PopulateTimeLabels()
    {
        dayLabel.Text = days.ToString();
        hourLabel.Text = $"{Hours:00}:{minutes:00}";
    }

    private void TimeChangeButtonOnPressed(float i)
    {
        timeMultiplier = i;
        currentSpeedLabel.Text = timeMultiplier.ToString(CultureInfo.CurrentCulture);
    }
    public void PauseTime()
    {
        GD.Print("Pause Time!");

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

    /*
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

    }
    */

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