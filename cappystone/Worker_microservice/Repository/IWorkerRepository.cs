using Worker_microservice.Models.DTO;
using Worker_microservice.Models;

namespace Worker_microservice.Repository

{
    public interface IWorkerRepository
    {
        Task<int> CreateWorkerAsync(Create newWorker);
        Task<IEnumerable<Worker>> GetAllWorkerAsync();
        Task<int> DeleteWorkerAsync(int id);
        Task<Worker> GetWorkerAsync(int id);
        Task<int> UpdateWorkerAsync(int id, Worker worker);
        Task<Worker?> GetWorkerByUserIdAsync(string userId);


    }
}
