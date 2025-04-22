using Godot;
using System;

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

    private float
        tickInterval = .1f; // This is where we are going to mess with the speed of the game to match movement speed.

    private float tickTimer;

    private float timeMultiplier = 1;
    private int numberOfThingsPausing;

    public float TimeMultiplier
    {
        get => timeMultiplier;
        private set
        {
            //NumberOfThingsPausing = value > 0? 0: 1;
            //oldTimeMultiplier = timeMultiplier;
            timeMultiplier = value;

            TopBarControl.Instance.UpdateTimeMultiplierLabel(value);

            TopBarControl.Instance.UpdatePauseLabel(timeMultiplier == 0);
        }
    }

    private int Minutes { get; set; }

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
                if (Days % weeklyEvent == 0)
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

    public int Days { get; private set; }

    [Export] public float oldTimeMultiplier;

    public override void _Ready()
    {
        Instance = this;

        oldTimeMultiplier = 1;
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
        Minutes += 15;
        //GD.Print(Minutes);
        if (Minutes >= 60) // 60 min = 1 hr
        {
            Minutes = 0;
            Hours += 1;
        }

        if (Hours >= 24) // 24 hr = 1 day
        {
            Hours = 0;
            Days += 1;
        }

        TopBarControl.Instance.PopulateTimeLabels(Days, Hours, Minutes);
    }


    public void UpdateTimeMultiplier(float newTimeMultiplier)
    {
        TimeMultiplier = newTimeMultiplier;
    }

    public void PauseTime()
    {
        GD.Print("Pause Time!");

        if (TimeMultiplier > 0)
        {
            oldTimeMultiplier = TimeMultiplier;
            TimeMultiplier = 0;
        }

        numberOfThingsPausing++;
    }

    public void UnpauseTime()
    {
        //GD.Print("Unpause Time!");
        numberOfThingsPausing--;
        TimeMultiplier = oldTimeMultiplier;
    }

    public string GetDateAndTime()
    {
        string dateAndTime = $"{Days} {Hours:00}:{Minutes:00}";
        return dateAndTime;
    }

    public void SpaceBarPause()
    {
        PauseAndUnpause();
    }
    public void PauseAndUnpause()
    {
        GD.Print("Pause and Unpause has been fired!");

        if (TimeMultiplier > 0)
        {
            oldTimeMultiplier = TimeMultiplier;
            TimeMultiplier = 0;
            numberOfThingsPausing++;
        }
        else
        {
            if (numberOfThingsPausing <= 1)
            {
                (TimeMultiplier, oldTimeMultiplier) = (oldTimeMultiplier, TimeMultiplier);
            }
            numberOfThingsPausing--;
        }
        //GD.Print($"Modified Time: {ModifiedTimeScale} and Old Time Speed: {oldTimeSpeed}.");
    }
}