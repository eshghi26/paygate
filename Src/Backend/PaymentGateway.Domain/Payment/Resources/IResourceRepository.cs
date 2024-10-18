namespace PaymentGateway.Domain.Payment.Resources
{
    public interface IResourceRepository
    {
        Task<Resource?> GetById(long id);
        Task<Resource?> GetByName(string name);
        Task<int> Insert(Resource entity);
        Task<bool> Update(Resource entity);
        Task<bool> Delete(long id);
    }
}
