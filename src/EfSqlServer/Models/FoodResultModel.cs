using System;

namespace EfSqlServer.Models
{
    public class FoodResultModel
    {
        public int Code { get; set; }
        public string Ingredient { get; set; }
        public DateTime? DateCreated { get; set; }

        public string CalculatedSource { get; set; }
    }
}
