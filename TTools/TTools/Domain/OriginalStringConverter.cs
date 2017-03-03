using Smart.Text.Japanese;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TTools.Models
{
    public static class OriginalStringConverter
    {
        /// <summary>
        /// 検索用の文字列の整理コンバータ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SearchStringConvert(string str)
        {
            str = KanaConverter.Convert(str, KanaOption.HankanaToHiragana);
            str = KanaConverter.Convert(str, KanaOption.KatakanaToHiragana);
            str = KanaConverter.Convert(str, KanaOption.RomanToNarrow);
            str = KanaConverter.Convert(str, KanaOption.AsciiToNarrow);
            str = KanaConverter.Convert(str, KanaOption.NumericToNarrow);

            str = str.ToUpper();
            return str;
        }

        /// <summary>
        /// 標準表現へ変換する
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DefaultStringConvert(string str)
        {
            if (string.IsNullOrEmpty(str) == true || string.IsNullOrWhiteSpace(str) == true) return null;

            str = KanaConverter.Convert(str, KanaOption.AsciiToNarrow);
            str = KanaConverter.Convert(str, KanaOption.NumericToNarrow);
            str = KanaConverter.Convert(str, KanaOption.RomanToNarrow);
            str = KanaConverter.Convert(str, KanaOption.SpaceToNarrow);
            str = KanaConverter.Convert(str, KanaOption.KanaToWide);

            var items = str.Split(' ');
            string returnStr = "";

            foreach (var a in items)
            {
                if (a != "")
                {
                    returnStr = returnStr + " " + a;
                }
            }

            returnStr = returnStr.TrimStart();

            return returnStr;
        }
    }
}
