using System;

namespace AndreasReitberger.SQL.Events
{
    public class DatabaseQueryResultEventArgs : EventArgs
    {
        #region Properties
        public string SqlCommand { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }

        #endregion

        #region Overrides
        /*
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        */
        #endregion
    }
}
