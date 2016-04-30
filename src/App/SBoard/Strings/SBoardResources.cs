using Windows.ApplicationModel.Resources;
using UwCore.Common;

namespace SBoard.Strings
{
    public static class SBoardResources
    {
        private static readonly ResourceAccessor Accessor = new ResourceAccessor(ResourceLoader.GetForViewIndependentUse("Resources"));
        
        public static string Get(string resource)
        {
            return Accessor.Get(resource);
        }

        public static string GetFormatted(string resource, params object[] arguments)
        {
            return Accessor.GetFormatted(resource, arguments);
        }
    }
}
