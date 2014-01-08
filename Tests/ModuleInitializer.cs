using System.Diagnostics;
using System.Linq;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        ////foreach (var listener in Debug.Listeners.Cast<TraceListener>().ToList())
        ////{
        ////    if (listener is DefaultTraceListener)
        ////    {
        ////        Debug.Listeners.Remove(listener);
        ////    }
        ////}
    }
}