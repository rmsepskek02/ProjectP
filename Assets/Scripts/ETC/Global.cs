using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    public static class Colors
    {
        public static readonly string WhiteColor = "#FFFFFF";
        public static readonly string OpenColor = "#FFFFFF";
        public static readonly string SecretColor = "#616161";
        public static readonly string GreenColor = "#00FF00";
        public static readonly string BlueColor = "#0000FF";

        // 어디에서든 사용할 수 있도록 public으로 설정
        public static Color ChangeColor(string hexColor)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
            {
                return newColor;
            }
            // 기본 색상을 흰색으로 설정하거나 필요에 따라 변경
            return Color.white;
        }
    }
}
