using System.Collections.Generic;

namespace Console
{
    public class History
    {
        private List<string> logs;
        private int index;

        public History()
        {
            logs = new List<string>();
            index = 0;
        }

        public string GetLog()
        {
            if (index == logs.Count)
                return "";
            return logs[index];
        }

        public void AddLog(string text)
        {
            if (logs.Count > 0 && logs[logs.Count - 1] == text) return;
            logs.Add(text);
            index = logs.Count;
        }

        public void IncreaseIndex()
        {
            if (index == logs.Count)
                return;
            else
                index++;
        }

        public void DecreaseIndex()
        {
            if (index == 0)
                return;
            else
                index--;
        }
    }
}
