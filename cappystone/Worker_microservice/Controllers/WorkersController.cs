using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Worker_microservice.Models;
using Worker_microservice.Models.DTO;
using Worker_microservice.Repository;

namespace Worker_microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;


        public WorkersController(IWorkerRepository workerRepository, IMemoryCache memoryCache, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _memoryCache = memoryCache;
            _mapper = mapper;

        }

        [Authorize(Roles = "Worker")]
        [HttpPost("createprofile")]
        public async Task<IActionResult> CreateMyProfile([FromBody] Create dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            dto.UserId = userId;
            var result = await _workerRepository.CreateWorkerAsync(dto);
            return result > 0 ? Ok("Created") : BadRequest();
        }
        /*
        [HttpPost]
        public async Task<ActionResult<string>> PostWorker(Create newworker)
        {
            if (newworker == null)
            {
                return BadRequest("Author is Null");
            }
            else
            {
                // Mannual Mapping For one object to another 

                //author.Name = newauthor.Name;
                //author.Email = newauthor.Email;

                //author.Country = newauthor.Country;
                var res = await _workerRepository.CreateWorkerAsync(newworker);
                if (res > 0)
                {
                    return Ok("Created Successfully");
                }
                else
                {
                    return BadRequest("Something Went Wrong");
                }

                


            }
        }*/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Worker>>> GetAllWorkers()
        {
            IEnumerable<Worker>? workersList = null;
            if (_memoryCache.TryGetValue("AllWorkers", out workersList))
            {
                return Ok(workersList);
            }
            else
            {
                workersList = await _workerRepository.GetAllWorkerAsync();
                _memoryCache.Set("AllWorkers", workersList, TimeSpan.FromMinutes(10));
                return Ok(workersList);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Worker>> GetWorker(int id)
        {
            var worker = await _workerRepository.GetWorkerAsync(id);
            return worker is null ? NotFound() : Ok(worker);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorker(int id, [FromBody] Update dto)
        {
            var worker = _mapper.Map<Worker>(dto);
            var result = await _workerRepository.UpdateWorkerAsync(id, worker);
            return result > 0 ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            var deleted = await _workerRepository.DeleteWorkerAsync(id);
            return deleted > 0 ? NoContent() : NotFound();
        }


        [Authorize(Roles ="Worker")]
        [HttpGet("me")]
        public async Task<ActionResult<Worker>> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var worker = await _workerRepository.GetWorkerByUserIdAsync(userId);
            return worker is null ? NotFound() : Ok(worker);
        }
    }

    }
