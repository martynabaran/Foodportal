using Microsoft.EntityFrameworkCore;
using Foodbook.Models;

namespace Foodbook.Data;

public class FoodbookDBContext : DbContext {
    public FoodbookDBContext(DbContextOptions<FoodbookDBContext> options): base(options) {
        Database.EnsureCreated();

        Users = Set<UserModel>() as DbSet<UserModel>;
        Recipes = Set<RecipeModel>() as DbSet<RecipeModel>;
        //Authors = Set<authorModel>() as DbSet<authorModel>;
        RecipeCategories = Set<RecipeCategoryModel>() as DbSet<RecipeCategoryModel>;
        MealPlans = Set<MealPlanModel>() as DbSet<MealPlanModel>;
        MealPlanItems = Set<MealPlanItemModel>() as DbSet<MealPlanItemModel>;
        Ratings = Set<RatingModel>() as DbSet<RatingModel>;
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<RecipeModel> Recipes { get; set; }
    //public DbSet<authorModel> Authors {get; set;}
    public DbSet<RecipeCategoryModel> RecipeCategories { get; set; }
    public DbSet<MealPlanModel> MealPlans{ get; set; }
    public DbSet<MealPlanItemModel> MealPlanItems { get; set; }
    public DbSet<RatingModel> Ratings { get; set; }
}