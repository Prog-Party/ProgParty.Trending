using System;

namespace ProgParty.Trending.Api.Result
{
    public class OverviewResult
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int Index { get; set; }

        public override string ToString() => Title;
    }
}
