using Idam.EFTimestamps.Extensions;
using Idam.EFTimestamps.Sample.Context;
using Idam.EFTimestamps.Sample.Models.Dto;
using Idam.EFTimestamps.Sample.Models.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DtsController(MyDbContext context) : ControllerBase
    {
        // GET: api/Dts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dt>>> GetDts()
        {
            return await context.Dts.ToListAsync();
        }

        // GET: api/Dts/1
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(NotFound))]
        public async Task<ActionResult<Dt>> GetDt(int id)
        {
            var dt = await context.Dts.FindAsync(id);

            if (dt is null)
            {
                return NotFound();
            }

            return dt;
        }

        // PUT: api/Dts/1
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDt(int id, DtUpdateDto dtDto)
        {
            if (id != dtDto.Id)
            {
                return BadRequest();
            }

            var dt = new Dt(dtDto);

            context.Entry(dt).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DtExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Dts
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType<Dt>(StatusCodes.Status201Created)]
        public async Task<ActionResult<Dt>> PostDt(DtCreateDto dtDto)
        {
            var dt = new Dt(dtDto);

            context.Dts.Add(dt);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDt), new { id = dt.Id }, dt);
        }

        // DELETE: api/Dts/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDt(int id)
        {
            var dt = await context.Dts.IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id.Equals(id));

            if (dt is null)
            {
                return NotFound();
            }

            context.Dts.Remove(dt);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Dts/1/force
        [HttpDelete("{id}/force")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForceDeleteDt(int id)
        {
            var dt = await context.Dts.IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id.Equals(id));

            if (dt is null)
            {
                return NotFound();
            }

            context.Dts.ForceRemove(dt);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Dts/deleted
        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<Dt>>> GetDeletedDts()
        {
            return await context.Dts
                .IgnoreQueryFilters()
                .Where(w => w.DeletedAt != null)
                .ToListAsync();
        }

        // GET: api/Dts/deleted/1
        [HttpGet("deleted/{id}")]
        public async Task<ActionResult<Dt>> GetDeletedDt(int id)
        {
            var foo = await context.Dts
                .IgnoreQueryFilters()
                .Where(w => w.Id == id)
                .Where(w => w.DeletedAt != null)
                .FirstOrDefaultAsync();

            if (foo is null)
            {
                return NotFound();
            }

            return foo;
        }

        // PUT: api/Dts/restore/1
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreDt(int id)
        {
            var entity = await context.Dts
                .IgnoreQueryFilters()
                .Where(w => w.Id == id)
                .Where(w => w.DeletedAt != null)
                .FirstOrDefaultAsync();
            if (entity is null)
            {
                return NotFound();
            }

            context.Dts.Restore(entity);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool DtExists(int id) =>
            (context.Dts?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
