using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Foodbook.Models;
using Foodbook.Data;

namespace Foodbook.Controllers
{
    [Route("[controller]/")]
    public class RecipeController : Controller
    {
        private readonly FoodbookDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecipeController(FoodbookDBContext contextUser, IHttpContextAccessor httpContextAccessor)
        {
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


[Route("myrecipes/")]

        public IActionResult ListMyRecipes()
    {
           if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        var username = HttpContext.Session.GetString("username");

        RecipeDisplayElement []userRecipes = (
        from recipe in _db.Recipes
         where recipe.userName == username join
          category in _db.RecipeCategories on recipe.Category equals category.RecipeCategoryId join
           author in _db.Users on recipe.userName equals author.Username
            select new RecipeDisplayElement
        {
        RecipeId = recipe.RecipeId,
        AuthorId = author.UserId,
        AuthorName = author.Username,
        Title = recipe.Title,
        publishingDate = recipe.PublicationDate,
        Description = recipe.Description,
        Ingridients = recipe.Ingridients,
        Instructions = recipe.Instructions,
        Category = category.RecipeCategoryId,
        //Ratings = ratingGroup.Select(r => r.Rate).ToList()
        }
    ).ToArray();

        ViewData["MyRecipes"] = userRecipes;

        return View();
    }

    [Route("listrecipes/")]
    public IActionResult ListRecipes(){
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();
        RecipeDisplayElement []Recipes = (
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
    }
).ToArray();


        ViewData["Recipes"] = Recipes;

        return View();

    }

    [Route("viewrecipedetails/{recipeId:int}")]
    public IActionResult ViewRecipeDetails(int recipeId){
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }
       
        SetViewDataFromSession();
         var id = _db.Users
    .Where(user => user.Username == HttpContext.Session.GetString("username"))
    .Select(user => user.UserId)
    .FirstOrDefault();
        var recipe = _db.Recipes.FirstOrDefault(rc => rc.RecipeId == recipeId);
        RecipeDisplayElement RecipeDetails = new RecipeDisplayElement
    {
        RecipeId = recipe.RecipeId,
        AuthorId = id,
        AuthorName = HttpContext.Session.GetString("username"),
        Title = recipe.Title,
        publishingDate = recipe.PublicationDate,
        Description = recipe.Description,
        Ingridients = recipe.Ingridients,
        Instructions = recipe.Instructions,
        Category = recipe.Category,
        Ratings =  (
            from rating in _db.Ratings
            where rating.RecipeId == recipe.RecipeId
            select rating.Rate
        ).ToList() // Pamiętaj, aby zainicjować listę ocen
    };


        ViewData["RecipeDetails"] = RecipeDetails;

        return View();
    }

    [Route("addRecipe/")]
    public IActionResult AddRecipe() {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        RecipeCategoryModel []recipeCategories = _db.RecipeCategories.ToArray();

        ViewData["RecipeCategories"] = recipeCategories;

        return View();
    }
     
    [Route("addrecipe/")]
    [HttpPost]
    public IActionResult AddRecipe(IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        List<String> fields = new List<String> {
            "title",
            "authorName",
            "description",
            "ingridients",
            "instructions",
            "recipe-category-id"
        };

        foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["AddRecipeMessage"] = "All fields are required, missing "+ field;

                return View();
            }
        }

        try {
            List<RecipeCategoryModel> category = (
                from recipeCategory in _db.RecipeCategories where recipeCategory.RecipeCategoryId == Int32.Parse((String ?) form["recipe-category-id"] ?? "") select recipeCategory
            ).ToList();

            if (category.Count() == 0) {
                ViewData["AddRecipeMessage"] = "Invalid category";

                return View();
            }
            var id = _db.Users
    .Where(user => user.Username == HttpContext.Session.GetString("username"))
    .Select(user => user.UserId)
    .FirstOrDefault();
            //var id = _db.Users.Where(user => user.Username == HttpContext.Session.GetString("username")).Select(user=> user.UserId);
            foreach (String field in fields) {
                System.Console.WriteLine(form[field]);
            }
            _db.Recipes.Add(new RecipeModel {
               
                Title = ((String ?) form["title"]) ?? "",
                UserId = id,
                userName = ((String ?) form["authorName"]) ?? "",               
                Description = ((String ?) form["description"]) ?? "",
                Ingridients = ((String ?) form["ingridients"]) ?? "",
                Instructions = ((String ?) form["instructions"]) ?? "",
                Category = Int32.Parse((String ?) form["recipe-category-id"] ?? "0")
            });

            _db.SaveChanges();
        } catch (Exception) {
            ViewData["AddRecipeMessage"] = "Invalid field format";

            return View();
        }

        ViewData["AddRecipeMessage"] = "Book added successfully";

        return RedirectToAction("AddRecipe");
    }


    [Route("editrecipe/{recipeId:int}")]
public IActionResult EditRecipe(int recipeId) {
    if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) {
        return RedirectToAction("Login", "User");
    }

    SetViewDataFromSession();

    RecipeModel recipe = _db.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
    if (recipe == null) {
        return RedirectToAction("ListMyRecipes");
    }

    ViewData["RecipeCategories"] = _db.RecipeCategories.ToArray();
    ViewData["recipe-id"] = recipe.RecipeId;
    ViewData["title"] = recipe.Title;
    ViewData["author"] = recipe.userName;
    ViewData["description"] = recipe.Description;
    ViewData["ingridients"] = recipe.Ingridients;
    ViewData["instructions"] = recipe.Instructions;
    ViewData["publication-date"] = recipe.PublicationDate.ToString("yyyy-MM-dd");
    ViewData["recipe-category-id"] = recipe.Category;

    return View();
}

[Route("editrecipe/{recipeId:int}")]
[HttpPost]
public IActionResult EditRecipe(int recipeId, IFormCollection form) {
    if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) {
        return RedirectToAction("Login", "User");
    }

    SetViewDataFromSession();

    RecipeModel recipe = _db.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
    if (recipe == null) {
        return RedirectToAction("ListMyRecipes");
    }

    List<string> fields = new List<string> { "title", "description", "ingridients", "instructions", "recipe-category-id" };

    foreach (string field in fields) {
        if (string.IsNullOrEmpty(form[field])) {
            ViewData["EditRecipeMessage"] = "All fields are required";
            return View();
        }
    }

    try {
        int categoryId = int.Parse(form["recipe-category-id"]);
        if (!_db.RecipeCategories.Any(c => c.RecipeCategoryId == categoryId)) {
            ViewData["EditRecipeMessage"] = "Invalid category";
            return View();
        }

        var username = HttpContext.Session.GetString("username");
        var userId = _db.Users.FirstOrDefault(user => user.Username == username)?.UserId;

        if (userId == null) {
            ViewData["EditRecipeMessage"] = "User not found";
            return View();
        }

        recipe.Title = form["title"];
        recipe.UserId = userId.Value;
        recipe.userName = username;
        recipe.Description = form["description"];
        recipe.Ingridients = form["ingridients"];
        recipe.Instructions = form["instructions"];
        recipe.Category = categoryId;

        _db.Recipes.Update(recipe);
        _db.SaveChanges();

        ViewData["EditRecipeMessage"] = "Recipe edited successfully";
        return RedirectToAction("ListMyRecipes");
    } catch (Exception ex) {
        ViewData["EditRecipeMessage"] = "Invalid field format: " + ex.Message;
        return View();
    }
}
    }
    
    }
