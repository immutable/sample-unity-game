using System;

namespace HyperCasual.Runner
{
    [Serializable]
    public class PageModel
    {
        public string previous_cursor;
        public string next_cursor;
    }
}