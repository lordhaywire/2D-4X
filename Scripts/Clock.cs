using Godot;
using System;

namespace PlayerSpace
{
    public partial class Clock : Node
    {
        //public static TimeKeeper Instance;

        public event Action FirstRun;
        public event Action DayStart;
        public event Action WorkDayOver;

        [Export] private Label dayLabel;
        [Export] private Label HourLabel;
        //[SerializeField] private TextMeshProUGUI currentSpeedText;
        //[SerializeField] private GameObject pausedText;
        [Export] private int ticks;

        //public MapControls mapControls;


        public float foreverTimer; // This will eventually need to be reset.  I think.  It depends on if we run out of numbers.
        public float minutes;
        public int hours;
        public int days = 0;

        public int oldTimeSpeed;
        public int numberOfThingsPaused;

        public int Hours
        {
            get { return hours; }
            set
            {
                hours = value;
                /*
                // This will not trigger on day zero.
                if (hours == 0)
                {
                    GD.Print("Hour is ZERO!!!");
                    DayStart?.Invoke();
                }

                if (days == 0 && hours == 1)
                {
                    GD.Print("It is 1 am on day zero.");
                    FirstRun?.Invoke();

                }

                if (hours == 17)
                {
                    GD.Print("Workday is over!");
                    WorkDayOver?.Invoke();
                }
                */
            }
        }

        private int modifiedTimeScale;
        public int ModifiedTimeScale
        {
            get
            {
                return modifiedTimeScale;
            }
            set
            {
                modifiedTimeScale = value;
                Engine.TimeScale = modifiedTimeScale;
                //currentSpeedText.text = "Speed: " + modifiedTimeScale;
                //GD.Print($"ModifiedScale has changed to {modifiedTimeScale}");
                /*
                if (modifiedTimeScale == 0)
                {
                    pausedText.SetActive(true);
                }
                else
                {
                    pausedText.SetActive(false);
                }
                */
            }
        }

        public override void _Ready()
        {
            //Instance = this;
            GD.Print("Get Physics Process Delta Time: " + GetPhysicsProcessDeltaTime());
            //mapControls = new MapControls();
            ModifiedTimeScale = 1;
            numberOfThingsPaused = 0;

            /*
            if (Globals.Instance.startPaused == true)
            {
                PauseTime();
            }
            */

            //mapControls.Keyboard.Spacebar.performed += _ => PauseandUnpause();
            oldTimeSpeed = 1;
        }

        public override void _PhysicsProcess(double delta)
        {
            //GD.Print("Delta: " + delta);
            TimeKeeper();
        }

        private void TimeKeeper() // Used to calculate sec, min and Hours
        {
            GD.Print("Get Physics Process Delta Time: " + GetPhysicsProcessDeltaTime());
            double fixedDeltaTime = GetPhysicsProcessDeltaTime();
            minutes += (float)fixedDeltaTime * ticks; // multiply time between fixed update by tick.
            //foreverTimer += Engine.fixedDeltaTime * ticks;
            //GD.Print("Fixed Delta Engine: " + Engine.fixedDeltaTime);
            GD.Print("Minutes: " + minutes);

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
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed == false)
            {
                GD.Print($"{keyEvent.Keycode}");

                switch (keyEvent.Keycode)
                {
                    case Key.Space:
                        PauseandUnpause();
                        break;
                }
            }
        }

        public void TimeSpeedx0()
        {
            if (ModifiedTimeScale != 0)
            {
                oldTimeSpeed = ModifiedTimeScale;
                ModifiedTimeScale = 0;
                numberOfThingsPaused++;
            }
        }

        public void TimeSpeedx1()
        {
            ModifiedTimeScale = 1;
            oldTimeSpeed = 1;
        }

        public void TimeSpeedx2()
        {
            ModifiedTimeScale = 2;
            oldTimeSpeed = 2;
        }

        public void TimeSpeedx4()
        {
            ModifiedTimeScale = 4;
            oldTimeSpeed = 4;
        }

        public void TimeSpeedx8()
        {
            ModifiedTimeScale = 8;
            oldTimeSpeed = 8;
        }

        public void PauseTime()
        {
            //GD.Print("Pause Time!");
            //mapControls.Keyboard.Spacebar.Disable();
            numberOfThingsPaused++;
            if (ModifiedTimeScale != 0)
            {
                oldTimeSpeed = ModifiedTimeScale;
                ModifiedTimeScale = 0;
            }
        }

        public void UnpauseTime()
        {
            //GD.Print("Unpause Time!");
            //mapControls.Keyboard.Spacebar.Enable();

            numberOfThingsPaused--;
            if (numberOfThingsPaused == 0)
            {
                ModifiedTimeScale = oldTimeSpeed;
            }
        }
        public void PauseandUnpause()
        {
            GD.Print("Keyboard has been pressed!");
            if (ModifiedTimeScale > 0)
            {
                oldTimeSpeed = ModifiedTimeScale;
                ModifiedTimeScale = 0;
                numberOfThingsPaused++;
            }
            else
            {
                ModifiedTimeScale = oldTimeSpeed;
                numberOfThingsPaused--;
            }
            //GD.Print($"Modified Time: {ModifiedTimeScale} and Old Time Speed: {oldTimeSpeed}.");
        }
    }
}