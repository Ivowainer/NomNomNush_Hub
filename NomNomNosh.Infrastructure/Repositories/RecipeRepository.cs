using NomNomNosh.Application.DTOs;
using NomNomNosh.Application.Interfaces;
using NomNomNosh.Domain.Entities;
using NomNomNosh.Infrastructure.Data;
using NomNomNosh.Infrastructure.Utils;

namespace NomNomNosh.Infrastructure.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {

        private readonly AppDbContext _appDbContext;
        private readonly IUtils _utils;
        public RecipeRepository(AppDbContext appDbContext, IUtils utils)
        {
            _appDbContext = appDbContext;
            _utils = utils;
        }

        public async Task<RecipeDto> CreateRecipe(Guid member_id, Recipe recipe)
        {
            if (!_appDbContext.Members.Any(m => m.Member_Id == member_id))
                throw new InvalidOperationException("Member not found");

            recipe.Member_Id = member_id;
            recipe.Average_Rating = 0;
            recipe.Published_Date = DateTime.Now;

            await _appDbContext.Recipes.AddAsync(recipe);
            await _appDbContext.SaveChangesAsync();

            return new RecipeDto
            {
                Recipe_Id = recipe.Recipe_Id,
                Title = recipe.Title,
                Average_Rating = recipe.Average_Rating,
                Description = recipe.Description,
                Main_Image = recipe.Main_Image,
                Member_Id = recipe.Member_Id,
                Published_Date = recipe.Published_Date,
            };
        }


        public async Task<RecipeDto> UpdateRecipe(Guid recipe_id, Guid member_id, Recipe recipe)
        {
            var recipeToUpdate = await _utils.GetRecipeIfOwner(recipe_id, member_id);

            recipeToUpdate.Title = recipe.Title;
            recipeToUpdate.Description = recipe.Description;
            recipeToUpdate.Main_Image = recipe.Main_Image;

            await _appDbContext.SaveChangesAsync();

            return new RecipeDto
            {
                Title = recipeToUpdate.Title,
                Description = recipeToUpdate.Description,
                Main_Image = recipeToUpdate.Main_Image,
                Published_Date = recipe.Published_Date,
            };
        }

        public async Task<RecipeDto> DeleteRecipe(Guid recipe_id, Guid member_id)
        {
            var recipe = await _utils.GetRecipeIfOwner(recipe_id, member_id);

            _appDbContext.Recipes.Remove(recipe);
            await _appDbContext.SaveChangesAsync();

            return new RecipeDto
            {
                Recipe_Id = recipe.Recipe_Id,
                Title = recipe.Title,
                Average_Rating = recipe.Average_Rating,
                Description = recipe.Description,
                Main_Image = recipe.Main_Image,
                Member_Id = recipe.Member_Id,
                Published_Date = recipe.Published_Date,
            };
        }
    }
}