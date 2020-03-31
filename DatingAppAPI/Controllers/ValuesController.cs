using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ValuesController : ControllerBase
  {
    private readonly DataContext _context;
    public ValuesController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetValues()
    {
      var values = await _context.Values.ToListAsync();
      return Ok(values);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetValue(int id)
    {
      var value = await _context.Values.FirstOrDefaultAsync(v => v.Id == id);

      return Ok(value);
    }
  }
}