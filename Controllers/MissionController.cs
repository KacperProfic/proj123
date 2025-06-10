using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_zaliczenie.Data;
using Projekt_zaliczenie.Models;

namespace Projekt_zaliczenie.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MissionController : Controller
{
    private readonly MissionContext _context;

    public MissionController(MissionContext context)
    {
        _context = context;
    }

    // GET: api/mission (for all users)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mission>>> GetMissions()
    {
        return await _context.Missions.ToListAsync();
    }

    // GET: api/mission/5 
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Mission>> GetMission(long id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return NotFound();
        return mission;
    }

    // POST: api/mission 
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Mission>> CreateMission(Mission mission)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMission), new { id = mission.Id }, mission);
    }

    // PUT: api/mission/5 
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateMission(int id, Mission mission)
    {
        if (id != mission.Id) return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        _context.Entry(mission).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/mission/5 
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteMission(long id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return NotFound();
        _context.Missions.Remove(mission);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedMissions(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = null,
        string? sortDir = "asc",
        string? company = null,
        string? missionName = null,
        int? missionStatusId = null)
    {
        var query = _context.Missions.AsQueryable();
        if (!string.IsNullOrEmpty(company))
            query = query.Where(m => m.Company.Contains(company));
        if (!string.IsNullOrEmpty(missionName))
            query = query.Where(m => m.MissionName.Contains(missionName));
        if (missionStatusId.HasValue)
            query = query.Where(m => m.MissionStatusId == missionStatusId.Value);

        // Sortowanie
        if (!string.IsNullOrEmpty(sortBy))
        {
            if (sortBy.ToLower() == "launchdate")
                query = sortDir == "desc" ? query.OrderByDescending(m => m.LaunchDate) : query.OrderBy(m => m.LaunchDate);
            else if (sortBy.ToLower() == "missionname")
                query = sortDir == "desc" ? query.OrderByDescending(m => m.MissionName) : query.OrderBy(m => m.MissionName);
            else if (sortBy.ToLower() == "company")
                query = sortDir == "desc" ? query.OrderByDescending(m => m.Company) : query.OrderBy(m => m.Company);
            // Dodaj inne sortowania jeśli chcesz
        }
        else
        {
            query = query.OrderBy(m => m.Id);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        string baseUrl = $"/api/Mission/paged?pageSize={pageSize}";
        if (!string.IsNullOrEmpty(sortBy)) baseUrl += $"&sortBy={sortBy}";
        if (!string.IsNullOrEmpty(sortDir)) baseUrl += $"&sortDir={sortDir}";
        if (!string.IsNullOrEmpty(company)) baseUrl += $"&company={company}";
        if (!string.IsNullOrEmpty(missionName)) baseUrl += $"&missionName={missionName}";
        if (missionStatusId.HasValue) baseUrl += $"&missionStatusId={missionStatusId}";

        var links = new
        {
            first = totalPages > 0 ? $"{baseUrl}&pageNumber=1" : null,
            prev = pageNumber > 1 ? $"{baseUrl}&pageNumber={pageNumber - 1}" : null,
            next = pageNumber < totalPages ? $"{baseUrl}&pageNumber={pageNumber + 1}" : null,
            last = totalPages > 0 ? $"{baseUrl}&pageNumber={totalPages}" : null
        };

        return Ok(new
        {
            items,
            pageNumber,
            pageSize,
            totalPages,
            totalCount,
            links
        });
    }
}