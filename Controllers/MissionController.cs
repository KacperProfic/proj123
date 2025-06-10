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

    // GET: api/mission/5 (requires login)
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Mission>> GetMission(long id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return NotFound();
        return mission;
    }

    // POST: api/mission (requires login)
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

    // PUT: api/mission/5 (requires login)
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

    // DELETE: api/mission/5 (requires login)
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

    // GET: api/mission/filter?status=1 (requires login)
    [HttpGet("filter")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Mission>>> FilterMissions([FromQuery(Name = "missionStatusId")] int missionStatusId)
    {
        return await _context.Missions.Where(m => m.MissionStatusId == missionStatusId).ToListAsync();
    }
}