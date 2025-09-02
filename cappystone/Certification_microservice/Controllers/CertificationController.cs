using AutoMapper;
using Certification_microservice.Data;
using Certification_microservice.DTO;
using Certification_microservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class CertificationController : ControllerBase
{
    private readonly CertificationDbContext _context;
    private readonly IMapper _mapper;

    public CertificationController(CertificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<CertificationDTO>>> GetAll()
    {
        var certs = await _context.Certifications.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<CertificationDTO>>(certs));
    }

    [HttpGet("byWorker")]
    public async Task<ActionResult<IEnumerable<CertificationDTO>>> GetByWorkerId([FromQuery] string workerId)
    {
        var certs = await _context.Certifications
            .Where(c => c.WorkerId == workerId)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<CertificationDTO>>(certs));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CertificationDTO>> GetById(int id)
    {
        var cert = await _context.Certifications.FindAsync(id);
        if (cert == null) return NotFound();
        return Ok(_mapper.Map<CertificationDTO>(cert));
    }

    [HttpPost]
    public async Task<ActionResult<CertificationDTO>> Create(CertificationDTO dto)
    {
        var cert = _mapper.Map<Certification>(dto);
        _context.Certifications.Add(cert);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cert.Id }, _mapper.Map<CertificationDTO>(cert));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CertificationDTO dto)
    {
        if (id != dto.Id) return BadRequest();
        var cert = _mapper.Map<Certification>(dto);
        _context.Entry(cert).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cert = await _context.Certifications.FindAsync(id);
        if (cert == null) return NotFound();
        _context.Certifications.Remove(cert);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ✅ Secure endpoint for logged-in worker
    [Authorize(Roles = "Worker")]
    [HttpPost("mine")]
    public async Task<ActionResult<CertificationDTO>> CreateForLoggedInWorker(CertificationDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        dto.WorkerId = userId; // ✅ assign it here

        var cert = _mapper.Map<Certification>(dto);
        _context.Certifications.Add(cert);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = cert.Id }, _mapper.Map<CertificationDTO>(cert));
    }

    [Authorize(Roles = "Worker")]
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<CertificationDTO>>> GetMyCerts()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var certs = await _context.Certifications
            .Where(c => c.WorkerId == userId)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<CertificationDTO>>(certs));
    }

}
