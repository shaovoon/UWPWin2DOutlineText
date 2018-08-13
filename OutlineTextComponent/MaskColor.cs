﻿using Windows.UI;

namespace OutlineTextComponent
{
    /// <summary>
    /// Simple helper class to use mask color
    /// </summary>
    public sealed class MaskColor
    {
        /// <summary>
        /// Method to return a primary red color to be used as mask
        /// </summary>
        public static Color Red
        {
            get
            {
                return Color.FromArgb(0xFF, 0xFF, 0, 0);
            }
        }
        /// <summary>
        /// Method to return a primary green color to be used as mask
        /// </summary>
        public static Color Green
        {
            get
            {
                return Color.FromArgb(0xFF, 0, 0xFF, 0);
            }
        }
        /// <summary>
        /// Method to return a primary blue color to be used as mask
        /// </summary>
        public static Color Blue
        {
            get
            {
                return Color.FromArgb(0xFF, 0, 0, 0xFF);
            }
        }
        /// <summary>
        /// Method to compare 2 GDI+ color
        /// </summary>
        /// <param name="clr1">is 1st color</param>
        /// <param name="clr2">is 2nd color</param>
        /// <returns>true if equal</returns>
        public static bool IsEqual(Color clr1, Color clr2)
        {
            if (clr1.R == clr2.R && clr1.G == clr2.G && clr1.B == clr2.B)
                return true;

            return false;
        }
    }
}
