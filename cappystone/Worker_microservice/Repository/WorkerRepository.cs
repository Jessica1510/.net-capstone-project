using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Worker_microservice.Data;
using Worker_microservice.Models;
using Worker_microservice.Models.DTO;
using Worker_microservice.Repository;

public class WorkerRepository : IWorkerRepository
{
    private readonly WorkerDBContext _workerDBContext;
    private readonly IMapper _mapper;

    public WorkerRepository(WorkerDBContext dbContext, IMapper mapper)
    {
        _workerDBContext = dbContext;
        _mapper = mapper;
    }

    public async Task<int> CreateWorkerAsync(Create newWorker)
    {
        var entity = _mapper.Map<Worker>(newWorker);
        await _workerDBContext.Workers.AddAsync(entity);
        return await _workerDBContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Worker>> GetAllWorkerAsync()
    {
        return await _workerDBContext.Workers.AsNoTracking().ToListAsync();
    }

    public async Task<Worker?> GetWorkerAsync(int id)
    {
        return await _workerDBContext.Workers.FindAsync(id);
    }

    public async Task<int> UpdateWorkerAsync(int id, Worker updated)
    {
        var existing = await _workerDBContext.Workers.FindAsync(id);
        if (existing is null) return 0;

        // Map updated fields (or use AutoMapper)
        existing.FullName = updated.FullName ?? existing.FullName;
        existing.Email = updated.Email ?? existing.Email;
        existing.Phone = updated.Phone ?? existing.Phone;
        
        existing.SkillLevel = updated.SkillLevel ?? existing.SkillLevel;

        _workerDBContext.Workers.Update(existing);
        return await _workerDBContext.SaveChangesAsync();
    }

    public async Task<int> DeleteWorkerAsync(int id)
    {
        var worker = await _workerDBContext.Workers.FindAsync(id);
        if (worker is null) return 0;
        _workerDBContext.Workers.Remove(worker);
        return await _workerDBContext.SaveChangesAsync();
    }

    public async Task<Worker?> GetWorkerByUserIdAsync(string userId)
    {
        return await _workerDBContext.Workers
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

}
