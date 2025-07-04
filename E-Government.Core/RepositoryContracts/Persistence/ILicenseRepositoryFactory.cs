namespace E_Government.Domain.RepositoryContracts.Persistence
{
   public interface ILicenseRepositoryFactory
    {
        ILicenseRequestRepository GetRepository(string requestType);

    }
}
