using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Time
{
    [MoonSharpUserData]
    public class TimeManager : Manager.Manager
    {
        private static TimeManager instance;
        public static TimeManager Instance => instance;

        public event EventHandler OnPaused;
        public event EventHandler OnResumed;

        public event EventHandler<Intervals> OnIntervalChanged;

        public event EventHandler<Date> OnHourPass;
        public event EventHandler<Date> OnDayPass;
        public event EventHandler<Date> OnMonthPass;
        public event EventHandler<Date> OnYearPass;

        public event EventHandler<Calendar.Calendar> OnCalendarLoaded;
        public event EventHandler<Intervals> OnIntervalsLoaded;
        public event EventHandler<Date> OnStartDateLoaded;
        public event EventHandler OnIntervalLooped;

        private Calendar.Calendar calendar;
        public Intervals Intervals { get; private set; }
        public Date Date { get; private set; }

        public bool IsPaused { get; private set; }

        public bool IsReady { get; private set; }


        private float lastDifference;
        private float lastCheckTime;

        private bool ignore_input;

        private void Awake()
        {
            if (!instance)
                instance = this;
            else return;
        }

        /// <summary>
        /// Handled by the Game.Manager
        /// </summary>
        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "time/manager.lua";

            LoadScript();
            LoadCalendar();
            LoadIntervals();

            Pause();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadDate(path);
            IsReady = true;

            RaiseOnContentLoaded();
        }

        private void Update()
        {
            if (IsReady)
            {
                HandleInput();
                CheckForIntervalLoop();
            }
        }

        private void CheckForIntervalLoop()
        {
            if (IsPaused)
            {
                lastCheckTime = UnityEngine.Time.time;
                return;
            }
            else if (UnityEngine.Time.time > lastCheckTime + Intervals.Interval.tick_in_seconds)
            {
                lastCheckTime = UnityEngine.Time.time;
                OnIntervalLooped?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Play() 
        {
            IsPaused = false;
            lastCheckTime = UnityEngine.Time.time - lastDifference;

            OnResumed?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            IsPaused = true;
            lastDifference = UnityEngine.Time.time - lastCheckTime;

            OnPaused?.Invoke(this, EventArgs.Empty);
        }

        public void ChangeInterval(int to)
        {
            Intervals.SelectInterval(to);
            OnIntervalChanged?.Invoke(this, Intervals);
        }

        #region HANDLERS
        private void HandleInput()
        {
            if (ignore_input) return;
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                ChangeInterval(Intervals.SelectedInterval + 1);
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                ChangeInterval(Intervals.SelectedInterval - 1);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPaused) 
                    Play();
                else 
                    Pause();
            }
        }

        private void HandleIntervalLooped(object sender, EventArgs e)
        {
            bool[] passState = Date.PassTime(calendar);
            
            if (passState[0])
            {
                OnHourPass?.Invoke(this, Date);
            }
            if (passState[1])
            {
                OnDayPass?.Invoke(this, Date);
            }
            if (passState[2])
            {
                OnMonthPass?.Invoke(this, Date);
            }
            if (passState[3])
            {
                OnYearPass?.Invoke(this, Date);
            }
        }

        private void HandleIntervalsLoaded(object sender, Intervals intervals)
        {
            intervals.SelectInterval(0);
        }

        private void HandleStartDateLoaded(object sender, Date date)
        {
            date.hour = (ushort)Mathf.Clamp(date.hour, 0, calendar.hours - 1);
            date.day = (ushort)Mathf.Clamp(date.day, 1, calendar.months[date.month].days);
            date.month = (ushort)Mathf.Clamp(date.month, 1, calendar.months.Length);
        }
        #endregion

        #region SUBSCRIPTIONS
        protected override void Subscribe()
        {
            OnIntervalsLoaded += HandleIntervalsLoaded;
            OnStartDateLoaded += HandleStartDateLoaded;
            OnIntervalLooped += HandleIntervalLooped;

            Console.UI.OnConsoleFocused += (sender, args) => { ignore_input = true; };
            Console.UI.OnConsoleDefocused += (sender, args) => { ignore_input = false; };
        }

        protected override void Unsubscribe()
        {
            OnIntervalsLoaded -= HandleIntervalsLoaded;
            OnStartDateLoaded -= HandleStartDateLoaded;
            OnIntervalLooped -= HandleIntervalLooped;
        }
        #endregion

        #region CONTENT LOADERS
        private void LoadCalendar()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/calendar/calendar.txt");
            if (json == null) return;

            calendar = Calendar.Calendar.FromJson(json);
            OnCalendarLoaded?.Invoke(this, calendar);
        }

        private void LoadIntervals()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/time/intervals.txt");
            if (json == null) return;

            Intervals = Intervals.FromJson(json);
            OnIntervalsLoaded?.Invoke(this, Intervals);
        }

        private void LoadDate(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/calendar/start_date.txt");
            if (json == null) return;

            Date = Date.FromJson(json);
            OnStartDateLoaded?.Invoke(this, Date);
        }
        #endregion
    }
}
