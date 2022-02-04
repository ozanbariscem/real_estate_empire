using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Time
{
    public class Manager : MonoBehaviour
    {
        public enum STATE { PAUSED, PLAYING }

        private static Manager instance;
        public static Manager Instance => instance;

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
        public event EventHandler OnScriptLoaded;
        public event EventHandler OnIntervalLooped;

        private Calendar.Calendar calendar;
        public Intervals Intervals { get; private set; }
        public Date date { get; private set; }

        public bool IsPaused { get; private set; }

        private float lastDifference;
        private float lastCheckTime;

        private Script script;

        private void Awake()
        {
            if (!instance)
                instance = this;
            else return;
        }

        private void Start()
        {
            Subscribe();
        }

        /// <summary>
        /// Handled by the Game.Manager
        /// </summary>
        [MoonSharpHidden]
        public void Initialize()
        {
            LoadScript();
            LoadCalendar();
            LoadIntervals();
            LoadStartDate();

            Pause();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Update()
        {
            HandleInput();
            CheckForIntervalLoop();
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
                script.Call(script.Globals[nameof(OnIntervalLooped)]);
            }
        }

        public void Play() 
        {
            IsPaused = false;
            lastCheckTime = UnityEngine.Time.time - lastDifference;

            OnResumed?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnResumed)]);
        }

        public void Pause()
        {
            IsPaused = true;
            lastDifference = UnityEngine.Time.time - lastCheckTime;

            OnPaused?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnPaused)]);
        }

        public void ChangeInterval(int to)
        {
            Intervals.SelectInterval(to);
            OnIntervalChanged?.Invoke(this, Intervals);
            script.Call(script.Globals[nameof(OnIntervalChanged)], Intervals);
        }

        #region HANDLERS
        private void HandleInput()
        {
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
            bool[] passState = date.PassTime(calendar);
            
            if (passState[0])
            {
                OnHourPass?.Invoke(this, date);
                script.Call(script.Globals[nameof(OnHourPass)], date.hour);
            }
            if (passState[1])
            {
                OnDayPass?.Invoke(this, date);
                script.Call(script.Globals[nameof(OnDayPass)], date.day);
            }
            if (passState[2])
            {
                OnMonthPass?.Invoke(this, date);
                script.Call(script.Globals[nameof(OnMonthPass)], date.month);
            }
            if (passState[3])
            {
                OnYearPass?.Invoke(this, date);
                script.Call(script.Globals[nameof(OnYearPass)], date.year);
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
        private void Subscribe()
        {
            OnIntervalsLoaded += HandleIntervalsLoaded;
            OnStartDateLoaded += HandleStartDateLoaded;
            OnIntervalLooped += HandleIntervalLooped;
        }

        private void Unsubscribe()
        {
            OnIntervalsLoaded -= HandleIntervalsLoaded;
            OnStartDateLoaded -= HandleStartDateLoaded;
            OnIntervalLooped -= HandleIntervalLooped;
        }
        #endregion

        #region CONTENT LOADERS
        private void LoadScript()
        {
            string script = Utils.StreamingAssetsHandler.SafeGetString("vanilla/time/manager.lua");
            if (script == null) return;

            UserData.RegisterType<Calendar.Month>();
            UserData.RegisterType<Calendar.Calendar>();
            UserData.RegisterType<Date>();
            UserData.RegisterType<Interval>();
            UserData.RegisterType<Intervals>();
            UserData.RegisterType<Console.Console>();


            this.script = new Script();
            this.script.Globals["ChangeInterval"] = (Action<int>)ChangeInterval;
            this.script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;

            this.script.DoString(script);

            OnScriptLoaded?.Invoke(this, EventArgs.Empty);
            this.script.Call(this.script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadCalendar()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/calendar/calendar.txt");
            if (json == null) return;

            calendar = Calendar.Calendar.FromJson(json);
            OnCalendarLoaded?.Invoke(this, calendar);
            script.Call(script.Globals[nameof(OnCalendarLoaded)], calendar);
        }

        private void LoadIntervals()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/time/intervals.txt");
            if (json == null) return;

            Intervals = Intervals.FromJson(json);
            OnIntervalsLoaded?.Invoke(this, Intervals);
            script.Call(script.Globals[nameof(OnIntervalsLoaded)], Intervals);
        }

        private void LoadStartDate()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/calendar/start_date.txt");
            if (json == null) return;

            date = Date.FromJson(json);
            OnStartDateLoaded?.Invoke(this, date);
            script.Call(script.Globals[nameof(OnStartDateLoaded)], date);
        }
        #endregion
    }
}
