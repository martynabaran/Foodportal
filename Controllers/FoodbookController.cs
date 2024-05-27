using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Foodbook.Models;
using Foodbook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Immutable;
namespace Foodbook.Controllers{

public struct AuthorDisplayElement{
    public int AuthorId {get; set;}
    public string FirstName {get; set;}
    public string LastName {get;set;}

    public List<int> RecipesPublished {get; set;}
    
}

public struct RecipeDisplayElement{
    public int RecipeId {get; set;}
    public int AuthorId {get; set;}
    public string AuthorName {get; set;}
    //public string AuthorSurname {get; set;}
    public string Title {get; set;}
    public DateTime publishingDate {get; set;}
    public string Description{get;set;}
    public string Ingridients{get; set;}
    public string Instructions{get; set;}
    public int Category {get; set;}
    public List<int> Ratings { get; set; }
    public int Rate => Ratings.Count > 0 ? CalculateAverageRating() : 0;

    private int CalculateAverageRating()
    {
        int sum = 0;
        foreach (int rating in Ratings)
        {
            sum += rating;
        }
        return sum / Ratings.Count;
    } 
}
public struct MealPlanItemDisplayElement {
    public int RecipeId{get;set;}
    public string Title {get; set;}
    public int Category {get; set;}
    public DayOfWeek Day {get; set;}
}
public struct MealPlanDisplay {
    public int MealPlanId {get; set;}
    public int userId {get; set;}
    public DateTime PlanStartDate {get; set;}

}

[Route("[controller]/")]
public class FoodbookController: Controller {
    private readonly FoodbookDBContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    

    public FoodbookController(FoodbookDBContext contextUser,IHttpContextAccessor httpContextAccessor){
        _db = contextUser;
        _httpContextAccessor = httpContextAccessor;
    }

     private void SetViewDataFromSession() {
        if (HttpContext.Session.GetString("username") == null) {
            ViewData["Username"] = "";
            ViewData["IsAdmin"] = "";

            return;
        }

        ViewData["Username"] = HttpContext.Session.GetString("username");
        ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
    }


     [Route("addcategory/")]
    public IActionResult AddCategory() {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        RecipeCategoryModel []recipeCategories = _db.RecipeCategories.ToArray();

        ViewData["RecipeCategories"] = recipeCategories;

        return View();
    }

    [Route("addcategory/")]
    [HttpPost]
    public IActionResult AddCategory(IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        if (((String ?) form["category-name"]) is null || form["category-name"] == "") {
            ViewData["AddCategoryMessage"] = "Name is required";

            return View();
        }

        if ((from recipeCategory in _db.RecipeCategories where recipeCategory.CategoryName == ((String ?) form["category-name"]) select recipeCategory).Count() > 0) {
            ViewData["AddCategoryMessage"] = "Category already exists";

            return View();
        }

        _db.RecipeCategories.Add(new RecipeCategoryModel {
            CategoryName = ((String ?) form["category-name"]) ?? ""
        });

        _db.SaveChanges();

        ViewData["AddCategoryMessage"] = "Category added successfully";

        return View();
    }



[Route("raterecipe/{recipeId:int}")]
public IActionResult RateRecipe(int recipeId ){
    if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
    {
        return RedirectToAction("Login", "User");
    }
    
    SetViewDataFromSession();
    var recipe = _db.Recipes.FirstOrDefault(rc => rc.RecipeId == recipeId);
    ViewData["Recipe"] = recipe;
    return View();
}


[Route("raterecipe/{recipeId:int}")]
[HttpPost]
public IActionResult RateRecipe(int recipeId,IFormCollection form ){
    
     if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        List<String> fields = new List<String> {
            "rate",
            "comment"
        };
           foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["RateRecipeMessage"] = "All fields are required";

                return View();
            }
        }
       ;
       System.Console.WriteLine("pipeline: rate "+ form["rate"] + " comment: "+form["comment"]);
        try{
            var id = _db.Users
    .Where(user => user.Username == HttpContext.Session.GetString("username"))
    .Select(user => user.UserId)
    .FirstOrDefault();
            _db.Ratings.Add(new RatingModel{
                RecipeId = recipeId,
                UserId = id,
                Rate = Int32.Parse((String ?) form["rate"] ?? "0"),
                Comment = ((String ?) form["comment"]) ?? ""
            });
           _db.SaveChanges();
           ViewData["RateRecipeMessage"] = "Rated succesfully"; 
        }
        catch (Exception){
             ViewData["RateRecipeMessage"] = "Invalid field format";

            return View();
        }

        return RedirectToAction("ListRecipes");
}



[Route("topsearch/")]
public IActionResult HighestScore(){
     if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();
        RecipeDisplayElement []bestRecipes = (
       from recipe in _db.Recipes
        join category in _db.RecipeCategories on recipe.Category equals category.RecipeCategoryId
        join author in _db.Users on recipe.userName equals author.Username
       // join rating in _db.Ratings on recipe.RecipeId equals rating.RecipeId into ratingGroup
        select new RecipeDisplayElement
    {
        RecipeId = recipe.RecipeId,
        AuthorId = author.UserId,
        AuthorName = author.Username,
        //AuthorSurname = author.LastName,
        Title = recipe.Title,
        publishingDate = recipe.PublicationDate,
        Description = recipe.Description,
        Ingridients = recipe.Ingridients,
        Instructions = recipe.Instructions,
        Category = category.RecipeCategoryId,
        Ratings =  (
            from rating in _db.Ratings
            where rating.RecipeId == recipe.RecipeId
            select rating.Rate
        ).ToList() // Pamiętaj, aby zainicjować listę ocen
    }).AsEnumerable() // Przełączenie na LINQ to Objects do użycia funkcji agregujących C#
.OrderByDescending(recipe => recipe.Rate)
.Take(10)
.ToArray();

        ViewData["TopSearch"] = bestRecipes;
        return View();
}

    }


}
