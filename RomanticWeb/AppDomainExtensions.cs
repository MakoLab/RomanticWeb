namespace System
{
    /// <summary>Contains useful extension methods for AppDomain class.</summary>
    public static class AppDomainExtensions
    {
        /// <summary>Gets a primary path storing assemblies for given application domain.</summary>
        /// <remarks>This method shouldn't reutrn <b>null</b> in any case.</remarks>
        /// <param name="appDomain">Application domain for which the path is beeing detremined.</param>
        /// <returns>Primary place where assemblies for given application domain are stored.</returns>
        public static string GetPrimaryAssemblyPath(this AppDomain appDomain)
        {
            return (System.String.IsNullOrWhiteSpace(appDomain.RelativeSearchPath)?appDomain.BaseDirectory:appDomain.RelativeSearchPath);
        }

        /// <summary>Gets a primary path storing assemblies for given application domain.</summary>
        /// <remarks>This method shouldn't reutrn <b>null</b> in any case.</remarks>
        /// <param name="appDomain">Application domain for which the path is beeing detremined.</param>
        /// <returns>Primary place where assemblies for given application domain are stored.</returns>
        public static string GetApplicationStoragePath(this AppDomain appDomain)
        {
            return System.IO.Path.Combine(
                (System.String.IsNullOrWhiteSpace(appDomain.RelativeSearchPath)?appDomain.BaseDirectory:System.IO.Path.Combine(appDomain.RelativeSearchPath,"..")),
                "App_Data");
        }
    }
}