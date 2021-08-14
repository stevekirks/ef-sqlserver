using EfSqlServer.Ef;
using System;
using System.Linq.Expressions;

namespace EfSqlServer.Models
{
    public static class FoodResultHelpers
    {
        public static Expression<Func<Food, FoodResultModel>> Map()
        {
            return (food) => new FoodResultModel
            {
                Code = food.Id,
                Ingredient = food.Name,
                DateCreated = food.Timestamp,

                CalculatedSource = "Dinner"
            };
        }
    }
}
