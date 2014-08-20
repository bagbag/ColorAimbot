namespace ColorAimbot.Filter
{
    public interface IFilter
    {
        /// <summary>
        /// returns true if color is in range
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns></returns>
        bool FilterPixel(byte r, byte g, byte b);
    }
}
