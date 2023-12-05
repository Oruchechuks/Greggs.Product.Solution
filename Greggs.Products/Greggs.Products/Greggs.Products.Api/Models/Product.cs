using System.ComponentModel.DataAnnotations.Schema;

namespace Greggs.Products.Api.Models;

public class Product
{
    public string Name { get; set; }
    public decimal PriceInPounds { get; set; }

    // User Story 2 Solution
    // Added a new property to return the price in Euros based on the price in pounds
    // This property is not mapped to the database as it is not required to be stored in the database
    // The conversion rate is hardcoded as 1.11 which is not the best solution, but should stored in a configuration file (appsettings.json) or database
    [NotMapped]
    public decimal PriceInEuros
    {
        get
        {
            return PriceInPounds * 1.11M;
        }
    }
}
