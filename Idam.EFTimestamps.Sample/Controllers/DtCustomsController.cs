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
public class DtCustomsController(MyDbContext context) : ControllerBase
{
    // GET: api/DtCustoms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DtCustom>>> GetDtCustoms()
    {
        return await context.DtCustoms.ToListAsync();
    }

    // GET: api/DtCustoms/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(NotFound))]
    public async Task<ActionResult<DtCustom>> GetDtCustom(int id)
    {
        var dt = await context.DtCustoms.FindAsync(id);

        if (dt is null) return NotFound();

        return dt;
    }

    // PUT: api/DtCustoms/1
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutDtCustom(int id, DtCustomUpdatedDto dtCustomDto)
    {
        if (id != dtCustomDto.Id) return BadRequest();

        var dt = new DtCustom(dtCustomDto);

        context.Entry(dt).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DtCustomExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/DtCustoms
    // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType<Dt>(StatusCodes.Status201Created)]
    public async Task<ActionResult<DtCustom>> PostDtCustom(DtCustomCreatedDto dtCustomDto)
    {
        var dt = new DtCustom(dtCustomDto);

        context.DtCustoms.Add(dt);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDtCustom), new { id = dt.Id }, dt);
    }

    // DELETE: api/DtCustoms/1
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDtCustom(int id)
    {
        var dt = await context.DtCustoms.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (dt is null) return NotFound();

        context.DtCustoms.Remove(dt);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DtCustoms/1/force
    [HttpDelete("{id:int}/force")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForceDeleteDtCustom(int id)
    {
        var dt = await context.DtCustoms.IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id.Equals(id));

        if (dt is null) return NotFound();

        context.DtCustoms.ForceRemove(dt);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/DtCustoms/deleted
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<DtCustom>>> GetDeletedDtCustoms()
    {
        return await context.DtCustoms
            .IgnoreQueryFilters()
            .Where(w => w.DeletedAt != null)
            .ToListAsync();
    }

    // GET: api/DtCustoms/deleted/1
    [HttpGet("deleted/{id}")]
    public async Task<ActionResult<DtCustom>> GetDeletedDtCustom(int id)
    {
        var dt = await context.DtCustoms
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();

        if (dt is null) return NotFound();

        return dt;
    }

    // PUT: api/DtCustoms/restore/1
    [HttpPut("restore/{id}")]
    public async Task<IActionResult> RestoreDtCustom(int id)
    {
        var entity = await context.DtCustoms
            .IgnoreQueryFilters()
            .Where(w => w.Id == id)
            .Where(w => w.DeletedAt != null)
            .FirstOrDefaultAsync();
        if (entity is null) return NotFound();

        context.DtCustoms.Restore(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool DtCustomExists(int id)
    {
        return context.DtCustoms.Any(e => e.Id == id);
    }
}