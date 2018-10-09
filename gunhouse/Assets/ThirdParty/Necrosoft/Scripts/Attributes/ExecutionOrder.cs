namespace Necrosoft
{
    /// <summary>
    /// Explicitly delcare execution order.
    /// NOTE: The value will not be automatially
    /// reset if this Attribute is removed.
    /// There is no way to know which script was
    /// previously set manually or explicitly.
    /// </summary>
    public class ExecutionOrder : System.Attribute
    {
        public int executionOrder;

        /// <summary>
        /// Explicitly delcare execution order.
        /// NOTE: The value will not be automatially
        /// reset if this Attribute is removed.
        /// There is no way to know which script was
        /// previously set manually or explicitly.
        /// </summary>
        public ExecutionOrder(int executionOrder) { this.executionOrder = executionOrder; }
    }
}