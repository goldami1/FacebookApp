using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookApp_Logic
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("the string to find may not be empty", "value");
            }

            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                {
                    return indexes;
                }

                indexes.Add(index);
            }
        }

        private static readonly HashSet<char> DefaultNonWordCharacters
          = new HashSet<char> { ',', '.', ':', ';' };

        public static string CropWholeWords(
          this string stringToProcess,
          int stringLengthLimit,
          HashSet<char> nonWordCharacters = null)
        {
            StringBuilder res = new StringBuilder();
            string readyPartOfString = null;
            string stringPartToBeProcessed = stringToProcess;

            if (stringToProcess == null)
            {
                throw new ArgumentNullException("value");
            }

            if (stringLengthLimit < 0)
            {
                throw new ArgumentException("Negative values not allowed.", "length");
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (stringLengthLimit >= stringToProcess.Length)
            {
                return stringToProcess;
            }

            int endIdx = stringLengthLimit;

            List<int> allIdxesOfGap = stringToProcess.AllIndexesOf(" ");
            List<int> allIdxesOfTab = stringToProcess.AllIndexesOf("\t");
            List<int> allIdxesOfNewLine = stringToProcess.AllIndexesOf(Environment.NewLine);

            List<int> allIdxes = allIdxesOfGap.Concat(allIdxesOfTab).Concat(allIdxesOfNewLine).ToList();
            allIdxes = allIdxes.OrderByDescending(x => x).ToList();
            IEnumerator<int> a = allIdxes.GetEnumerator();
            a.MoveNext();

            for (int i = endIdx; i > 0; i--)
            {
                if (a.Current == i)
                {
                    a.MoveNext();
                    break;
                }

                if (nonWordCharacters.Contains(stringToProcess[i])
                    && (stringToProcess.Length == i + 1 || stringToProcess[i + 1] == ' '))
                {
                    break;
                }

                endIdx--;
            }

            if (endIdx == 0)
            {
                endIdx = stringLengthLimit;
            }

            readyPartOfString = stringToProcess.Substring(0, endIdx);
            stringPartToBeProcessed = stringToProcess.Substring(endIdx);
            res.Append(readyPartOfString);
            if (stringPartToBeProcessed.Length > stringLengthLimit)
            {
                res.AppendFormat("{0}{1}", Environment.NewLine, stringPartToBeProcessed.CropWholeWords(stringLengthLimit));
            }
            else if (stringPartToBeProcessed.Length + 1 + readyPartOfString.Length <= stringLengthLimit)
            {
                res.AppendFormat(" {0}", stringPartToBeProcessed);
            }
            else
            {
                res.AppendFormat("{0}{1}", Environment.NewLine, stringPartToBeProcessed);
            }

            return res.ToString();
        }

        public static string getFirstNotNullStringOutOfFew(params string[] i_FewStrings)
        {
            string stringToBeFound = null;

            foreach(string oneOfStrings in i_FewStrings)
            {
                if (oneOfStrings != null && oneOfStrings.Length > 0)
                {
                    stringToBeFound = oneOfStrings;
                    break;
                }
            }

            return stringToBeFound;
        }

        public static string getFilteredStringOrNullIfNoMatch(this string i_StringToFilter, string[] i_FilterKeyWords)
        {
            string filteredString = i_StringToFilter;
            bool isKeywordFoundInString = false;

            if (i_FilterKeyWords != null)
            {
                filteredString = null;
                foreach(string keyWord in i_FilterKeyWords)
                {
                    isKeywordFoundInString = i_StringToFilter.Contains(keyWord);
                    if (isKeywordFoundInString == true)
                    {
                        filteredString = i_StringToFilter;
                        break;
                    }
                }
            }

            return filteredString;
        }
    }
}
