using System;
using System.Runtime.InteropServices;

namespace BlazorDemo.Data
{
    public class SaleInfo
    {
        public int OrderId { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
}