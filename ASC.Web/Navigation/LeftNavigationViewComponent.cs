using Microsoft.AspNetCore.Mvc;
using ASC.Web.Models;
using System.Threading.Tasks;
using ASC.Web.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using ASC.Utilities;

namespace ASC.Web.Navigation
{
    public class LeftNavigationViewComponent : ViewComponent
    {
        private readonly INavigationCacheOperations _navigationCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeftNavigationViewComponent(INavigationCacheOperations navigationCache, IHttpContextAccessor httpContextAccessor)
        {
            _navigationCache = navigationCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var navigationMenu = await _navigationCache.GetNavigationCacheAsync();
            var currentUser = _httpContextAccessor.HttpContext.User.GetCurrentUserDetails();

            navigationMenu.MenuItems = navigationMenu.MenuItems.OrderBy(p => p.Sequence).ToList();
            navigationMenu.MenuItems = FilterMenuItemsByUserRoles(navigationMenu.MenuItems, currentUser.Roles);

            return View(navigationMenu);
        }

        private List<NavigationMenuItem> FilterMenuItemsByUserRoles(List<NavigationMenuItem> menuItems, string[] userRoles)
        {
            return menuItems.Where(item => item.UserRoles.Any(role => userRoles.Contains(role)))
                            .Select(item => new NavigationMenuItem
                            {
                                DisplayName = item.DisplayName,
                                MaterialIcon = item.MaterialIcon,
                                Link = item.Link,
                                IsNested = item.IsNested,
                                Sequence = item.Sequence,
                                UserRoles = item.UserRoles,
                                NestedItems = item.IsNested ? FilterMenuItemsByUserRoles(item.NestedItems, userRoles) : new List<NavigationMenuItem>()
                            })
                            .ToList();
        }
    }
}