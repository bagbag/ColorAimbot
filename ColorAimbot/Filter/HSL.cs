using System;
using AForge.Imaging;

namespace ColorAimbot.Filter
{
    public class HSL
    {
        /// <summary>
        /// Hue component.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Hue is measured in the range of [0, 1].
        /// </remarks>
        public double Hue;
        /// <summary>
        /// Saturation component.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Saturation is measured in the range of [0, 1].
        /// </remarks>
        public double Saturation;
        /// <summary>
        /// Luminance value.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Luminance is measured in the range of [0, 1].
        /// </remarks>
        public double Luminance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AForge.Imaging.HSL"/> class.
        /// 
        /// </summary>
        public HSL()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AForge.Imaging.HSL"/> class.
        /// 
        /// </summary>
        /// <param name="hue">Hue component.</param><param name="saturation">Saturation component.</param><param name="luminance">Luminance component.</param>
        public HSL(double hue, double saturation, double luminance)
        {
            Hue = hue;
            Saturation = saturation;
            Luminance = luminance;
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// 
        /// </summary>
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param><param name="hsl">Destination color in <b>HSL</b> color space.</param>
        /// <remarks>
        /// 
        /// <para>
        /// See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
        ///             for information about the algorithm to convert from RGB to HSL.
        /// </para>
        /// 
        /// </remarks>
        public static void FromRGB(RGB rgb, HSL hsl)
        {
            double val1 = rgb.Red / (double)byte.MaxValue;
            double val2 = rgb.Green / (double)byte.MaxValue;
            double val3 = rgb.Blue / (double)byte.MaxValue;
            double num1 = Math.Min(Math.Min(val1, val2), val3);
            double num2 = Math.Max(Math.Max(val1, val2), val3);
            double num3 = num2 - num1;
            hsl.Luminance = (num2 + num1) / 2.0;
            if (num3 == 0.0)
            {
                hsl.Hue = 0;
                hsl.Saturation = 0.0f;
            }
            else
            {
                hsl.Saturation = hsl.Luminance <= 0.5 ? num3 / (num2 + num1) : num3 / (2f - num2 - num1);
                double num4 = val1 != num2 ? (val2 != num2 ? 0.666666686534882 + (val1 - val2) / 6.0 / num3 : 0.333333343267441 + (val3 - val1) / 6.0 / num3) : (val2 - val3) / 6.0 / num3;
                if (num4 < 0.0)
                    ++num4;
                if (num4 > 1.0)
                    --num4;
                hsl.Hue = num4;
            }
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// 
        /// </summary>
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <returns>
        /// Returns <see cref="T:AForge.Imaging.HSL"/> instance, which represents converted color value.
        /// </returns>
        public static HSL FromRGB(RGB rgb)
        {
            HSL hsl = new HSL();
            FromRGB(rgb, hsl);
            return hsl;
        }

        /// <summary>
        /// Convert from HSL to RGB color space.
        /// 
        /// </summary>
        /// <param name="hsl">Source color in <b>HSL</b> color space.</param><param name="rgb">Destination color in <b>RGB</b> color space.</param>
        public static void ToRGB(HSL hsl, RGB rgb)
        {
            if (hsl.Saturation == 0.0)
            {
                rgb.Red = rgb.Green = rgb.Blue = (byte)(hsl.Luminance * byte.MaxValue);
            }
            else
            {
                double vH = hsl.Hue;
                double v2 = hsl.Luminance < 0.5 ? hsl.Luminance * (1f + hsl.Saturation) : hsl.Luminance + hsl.Saturation - hsl.Luminance * hsl.Saturation;
                double v1 = 2f * hsl.Luminance - v2;
                rgb.Red = (byte)(byte.MaxValue * Hue2RGB(v1, v2, vH + 0.3333333f));
                rgb.Green = (byte)(byte.MaxValue * Hue2RGB(v1, v2, vH));
                rgb.Blue = (byte)(byte.MaxValue * Hue2RGB(v1, v2, vH - 0.3333333f));
            }
            rgb.Alpha = byte.MaxValue;
        }

        /// <summary>
        /// Convert the color to <b>RGB</b> color space.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:AForge.Imaging.RGB"/> instance, which represents converted color value.
        /// </returns>
        public RGB ToRGB()
        {
            var rgb = new RGB();
            ToRGB(this, rgb);
            return rgb;
        }

        private static double Hue2RGB(double v1, double v2, double vH)
        {
            if (vH < 0.0)
                ++vH;
            if (vH > 1.0)
                --vH;
            if (6.0 * vH < 1.0)
                return v1 + (v2 - v1) * 6.0 * vH;
            if (2.0 * vH < 1.0)
                return v2;
            if (3.0 * vH < 2.0)
                return v1 + (v2 - v1) * (0.666666686534882 - vH) * 6.0;

            return v1;
        }
    }
}
