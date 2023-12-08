using Godot;
using System;

namespace PlayerSpace
{
    public partial class Clock : Node
    {
        public static Clock Instance { get; private set; }

        public event Action FirstRun;
        public event Action DayStart;
        public event Action WorkDayOver;

        [Export] private Label dayLabel;
        [Export] private Label HourLabel;
        [Export] private Label currentSpeedLabel;
        [Export] private Label pausedLabel;

        [Export] private int ticks;


        public float foreverTimer; // This will eventually need to be reset.  I think.  It depends on if we run out of numbers.
        public float minutes;
        public int hours;
        public int days = 0;

        [Export] public int oldTimeSpeed;
        private int numberOfPanelsVisible; // For when there are multiple panels open.

        [Export]
        public int NumberOfPanelsVisible
        {
            get { return numberOfPanelsVisible; }
            set
            {
                numberOfPanelsVisible = value;
                if (numberOfPanelsVisible > 0)
                {
                    GD.Print("Number of panels visible: " +  numberOfPanelsVisible);
                    EventLog.Instance.Hide();

                }
                else
                {
                    EventLog.Instance.Show();
                }
            }
        }

        public int Hours
        {
            get { return hours; }
            set
            {
                hours = value;
                // This isn't used yet.
                if (days == 0 && hours == 1)
                {
                    GD.Print("It is 1 am on day zero.");
                    FirstRun?.Invoke();

                }
                // This will not trigger on day zero.
                if (hours == 0)
                {
                    GD.Print("Hour is ZERO!!!");
                    DayStart?.Invoke();
                }
                if (hours == 17)
                {
                    GD.Print("Workday is over!");
                    WorkDayOver?.Invoke();
                }              
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
                GD.Print($"ModifiedScale has changed to {modifiedTimeScale}");
                Engine.TimeScale = value;
                currentSpeedLabel.Text = modifiedTimeScale.ToString();

                
                if (modifiedTimeScale == 0)
                {
                    pausedLabel.Show();
                }
                else
                {
                    pausedLabel.Hide();
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
            HourLabel.Text = (string.Format("{0:00}:{1:00}", Hours, minutes));
        }

        public void PauseandUnpause()
        {
            GD.Print("Keyboard has been pressed!");
            if (ModifiedTimeScale > 0)
            {
                NumberOfPanelsVisible++;
                oldTimeSpeed = ModifiedTimeScale;
                ModifiedTimeScale = 0;
            }
            else
            {
                (ModifiedTimeScale, oldTimeSpeed) = (oldTimeSpeed, ModifiedTimeScale);
                NumberOfPanelsVisible--;
            }
            //GD.Print($"Modified Time: {ModifiedTimeScale} and Old Time Speed: {oldTimeSpeed}.");
        }


        public void ChangeSpeed(int speed)
        {
            oldTimeSpeed = ModifiedTimeScale;
            ModifiedTimeScale = speed;
        }

        public void PauseTime()
        {
            GD.Print("Pause Time!");
            
            if (ModifiedTimeScale != 0)
            {
                oldTimeSpeed = ModifiedTimeScale;                
                ModifiedTimeScale = 0;
            }
            NumberOfPanelsVisible++;
        }

        public void UnpauseTime()
        {
            GD.Print("Unpause Time!");
            NumberOfPanelsVisible--;
            if (NumberOfPanelsVisible == 0)
            {
                ModifiedTimeScale = oldTimeSpeed;
            }
            
        }
    }
}