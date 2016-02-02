using ColorAimbot.Filter;

namespace ColorAimbot.Target
{
    public struct TargetDescriptor
    {
        public IFilter filter;
        public byte priority;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">the filter which filters out the target of the other parts of the picture</param>
        /// <param name="priority">the priority of the target, 1-255 (0 is internally used for background)</param>
        public TargetDescriptor(IFilter filter, byte priority)
        {
            this.filter = filter;
            this.priority = priority;
        }
    }
}
