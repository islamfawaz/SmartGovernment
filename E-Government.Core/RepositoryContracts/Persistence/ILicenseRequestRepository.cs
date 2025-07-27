namespace E_Government.Domain.RepositoryContracts.Persistence
{
    public interface ILicenseRequestRepository
    {
        Task<IEnumerable<ILicenseRequest>> GetAllAsync();
        Task<ILicenseRequest?> GetByPublicIdAsync(Guid publicId); // ✅ لازم تبقى هنا كمان

    }
}
