using Idam.EFTimestamps.Extensions;
using Idam.EFTimestamps.Sample.Context;
using Idam.EFTimestamps.Sample.Models.Dto;
using Idam.EFTimestamps.Sample.Models.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Sample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UnixsController(MyDbContext context) : ControllerBase
{
    // GET: api/Unixs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Unix>>> GetUnixs()
    {
        return await context.Unixs.ToListAsync();
    }

    // GET: api/Unixs/1
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(NotFound))]
    public async Task<ActionResult<Unix>> GetUnix(int id)
    {
        var unix = await context.Unixs.FindAsync(id);

        if (unix is null) return NotFound();

        return unix;
    }

    // PUT: api/Unixs/1
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutUnix(int id, UnixUpdateDto unixDto)
    {
        if (id != unixDto.Id) return BadRequest();

        context.Entry(unixDto).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UnixExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Unixs
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType<Unix>(StatusCodes.Status201Created)]
    public async Task<ActionResult<Unix>> PostUnix(UnixCreateDto unixDto)
    {
        var unix = new Unix(unixDto);

        context.Unixs.Add(unix);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnix), new { id = unix.Id }, unix);
    }

    // DELETE: api/Unixs/1
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUnix(int id, [FromQuery] bool permanent = false)
    {
        var unix = await context.Unixs.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (unix is null || (unix.DeletedAt is not null && permanent.Equals(false))) return NotFound();

        context.Unixs.Remove(unix);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Unixs/1/force
    [HttpDelete("{id}/force")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForceDeleteDt(int id)
    {
        var unix = await context.Unixs.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (unix is null) return NotFound();

        context.Unixs.ForceRemove(unix);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Unixs/deleted
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<Unix>>> GetDeletedUnixs()
    {
        return await context.Unixs
            .IgnoreQueryFilters()
            .Where(w => w.DeletedAt != null)
            .ToListAsync();
    }

    // GET: api/Unixs/deleted/1
    [HttpGet("deleted/{id}")]
    public async Task<ActionResult<Unix>> GetDeletedUnix(int id)
    {
        var unix = await context.Unixs
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();

        if (unix is null) return NotFound();

        return unix;
    }

    // PUT: api/Unixs/restore/1
    [HttpPut("restore/{id}")]
    public async Task<IActionResult> RestoreUnix(int id)
    {
        var entity = await context.Unixs
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();

        if (entity is null) return NotFound();

        context.Unixs.Restore(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool UnixExists(int id)
    {
        return context.Unixs.Any(e => e.Id == id);
    }
}