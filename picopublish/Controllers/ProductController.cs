using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using picopublish.Services;
using picopublish.Models;

public class ProductController : Controller
{
    private readonly CsvProductService _productService;

    public ProductController(CsvProductService productService)
    {
        _productService = productService;
    }
    public IActionResult InsertProduct()
    {
        return View();
    }

    [HttpPost]
    public IActionResult InsertProduct(Product product)
    {
        _productService.AddProduct(product);
        return RedirectToAction("InsertProduct");
    }

    public IActionResult ViewProducts(int page = 1, string sortOrder = "id_asc")
    {
        int pageSize = 10;
        var products = _productService.GetProducts(1, int.MaxValue);

        // Apply sorting based on sortOrder
        switch (sortOrder)
        {
            case "id_desc":
                products = products.OrderByDescending(p => p.Id);
                break;
            case "name_asc":
                products = products.OrderBy(p => p.Name);
                break;
            case "name_desc":
                products = products.OrderByDescending(p => p.Name);
                break;
            default:
                products = products.OrderBy(p => p.Id); // Default to sorting by Id ascending
                break;
        }

        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentSort = sortOrder;
        ViewBag.TotalPages = (products.Count() + pageSize - 1) / pageSize;
        ViewBag.CurrentPage = page;
        return View(pagedProducts);
    }

    //[HttpGet]
    //public IActionResult Edit()
    //{
    //    var products = _productService.GetProducts;
    //    if (products == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(products);
    //}


    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (id == 0 || id == null)
        { id = 1; }

        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.AllIds = _productService.getAllProductIds();
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(int productId, int i = 2)
    {
        var product = _productService.GetProductById(productId);

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var isUpdated = _productService.UpdateProduct(product);
        if (isUpdated)
        {
            return RedirectToAction("ViewProducts");
        }

        ModelState.AddModelError("", "Unable to update product.");
        return View(product);
    }



}
