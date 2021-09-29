using DynamicBootstarpGrid.Models;
using Faker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicBootstarpGrid.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IndexModel> _logger;
        private const string DataCacheKey = "_dataCacheKey";
        /// <summary>
        /// This variable can be load from configuration.
        /// To make it more dynamic it should get load from datastore.
        /// </summary>
        private const int RecordsPerPage = 10;
        public List<Employee> Data { get; set; } = new List<Employee>();
        [BindProperty(SupportsGet = true)]
        public GridIndex GridParams { get; set; } = new GridIndex();
        public Pagination Pagination { get; set; } = new Pagination();
        public IndexModel(ILogger<IndexModel> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public void OnGet(GridIndex gridIndex)
        {
            InitilizeData(gridIndex);
        }

        private void InitilizeData(GridIndex gridIndex)
        {
            int totalPages;
            if (_memoryCache.TryGetValue(DataCacheKey, out List<Employee> data))
            {
                Data = data;
            }
            else
            {
                FillDataInMemory();
            }

            ApplyFilter(gridIndex);
            
            ApplySorting(gridIndex);

            totalPages = Data.Count % RecordsPerPage == 0 ? Data.Count / RecordsPerPage : (Data.Count / RecordsPerPage) + 1;
            
            GridParams.Current = gridIndex.Current; GridParams.Total = totalPages;

            if (gridIndex.Current == 1)
            {
                Data = Data.Skip(gridIndex.Current - 1).Take(RecordsPerPage).ToList();
                FillPagination(gridIndex, totalPages);
                return;
            }

            Data = Data.Skip(RecordsPerPage * (gridIndex.Current - 1)).Take(RecordsPerPage).ToList();
            
            FillPagination(gridIndex, totalPages);
        }

        private void ApplySorting(GridIndex gridIndex)
        {
            if (!string.IsNullOrEmpty(gridIndex.Col) && !string.IsNullOrEmpty(gridIndex.Dir))
            {
                var isAscending = gridIndex.Dir.Equals("asc", StringComparison.InvariantCultureIgnoreCase);

                Data = isAscending ? Data.OrderBy(q => q.GetType().GetProperty(gridIndex.Col).GetValue(q)).ToList() :
                    Data.OrderByDescending(q => q.GetType().GetProperty(gridIndex.Col).GetValue(q)).ToList();

                GridParams.Col = gridIndex.Col; GridParams.Dir = isAscending ? "asc" : "desc";
            }
        }

        private void ApplyFilter(GridIndex gridIndex)
        {
            if (!string.IsNullOrWhiteSpace(gridIndex.Search))
            {
                var ignoreCase = StringComparison.InvariantCultureIgnoreCase;

                Data = Data.Where(q => q.Country.Contains(gridIndex.Search, ignoreCase) ||
                    q.PhoneNumber.Contains(gridIndex.Search, ignoreCase) ||
                    q.Name.Contains(gridIndex.Search, ignoreCase) ||
                    q.SecurityNo.Contains(gridIndex.Search, ignoreCase)).ToList();

                GridParams.Search = gridIndex.Search; 
                gridIndex.Current = 1; GridParams.Current = 1;
            }
        }

        private void FillDataInMemory()
        {
            for (int i = 0; i < 200; i++)
            {
                Data.Add(new Employee
                {
                    Name = Name.FullName(),
                    Country = Country.Name(),
                    PhoneNumber = Phone.Number(),
                    SecurityNo = Identification.SocialSecurityNumber(false)
                });
            }
            _memoryCache.Set(DataCacheKey, Data);
            _logger.LogInformation("memoryCache set called");
        }

        private void FillPagination(GridIndex gridIndex, int totalPages)
        {
            Pagination.IsPreviousDisabled = gridIndex.Current == 1;
            Pagination.IsNextDisabled = gridIndex.Current == totalPages;
            for (int i = gridIndex.Current < 3 ? 1 : gridIndex.Current - 1; i <= totalPages; i++)
            {
                Pagination.Items.Add(new PaginationItems
                {
                    IsCurrent = gridIndex.Current == i,
                    Active = gridIndex.Current == i ? "active" : string.Empty,
                    Index = i
                });
                if (Pagination.Items.Count == 3) break;
            }
            Pagination.Next = Pagination.IsNextDisabled ? "" : (gridIndex.Current + 1).ToString();
            Pagination.Prev = Pagination.IsPreviousDisabled ? "" : (gridIndex.Current -1).ToString();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(new RouteValueDictionary {
                { "gridIndex.Current", GridParams.Current },
                { "gridIndex.Search", GridParams.Search },
                { "gridIndex.Col", GridParams.Col },
                { "gridIndex.Dir", GridParams.Dir }
            });
        }

        public class GridIndex
        {
            public int Current { get; set; } = 1;
            public int Total { get; set; }
            public string Search { get; set; }
            public string Col { get; set; } = "Name";
            public string Dir { get; set; } = "asc";
        }

        public class Employee
        {
            public string Name { get; set; }
            public string Country { get; set; }
            public string PhoneNumber { get; set; }
            public string SecurityNo { get; set; }
        }
    }
}
