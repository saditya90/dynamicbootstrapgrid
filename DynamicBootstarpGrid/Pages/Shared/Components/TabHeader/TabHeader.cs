using Microsoft.AspNetCore.Mvc;
using System;

namespace DynamicBootstarpGrid.Pages.Shared.Components.TableHeader
{
    public class TabHeader : ViewComponent
    {
        public string ColumnName { get; set; }
        public string DataColumnName { get; set; }
        public bool IsCurrentSorted { get; set; }
        public bool IsAsc { get; set; }
        public IViewComponentResult Invoke(string col, string dCol, string sortCol, string dir)
        {
            ColumnName = col;
            DataColumnName = dCol;
            IsCurrentSorted = dCol.Equals(sortCol, StringComparison.InvariantCultureIgnoreCase);
            IsAsc = dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase);
            return View("Index", this);
        }
    }
}
