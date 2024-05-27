using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Foodbook.Models;
using Foodbook.Data;
using System.Collections.Generic;
using System.Globalization;

namespace Foodbook.Controllers
{
    [Route("[controller]/")]
    public class MealPlanController : Controller
    {
        private readonly FoodbookDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MealPlanController(FoodbookDBContext contextUser, IHttpContextAccessor httpContextAccessor)
        {
            _db = contextUser;
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetViewDataFromSession()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                ViewData["Username"] = "";
                ViewData["IsAdmin"] = "";
                return;
            }

            ViewData["Username"] = HttpContext.Session.GetString("username");
            ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
        }

         [HttpGet]
        [Route("mealplans")]
        public IActionResult MealPlans()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "User");
            }
            SetViewDataFromSession();

            int userId = _db.Users
                .Where(u => u.Username == HttpContext.Session.GetString("username"))
                .Select(u => u.UserId)
                .FirstOrDefault();

            var mealPlans = _db.MealPlans
                .Where(mp => mp.UserId == userId)
                .ToList();

            return View(mealPlans);
        }

        
        [Route("mealplans/create")]
        public IActionResult CreateMealPlan()
        {
            if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }
            SetViewDataFromSession();

            // tu chyba musze cos dopisac
            return View();
        }

    [Route("mealplans/create")]
    [HttpPost]
    public IActionResult CreateMealPlan(IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        List<String> fields = new List<String> {
            "startDate",
        };

        foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["CreateMealPlan"] = "All fields are required, missing "+ field;

                return View();
            }
        }

        try {
            
            var id = _db.Users
    .Where(user => user.Username == HttpContext.Session.GetString("username"))
    .Select(user => user.UserId)
    .FirstOrDefault();
          
            
        if (DateTime.TryParseExact(form["startDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime weekStartDate))
        {
            _db.MealPlans.Add(new MealPlanModel
            {
                UserId = id,
                WeekStartDate = weekStartDate,
                // Inicjalizacja kolekcji MealPlanItems, aby uniknąć błędu kompilacji
                MealPlanItems = new List<MealPlanItemModel>()
            });

            _db.SaveChanges();

            ViewData["CreateMealPlan"] = "Plan created successfully";
            return RedirectToAction("MealPlans");
        }
        else
        {
            ViewData["CreateMealPlan"] = "Invalid date format";
            return View();
        }
        } catch (Exception) {
            ViewData["CreateMealPlan"] = "Invalid field format";

            return View();
        }
    }

[Route("mealplans/additemtomealplan/{mealPlanId:int}")]
public IActionResult AddItemtoMealPlan(int mealPlanId){
    if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        RecipeCategoryModel []recipeCategories = _db.RecipeCategories.ToArray();

        
        SetViewDataFromSession();
    RecipeModel []recipes = _db.Recipes.ToArray();
    MealPlanModel mealPlan = _db.MealPlans.FirstOrDefault(mealPlan => mealPlan.Id == mealPlanId);
    if(mealPlan is null){
        ViewData["AddItemToPlanMessage"] = "This meal plan does not exists";
        return View();
    }
    else {
        ViewData["RecipeCategories"] = recipeCategories;
        ViewData["MealPlan"] = mealPlan;
        ViewData["Recipes"] = recipes;
        return View();
    }
}
[Route("mealplans/additemtomealplan/{mealPlanId:int}")]
[HttpPost]
public IActionResult AddItemtoMealPlan(int mealPlanId,IFormCollection form ){
    if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
    {
        return RedirectToAction("Login", "User");
    }

    SetViewDataFromSession();

     List<String> fields = new List<String> {
            "recipe-id",
            "dayOfWeek",
            "recipe-category-id"
        };

        foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["AddItemToPlanMessage"] = "All fields are required, missing "+ field;

                return View();
            }
        }

        try {
            List<RecipeCategoryModel> category = (
                from recipeCategory in _db.RecipeCategories where recipeCategory.RecipeCategoryId == Int32.Parse((String ?) form["recipe-category-id"] ?? "") select recipeCategory
            ).ToList();

            if (category.Count() == 0) {
                ViewData["AddItemToPlanMessage"] = "Invalid category";

                return View();
            }
            var id = _db.Users
    .Where(user => user.Username == HttpContext.Session.GetString("username"))
    .Select(user => user.UserId)
    .FirstOrDefault();

            int categoryId = Int32.Parse(form["recipe-category-id"]);
             int recipeId = Int32.Parse(form["recipe-id"]);
        DayOfWeek dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), form["dayOfWeek"]);

        Console.WriteLine("Parsed values - recipeId: " + recipeId + ", dayOfWeek: " + dayOfWeek + ", categoryId: " + categoryId);
            _db.MealPlanItems.Add(new MealPlanItemModel {
                RecipeId = Int32.Parse((String ?) form["recipe-id"] ?? "0"),
                MealPlanId = mealPlanId,
                DayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), form["dayOfWeek"]),
                CategoryId = Int32.Parse((String ?) form["recipe-category-id"] ?? "0")
            });

            _db.SaveChanges();
             ViewData["AddItemToPlanMessage"] = "Item added successfully";

            return RedirectToAction("MealPlans"); 
        } catch (Exception) {
            ViewData["AddItemToPlanMessage"] = "Invalid field format";

            return View();
        }

       }
    [Route("mealplans/viewdetails/{mealPlanId:int}")]
public IActionResult ViewMealPlanDetails(int mealPlanId)
{


    if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

    MealPlanItemDisplayElement []mealPlanItems = (
        from mpitem in _db.MealPlanItems
         where mpitem.MealPlanId == mealPlanId
         join recipe in _db.Recipes on mpitem.RecipeId equals recipe.RecipeId
         select new MealPlanItemDisplayElement
        {
            RecipeId = recipe.RecipeId,
            Title = recipe.Title,
            Category = recipe.Category,
            Day = mpitem.DayOfWeek

        }
    ).ToArray();

    ViewData["MealPlanItems"] = mealPlanItems;
    return View();
   
}

    }
}
