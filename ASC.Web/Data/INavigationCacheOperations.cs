using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ASC.Web.Models;

namespace ASC.Web.Data
{
    public interface INavigationCacheOperations
    {
        Task<NavigationMenu> GetNavigationCacheAsync();
        Task CreateNavigationCacheAsync();
    }
}
