using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private static readonly string[] Products = new[]
    {
        "Sausage Roll", "Vegan Sausage Roll", "Steak Bake", "Yum Yum", "Pink Jammie"
    };

    private readonly ILogger<ProductController> _logger;

    private readonly IDataAccess<Product> _dataAccess;

    public ProductController(ILogger<ProductController> logger, IDataAccess<Product> dataAccess)
    {
        _logger = logger;
        _dataAccess = dataAccess;
    }

    [HttpGet]
    public IEnumerable<Product> Get(int pageStart = 0, int pageSize = 5)
    {
        if (pageSize > Products.Length)
            pageSize = Products.Length;

        var rng = new Random();
        return Enumerable.Range(1, pageSize).Select(index => new Product
        {
            PriceInPounds = rng.Next(0, 10),
            Name = Products[rng.Next(Products.Length)]
        })
            .ToArray();
    }

    // User Story 1 Solution
    // Added a new endpoint to return the latest products from the data access layer
    [HttpGet]
    [Route("priceInPounds")]
    public IActionResult GetLatest(int pageStart = 0, int pageSize = 5)
    {
        var products = _dataAccess.List(pageStart, pageSize).Select(p => new
        {
            p.Name,
            p.PriceInPounds
        }).ToList();
        return Ok(products);
    }

    // User Story 2 Solution
    // Added a new endpoint to return the latest products from the data access layer with euro price
    // In an ideal case, the currency can be provided via the api then converted to the required currency,
    // so latest?currency=EUR for example or latest?currency=dollars
    // if withEuro is false, then throw an exception. A try catch block is used to catch the exception and return a bad request
    [HttpGet]
    [Route("priceInDifferentCurrency")]
    public IActionResult GetLatestWithEuro(bool withEuro = true, int pageStart = 0, int pageSize = 5)
    {
        try
        {
            if (!withEuro)
            {
                throw new Exception("Euro conversion is required for this endpoint. Please set 'withEuro' to true.");
            }
            var products = _dataAccess.List(pageStart, pageSize).Select(p => new
            {
                p.Name,
                p.PriceInPounds,
                p.PriceInEuros
            }).ToList();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}