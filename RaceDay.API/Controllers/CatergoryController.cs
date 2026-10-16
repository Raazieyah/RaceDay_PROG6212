using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Contracts;
using RaceDay.Models;

namespace RaceDay.API.Controllers; // NEED: get,post,put,delete

[ApiController] //catergory enrolment status model
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly RaceDayDBContext _context;

    public CategoryController(RaceDayDBContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()

    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        return int.TryParse(userIdStr, out var id) ? id : null;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }

    // POST api
    [HttpPost]

    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest dto)
    {
        
        var role = HttpContext.Session.GetString("Role");
        if (role != "Organiser") return Forbid();

        var category = new Category
        {
            CategoryType = dto.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest dto)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Organiser") return Forbid();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id); // 3️⃣ FIXED: Primary Key name
        if (category == null) return NotFound("Category not found.");

        category.CategoryType = dto.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category updated successfully." });
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Organiser") return Forbid();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category == null) return NotFound("Category not found.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category deleted successfully." });
    }
}

