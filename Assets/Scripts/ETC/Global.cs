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

        // ��𿡼��� ����� �� �ֵ��� public���� ����
        public static Color ChangeColor(string hexColor)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
            {
                return newColor;
            }
            // �⺻ ������ ������� �����ϰų� �ʿ信 ���� ����
            return Color.white;
        }
    }
}
