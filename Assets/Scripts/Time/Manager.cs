using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Time
{
    public class Manager : MonoBehaviour
    {
        public event Action<int> OnIntervalChanged;

        public event Action<ushort> OnDayPass;
        public event Action<ushort> OnMonthPass;
        public event Action<ushort> OnYearPass;

        public event Action<Calendar.Calendar> OnCalendarLoaded;
        public event Action<Intervals> OnIntervalsLoaded;
        public event Action<Date> OnStartDateLoaded;
        public event Action OnScriptLoaded;

        public event Action OnIntervalLooped;

        private Calendar.Calendar calendar;
        private Intervals intervals;
        private Date date;

        private float lastCheckTime;

        private Script script;

        private void Start()
        {
            Subscribe();

            LoadScript();
            LoadCalendar();
            LoadIntervals();
            LoadStartDate();
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
            if (UnityEngine.Time.time > lastCheckTime + intervals.Interval.day_in_seconds)
            {
                lastCheckTime = UnityEngine.Time.time;
                OnIntervalLooped?.Invoke();
                script.Call(script.Globals[nameof(OnIntervalLooped)]);
            }
        }

        public void ChangeInterval(int to)
        {
            intervals.SelectInterval(to);
            OnIntervalChanged?.Invoke(intervals.SelectedInterval);
            script.Call(script.Globals[nameof(OnIntervalChanged)], to);
        }

        #region HANDLERS
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                ChangeInterval(intervals.SelectedInterval + 1);
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                ChangeInterval(intervals.SelectedInterval - 1);
            }
        }

        private void HandleIntervalLooped()
        {
            bool[] passState = date.PassDay(calendar);

            if (passState[0])
            {
                OnDayPass?.Invoke(date.day);
                script.Call(script.Globals[nameof(OnDayPass)], date.day);
            }
            if (passState[1]) 
            {
                OnMonthPass?.Invoke(date.month);
                script.Call(script.Globals[nameof(OnMonthPass)], date.month);
            }
            if (passState[2])
            {
                OnYearPass?.Invoke(date.year);
                script.Call(script.Globals[nameof(OnYearPass)], date.month);
            }
        }

        private void HandleIntervalsLoaded(Intervals intervals)
        {
            intervals.SelectInterval(0);
        }

        private void HandleStartDateLoaded(Date date)
        {
            date.month = (ushort)Mathf.Clamp(date.month, 1, calendar.months.Length);
            date.day = (ushort)Mathf.Clamp(date.day, 1, calendar.months[date.month].days);
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

            this.script = new Script();
            this.script.Globals["Log"] = (Action<string>)Debug.Log;
            this.script.Globals["ChangeInterval"] = (Action<int>)ChangeInterval;

            this.script.DoString(script);

            OnScriptLoaded?.Invoke();
            this.script.Call(this.script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadCalendar()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/calendar/calendar.txt");
            if (json == null) return;

            calendar = Calendar.Calendar.FromJson(json);
            OnCalendarLoaded?.Invoke(calendar);
            script.Call(script.Globals[nameof(OnCalendarLoaded)], calendar);
        }

        private void LoadIntervals()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/time/intervals.txt");
            if (json == null) return;

            intervals = Intervals.FromJson(json);
            OnIntervalsLoaded?.Invoke(intervals);
            script.Call(script.Globals[nameof(OnIntervalsLoaded)], intervals);
        }

        private void LoadStartDate()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/calendar/start_date.txt");
            if (json == null) return;

            date = Date.FromJson(json);
            OnStartDateLoaded?.Invoke(date);
            script.Call(script.Globals[nameof(OnStartDateLoaded)], date);
        }
        #endregion
    }
}
