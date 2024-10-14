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
public class DtUtcsController(MyDbContext context) : ControllerBase
{
    // GET: api/DtUtcs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DtUtc>>> GetDtUtcs()
    {
        return await context.DtUtcs.ToListAsync();
    }

    // GET: api/DtUtcs/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(NotFound))]
    public async Task<ActionResult<DtUtc>> GetDtUtc(int id)
    {
        var dtUtc = await context.DtUtcs.FindAsync(id);

        if (dtUtc is null) return NotFound();

        return dtUtc;
    }

    // PUT: api/DtUtcs/1
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutDtUtc(int id, DtUtcUpdateDto dtUtcDto)
    {
        if (id != dtUtcDto.Id) return BadRequest();

        var dtUtc = new DtUtc(dtUtcDto);

        context.Entry(dtUtc).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DtUtcExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/DtUtcs
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType<Dt>(StatusCodes.Status201Created)]
    public async Task<ActionResult<Dt>> PostDtUtc(DtUtcCreateDto dtUtcDto)
    {
        var dtUtc = new DtUtc(dtUtcDto);

        context.DtUtcs.Add(dtUtc);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDtUtc), new { id = dtUtc.Id }, dtUtc);
    }

    // DELETE: api/DtUtcs/1
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDtUtc(int id)
    {
        var dtUtc = await context.DtUtcs.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (dtUtc is null) return NotFound();

        context.DtUtcs.Remove(dtUtc);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DtUtcs/1/force
    [HttpDelete("{id:int}/force")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForceDeleteDtUtc(int id)
    {
        var dtUtc = await context.DtUtcs.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (dtUtc is null) return NotFound();

        context.DtUtcs.ForceRemove(dtUtc);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/DtUtcs/deleted
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<DtUtc>>> GetDeletedDtUtcs()
    {
        return await context.DtUtcs
            .IgnoreQueryFilters()
            .Where(w => w.DeletedAt != null)
            .ToListAsync();
    }

    // GET: api/DtUtcs/deleted/1
    [HttpGet("deleted/{id:int}")]
    public async Task<ActionResult<DtUtc>> GetDeletedDtUtc(int id)
    {
        var dtUtc = await context.DtUtcs
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();

        if (dtUtc is null) return NotFound();

        return dtUtc;
    }

    // PUT: api/DtUtcs/restore/1
    [HttpPut("restore/{id:int}")]
    public async Task<IActionResult> RestoreDtUtc(int id)
    {
        var entity = await context.DtUtcs
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();
        if (entity is null) return NotFound();

        context.DtUtcs.Restore(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool DtUtcExists(int id)
    {
        return (context.DtUtcs?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}