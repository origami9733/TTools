using Smart.Text.Japanese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTools.Models
{
    static class OriginalStringConverter
    {
        /// <summary>
        /// 検索用の文字列の整理コンバータ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SearchWordConvert(string str)
        {
            str = KanaConverter.Convert(str, KanaOption.HankanaToHiragana);
            str = KanaConverter.Convert(str, KanaOption.KatakanaToHiragana);
            str = KanaConverter.Convert(str, KanaOption.RomanToNarrow);
            str = KanaConverter.Convert(str, KanaOption.AsciiToNarrow);
            str = KanaConverter.Convert(str, KanaOption.NumericToNarrow);

            str = str.ToUpper();
            return str;
        }
    }
}
