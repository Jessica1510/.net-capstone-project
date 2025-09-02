using Microsoft.AspNetCore.Mvc;
using Claim_microservice.Models;
using Claim_microservice.Data;
using Microsoft.EntityFrameworkCore;

namespace Claim_microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ClaimDbContext _context;

        public ClaimsController(ClaimDbContext context)
        {
            _context = context;
        }

        // GET: api/claims
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaims()
        {
            return await _context.Claims.ToListAsync();
        }

        // GET: api/claims/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound();

            return claim;
        }
        [HttpGet("worker/{workerId}")]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaimsByWorker(string workerId)
        {
            var claims = await _context.Claims
                .Where(c => c.WorkerId == workerId)
                .ToListAsync();

            if (claims == null || claims.Count == 0)
                return NotFound();

            return claims;
        }


        // POST: api/claims
        [HttpPost]
        public async Task<ActionResult<Claim>> PostClaim(Claim claim)
        {
            claim.SubmittedOn = DateTime.UtcNow;
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, claim);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClaim(int id, Claim updatedClaim)
        {
            if (id != updatedClaim.Id)
                return BadRequest();

            var existingClaim = await _context.Claims.FindAsync(id);
            if (existingClaim == null)
                return NotFound();

            // Preserve submittedOn and update only editable fields
            existingClaim.CourseId = updatedClaim.CourseId;
            existingClaim.Amount = updatedClaim.Amount;
            existingClaim.Remarks = updatedClaim.Remarks;
            existingClaim.Status = updatedClaim.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Claims.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }


        // DELETE: api/claims/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
