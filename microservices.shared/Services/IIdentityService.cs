
namespace microservices.shared.Services
{
    public interface IIdentityService
    {
        public Guid GetUserId { get; }
        public string UserName { get; }
    }
}
