using System.Collections.Generic;

namespace DynamicBootstarpGrid.Models
{
    public class PaginationItems
    {
        public int Index { get; set; }
        public bool IsCurrent { get; set; }
        public string Active { get; set; } 
    }

    public class Pagination
    {
        public List<PaginationItems> Items { get; set; } = new List<PaginationItems>();
        public bool IsPreviousDisabled { get; set; }
        public bool IsNextDisabled { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; } 
        public string DisablePrev { get { return IsPreviousDisabled ? "disabled" : "";  } }
        public string DisableNext { get { return IsNextDisabled ? "disabled" : "";  } }
    }
}
