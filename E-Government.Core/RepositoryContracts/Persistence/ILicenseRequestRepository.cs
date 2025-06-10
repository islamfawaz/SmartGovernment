namespace E_Government.Domain.RepositoryContracts.Persistence
{
    public interface ILicenseRequestRepository
    {
        Task<IEnumerable<ILicenseRequest>> GetAllAsync();
    }
}
