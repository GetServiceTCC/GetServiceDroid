using Android.Content;
using Android.Graphics;
using Android.Util;
using static Android.Content.Res.Resources;

namespace GetServiceDroid.Utils
{
    public static class ThemeUtils
    {
        public static int GetThemeColor(Context context, int resAttr)
        {
            TypedValue typedValue = new TypedValue();
            Theme theme = context.Theme;
            theme.ResolveAttribute(resAttr, typedValue, true);
            return typedValue.Data;
        }

        public static Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }
    }
}