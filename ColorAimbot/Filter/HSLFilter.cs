using AForge.Imaging;

namespace ColorAimbot.Filter
{
    internal struct HSLFilter : IFilter
    {
        public HSL minHSL;
        public HSL maxHSL;

        public HSLFilter(HSL minHSL, HSL maxHSL)
        {
            this.minHSL = minHSL;
            this.maxHSL = maxHSL;
        }

        public bool FilterPixel(byte r, byte g, byte b)
        {
            HSL pixel = HSL.FromRGB(new RGB(r, g, b));

            if (pixel.Hue >= minHSL.Hue && pixel.Hue <= maxHSL.Hue &&
                pixel.Saturation >= minHSL.Saturation && pixel.Saturation <= maxHSL.Saturation &&
                pixel.Luminance >= minHSL.Luminance && pixel.Luminance <= maxHSL.Luminance)
                return true;

                return false;
        }
    }
}
