namespace NewRelic.DotNetSDK.Processors
{
    /// <summary>
    /// A general purpose interface for processing metric values in <see cref="Agent" />
    /// </summary>
    public interface IProcessor
    {
        //// ----------------------------------------------------------------------------------------------------------
		
        /// <summary>
        /// Process a numeric value for metric reporting.
        /// </summary>
        /// <param name="value">The value to be processed</param>
        /// <returns>The processed value</returns>
        float Process(float value);

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}
