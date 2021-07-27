using UnityEngine;

namespace InfoVisView
{
    public static class Style
    {
        private static Gradient initColorMap()
        {
            var gradient = new Gradient();

            var colorKey = new[]
            {
                new GradientColorKey(new Color(232 / 255f, 49 / 255f, 81 / 255f), 0),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(new Color(2 / 255f, 195 / 255f, 154 / 255f), 1),
            };

            gradient.SetKeys(colorKey, new[] {new GradientAlphaKey(1, 0)});
            return gradient;
        }

        public static Color selectionColor = Color.red;

        public static Gradient ColorMap = initColorMap();
    }
}