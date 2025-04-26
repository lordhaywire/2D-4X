using Godot;
using System;

namespace PlayerSpace;

public partial class Clock : Node
{
    public static Clock Instance { get; private set; }

    public event Action DailyHourZeroFirstQuarter;
    public event Action DailyHourZeroSecondQuarter;
    public event Action DailyHourZeroThirdQuarter;
    public event Action DailyHourZeroFourthQuarter;

    public event Action Weekly;
    public event Action HourChanged; // This is currently used for battles.

    private int weeklyEvent = 7;

    private float
        tickInterval = .1f; // This is where we are going to mess with the speed of the game to match movement speed.

    private float tickTimer;

    private float timeMultiplier = 1;
    private int numberOfThingsPausing = 1;

    public float TimeMultiplier
    {
        get => timeMultiplier;
        private set
        {
            numberOfThingsPausing = value > 0 ? 0 : 1;
            oldTimeMultiplier = timeMultiplier;
            timeMultiplier = value;

            TopBarControl.Instance.UpdateTimeMultiplierLabel(value);

            TopBarControl.Instance.UpdatePauseLabel(timeMultiplier == 0);
        }
    }

    private int minutes;

    private int Minutes
    {
        get => minutes;
        set
        {
            minutes = value;
            if (hours == 0)
            {
                switch (minutes)
                {
                    case 15:
                        DailyHourZeroFirstQuarter?.Invoke();
                        break;
                    case 30:
                        DailyHourZeroSecondQuarter?.Invoke();
                        break;
                    case 45:
                        DailyHourZeroThirdQuarter?.Invoke();
                        break;
                    case 60:
                        DailyHourZeroFourthQuarter?.Invoke();
                        break;
                }
            }
        }
    }


    private int hours;

    public int Hours
    {
        get => hours;
        private set
        {
            hours = value;
            if (hours == 1)
            {
                // This will happen on day zero as well.
                if (Days % weeklyEvent == 0)
                {
                    Weekly?.Invoke();
                }

                // DailyHourTwo?.Invoke();
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
        GD.Print("Unpause Time!");
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

    protected virtual void OnDailyHourZeroSecondQuarter()
    {
        DailyHourZeroSecondQuarter?.Invoke();
    }
}