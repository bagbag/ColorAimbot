using AForge.Imaging;

namespace ColorAimbot.Filter
{
    public struct RGBFilter : IFilter
    {
        public RGB minRGB;
        public RGB maxRGB;

        public RGBFilter(RGB minRGB, RGB maxRGB)
        {
            this.minRGB = minRGB;
            this.maxRGB = maxRGB;
        }

        public bool FilterPixel(byte r, byte g, byte b)
        {
            if (r >= minRGB.Red && r <= maxRGB.Red && //red
                g >= minRGB.Green && g <= maxRGB.Green && //green
                b >= minRGB.Blue && b <= maxRGB.Blue)
                return true;

            return false;
        }
    }
}
