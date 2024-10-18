using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common.Helper.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Helper.Extension
{
    public static class Extension
    {
        public static string CheckMobileNumber(this string mobilNumber)
        {
            if (string.IsNullOrEmpty(mobilNumber.Trim()))
                return string.Empty;

            mobilNumber = mobilNumber.Trim().Replace("+", string.Empty);

            if (mobilNumber.Length <= 9)
                return string.Empty;


            if (mobilNumber.StartsWith("989"))
            {
                if (mobilNumber.Length == 12)
                    return mobilNumber;
            }

            if (mobilNumber.StartsWith("09"))
            {
                if (mobilNumber.Length == 11)
                {
                    mobilNumber = "98" + mobilNumber.Remove(0, 1);
                    return mobilNumber;
                }
            }

            if (mobilNumber.StartsWith("9"))
            {
                if (mobilNumber.Length == 10)
                {
                    mobilNumber = "98" + mobilNumber;
                    return mobilNumber;
                }
            }
            return string.Empty;

        }

        public static long ToLongMobileNumber(this string mobilNumber)
        {
            if (string.IsNullOrEmpty(mobilNumber.Trim()))
                return 0;

            mobilNumber = mobilNumber.Trim().Replace("+", string.Empty);

            if (mobilNumber.Length < 9)
                return 0;


            if (mobilNumber.StartsWith("989"))
            {
                if (mobilNumber.Length == 12)
                    return long.Parse(mobilNumber.Substring(2, 10));
            }

            if (mobilNumber.StartsWith("09"))
            {
                if (mobilNumber.Length == 11)
                {
                    mobilNumber = mobilNumber.Remove(0, 1);
                    return long.Parse(mobilNumber);
                }
            }

            if (mobilNumber.StartsWith("9"))
            {
                if (mobilNumber.Length == 10)
                {
                    return long.Parse(mobilNumber);
                }
            }

            if (mobilNumber.Length == 9)
            {
                return long.Parse(mobilNumber);
            }
            return 0;

        }

        public static string GetFileExtension(this string fileName)
        {
            return fileName.Remove(0, fileName.LastIndexOf('.'));
        }

        public static bool IsValidEmail(this string email)
        {
            var trimmedEmail = email.Trim();

            var validator = new EmailAddressAttribute();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                return validator.IsValid(trimmedEmail);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidNationalCode(this string nationalCode)
        {
            var allDigitEqual = new Regex(@"(\d)\1{9}", RegexOptions.Compiled);
            var numberOnlyRegex = new Regex(@"\d{10}", RegexOptions.Compiled);

            if (string.IsNullOrEmpty(nationalCode))
                return false;

            if (nationalCode.Length != 10)
                return false;

            if (!numberOnlyRegex.IsMatch(nationalCode))
                return false;

            if (allDigitEqual.IsMatch(nationalCode))
                return false;

            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString(CultureInfo.InvariantCulture)) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString(CultureInfo.InvariantCulture)) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString(CultureInfo.InvariantCulture)) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString(CultureInfo.InvariantCulture)) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString(CultureInfo.InvariantCulture)) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString(CultureInfo.InvariantCulture)) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString(CultureInfo.InvariantCulture)) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString(CultureInfo.InvariantCulture)) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString(CultureInfo.InvariantCulture)) * 2;
            var a = Convert.ToInt32(chArray[9].ToString(CultureInfo.InvariantCulture));

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }

        public static string GetCountyCode(this string mobilNumber)
        {
            if (string.IsNullOrEmpty(mobilNumber))
                return "";

            if (mobilNumber.Length <= 9)
                return "";

            if (mobilNumber.StartsWith("98"))
            {
                return "98";
            }

            return "";

        }

        public static string ApplyUnifiedYeKe(this string data)
        {
            if (string.IsNullOrEmpty(data)) return data;
            return data.Replace("ي", "ی").Replace("ك", "ک");
        }

        public static string ParseValidationError(this KeyValuePair<string, string>[] items)
        {
            var validations = items.Select(c => @$"""{c.Key}"": ""{c.Value}""");
            var validationsStr = string.Join(",", validations);

            return @$"{{""errors"": {{{validationsStr}}}}}";
        }

        public static string ParseValidationReason(this KeyValuePair<string, string>[] items)
        {
            var validations = items.Select(c => @$"""{c.Key}"": ""{c.Value}""");
            var validationsStr = string.Join(",", validations);

            return @$"{{{validationsStr}}}";
        }

        public static string ParseValidationError(this string fieldName, string fieldMessage)
        {
            var validations = @$"""{fieldName}"": ""{fieldMessage}""";

            return @$"{{""errors"": {{{validations}}}}}";
        }

        public static string? ToPascalCase(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fieldName = str.Trim();
            fieldName = fieldName.Length > 0
                ? $"{fieldName[0].ToString().ToUpper()}{fieldName.Remove(0, 1)}"
                : fieldName;

            return fieldName;
        }

        public static long ToUnixTimeMilliseconds(this TimeSpan timeSpan)
        {
            var unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var result = unixEpoch.Add(timeSpan);

            var unixTimeMilliseconds = result.ToUnixTimeMilliseconds();

            return unixTimeMilliseconds;
        }

        public static string ToBase64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte ToByte(this object @object)
        {
            try
            {
                return Convert.ToByte(@object);
            }
            catch
            {
                return 0;
            }
        }
        public static string? GetPropertyValueByName(this object obj, string propertyName)
        {
            // Use reflection to get the property value by name
            var propertyInfo = obj.GetType().GetProperty(propertyName);

            // Check if the property exists
            if (propertyInfo != null)
            {
                // Get the value of the property
                return propertyInfo.GetValue(obj)?.ToString();
            }
            else
            {
                // Property not found
                return null;
            }
        }

        public static string? GetPropertyValueByNameInJsonStr(this string json, string propertyName)
        {
            // Parse the JSON string
            var jsonDoc = JsonDocument.Parse(json);

            // Access the root object
            var root = jsonDoc.RootElement;
            string? retValue = null;
            // Get the value of a specific property
            if (root.TryGetProperty(propertyName, out JsonElement nameElement))
            {
                retValue = nameElement.GetString();
            }

            // Dispose of the JsonDocument when done
            jsonDoc.Dispose();

            return retValue;
        }

        public static string RemoveByNameInJsonStr(this string json, string propertyName)
        {
            var jsonObject = JsonConvert.DeserializeObject<JObject>(json);

            if (jsonObject == null)
                return json;

            // Remove the property
            jsonObject.Remove(propertyName);

            // Serialize the modified JObject back to JSON string
            var modifiedJsonString = jsonObject.ToString();

            return modifiedJsonString;
        }

        public static string AddByNameInJsonStr(this string json, string propertyName, string propertyValue)
        {
            var jsonObject = JObject.Parse(json);

            // Add the property
            jsonObject[propertyName] = propertyValue;

            // Serialize the modified JObject back to JSON string
            var modifiedJsonString = jsonObject.ToString();

            return modifiedJsonString;
        }

        public static List<string>? SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// تبدیل رشته به بایت
        /// </summary>
        /// <param name="number">رشته بایت</param>
        /// <returns></returns>
        public static byte ToByte(this string number)
        {
            try
            {
                return Byte.Parse(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        ///تبدیل شی به فلوت
        /// </summary>
        /// <param name="object">شی فلوت</param>
        /// <returns></returns>
        public static float ToFloat(this object @object)
        {
            try
            {
                return float.Parse(@object.ToString() ?? string.Empty);
            }
            catch
            {
                return 0;
            }
        }

        ///  <summary>
        /// تبدیل رشته به فلوت
        ///  </summary>
        /// <param name="number">رشته عدد فلوت</param>
        /// <returns></returns>
        public static float ToFloat(this string number)
        {
            try
            {
                return Single.Parse(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به لانگ
        /// </summary>
        /// <param name="object">شی عدد لانگ</param>
        /// <returns></returns>
        public static long ToLong(this object @object)
        {
            try
            {
                return long.Parse(@object.ToString() ?? string.Empty, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// تبدیل رشته به لانگ
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static long ToLong(this string number)
        {
            try
            {
                return Int64.Parse(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// تبدیل شی به بولین
        /// </summary>
        /// <returns></returns>
        public static bool ToBool(this object @object)
        {
            try
            {
                return bool.Parse(@object.ToString() ?? string.Empty);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// تبدیل رشته به عدد
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static int ToInt(this string number)
        {
            try
            {
                return Int32.Parse(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به عدد
        /// </summary>
        /// <param name="object">شی عدد</param>
        /// <returns></returns>
        public static int ToInt(this object @object)
        {
            try
            {
                return int.Parse(@object.ToString() ?? string.Empty, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به عدد
        /// </summary>
        /// <param name="object">شی عدد</param>
        /// <returns></returns>
        public static short ToInt16(this object @object)
        {
            try
            {
                return short.Parse(@object.ToString() ?? string.Empty, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل رشته به عدد
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static Int16 ToInt16(this string number)
        {
            try
            {
                return Int16.Parse(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل رشته به عدد
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static Int32 ToInt32(this string number)
        {
            try
            {
                return Convert.ToInt32(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل دسیمال به عدد
        /// </summary>
        /// <param name="number">دسیمال عدد</param>
        /// <returns></returns>
        public static Int32 ToInt32(this decimal number)
        {
            try
            {
                return Convert.ToInt32(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به عدد
        /// </summary>
        /// <param name="object">شی عدد</param>
        /// <returns></returns>
        public static int ToInt32(this object @object)
        {
            try
            {
                return Convert.ToInt32(@object.ToString(), CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به عدد
        /// </summary>
        /// <param name="object">شی عدد</param>
        /// <returns></returns>
        public static long ToInt64(this object @object)
        {
            try
            {
                return Convert.ToInt64(@object.ToString(), CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به عدد
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static Int64 ToInt64(this string number)
        {
            try
            {
                return Convert.ToInt64(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل رشته به دابل
        /// </summary>
        /// <param name="number">رشته عدد</param>
        /// <returns></returns>
        public static double ToDouble(this string number)
        {
            try
            {
                return Convert.ToDouble(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static long ToUnixTimeTicks(this DateTime dateTime)
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// تبدیل شی به دابل
        /// </summary>
        /// <param name="object">شی دابل</param>
        /// <returns></returns>
        public static double ToDouble(this object @object)
        {
            try
            {
                return Convert.ToDouble(@object, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل رشته به سینگل
        /// </summary>
        /// <param name="number">شی عدد</param>
        /// <returns></returns>
        public static Single ToSingle(this object number)
        {
            try
            {
                return Convert.ToSingle(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// تبدیل شی به دسیمال
        /// </summary>
        /// <param name="number">شی دسیمال</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string number)
        {
            Int64.TryParse(number, out long value);
            return value;
        }

        public static DateTime ToDate(this string date)
        {
            return Convert.ToDateTime(date);
            //DateTime.TryParse(date, out DateTime value);
            //return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string number, bool allowNull)
        {
            Int64.TryParse(number, out long value);
            if (!allowNull)
            {
            }

            return value;
        }

        /// <summary>
        /// تبدیل شی به دسیمال
        /// </summary>
        /// <param name="number">شی دسیمال</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object number)
        {
            try
            {
                return Convert.ToDecimal(number, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static string ToAmountSeparation(this object sender)
        {
            try
            {
                return $"{int.Parse(s: sender.ToString() ?? string.Empty):#,###,###.##}";
            }
            catch
            {
                return "";
            }
        }

        public static bool IsEvenNumber(this long number)
        {
            return number % 2 != 0;
        }

        /// <summary>
        /// نبدیل عدد به حروف فارسی
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToFarsi(this string number)
        {
            try
            {
                var a = new AtoH();
                return a.C15D(number);
            }
            catch
            {
                return "";
            }
        }

        public static long ToToman(this decimal number)
        {
            return (long)Math.Round(number / 10);
        }


        public static long ToToman(this long number)
        {
            return (long)Math.Round((decimal)number / 10);
        }

        public static decimal ToRiyal(this decimal number)
        {
            return number * 10;
        }

        public static long ToRiyal(this long number)
        {
            return number * 10;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var array = source.ToArray();
            var n = array.Length;
            for (var i = 0; i < n; i++)
            {
                // Exchange a[i] with random element in a[i..n-1]
                var r = i + RandomProvider.Next(0, n - i);
                (array[i], array[r]) = (array[r], array[i]);
            }

            return array;
        }
    }

    public class AtoH
    {

        private string F1(string s)
        {
            var t = "";
            switch (s)
            {
                case "0":
                    {
                        t = "";
                        break;
                    }
                case "1":
                    {
                        t = "یکصد";
                        break;
                    }
                case "2":
                    {
                        t = "دویست";
                        break;
                    }
                case "3":
                    {
                        t = "سیصد";
                        break;
                    }
                case "4":
                    {
                        t = "چهارصد";
                        break;
                    }
                case "5":
                    {
                        t = "پانصد";
                        break;
                    }
                case "6":
                    {
                        t = "ششصد";
                        break;
                    }
                case "7":
                    {
                        t = "هفتصد";
                        break;
                    }
                case "8":
                    {
                        t = "هشتصد";
                        break;
                    }
                case "9":
                    {
                        t = "نهصد";
                        break;
                    }
            }

            return t;
        }

        private static string F2(string s)
        {
            var t = "";
            if (s.Length == 0) return ("");
            if (s.Length == 1) s = "0" + s;

            switch (s)
            {
                case "00":
                    {
                        t = "";
                        break;
                    }

                case "01":
                    {
                        t = "یک";
                        break;
                    }

                case "02":
                    {
                        t = "دو";
                        break;
                    }

                case "03":
                    {
                        t = "سه";
                        break;
                    }

                case "04":
                    {
                        t = "چهار";
                        break;
                    }

                case "05":
                    {
                        t = "پنج";
                        break;
                    }

                case "06":
                    {
                        t = "شش";
                        break;
                    }

                case "07":
                    {
                        t = "هفت";
                        break;
                    }

                case "08":
                    {
                        t = "هشت";
                        break;
                    }

                case "09":
                    {
                        t = "نه";
                        break;
                    }

                case "10":
                    {
                        t = "ده";
                        break;
                    }

                case "11":
                    {
                        t = "یازده";
                        break;
                    }

                case "12":
                    {
                        t = "دوازده";
                        break;
                    }

                case "13":
                    {
                        t = "سیزده";
                        break;
                    }

                case "14":
                    {
                        t = "چهارده";
                        break;
                    }

                case "15":
                    {
                        t = "پانزده";
                        break;
                    }

                case "16":
                    {
                        t = "شانزده";
                        break;
                    }

                case "17":
                    {
                        t = "هفده";
                        break;
                    }

                case "18":
                    {
                        t = "هجده";
                        break;
                    }

                case "19":
                    {
                        t = "نوزده";
                        break;
                    }

                case "20":
                    {
                        t = "بیست";
                        break;
                    }

                case "21":
                    {
                        t = "بیست ویک";
                        break;
                    }

                case "22":
                    {
                        t = "بیست و دو";
                        break;
                    }

                case "23":
                    {
                        t = "بیست و سه";
                        break;
                    }

                case "24":
                    {
                        t = "بیست و چهار";
                        break;
                    }

                case "25":
                    {
                        t = "بیست و پنج";
                        break;
                    }

                case "26":
                    {
                        t = "بیست و شش";
                        break;
                    }

                case "27":
                    {
                        t = "بیست و هفت";
                        break;
                    }

                case "28":
                    {
                        t = "بیست و هشت";
                        break;
                    }

                case "29":
                    {
                        t = "بیست و نه";
                        break;
                    }

                case "30":
                    {
                        t = "سی";
                        break;
                    }

                case "31":
                    {
                        t = "سی ویک";
                        break;
                    }

                case "32":
                    {
                        t = "سی و دو";
                        break;
                    }

                case "33":
                    {
                        t = "سی و سه";
                        break;
                    }

                case "34":
                    {
                        t = "سی و چهار";
                        break;
                    }

                case "35":
                    {
                        t = "سی و پنج";
                        break;
                    }

                case "36":
                    {
                        t = "سی و شش";
                        break;
                    }

                case "37":
                    {
                        t = "سی و هفت";
                        break;
                    }

                case "38":
                    {
                        t = "سی و هشت";
                        break;
                    }

                case "39":
                    {
                        t = "سی و نه";
                        break;
                    }


                case "40":
                    {
                        t = "چهل";
                        break;
                    }

                case "41":
                    {
                        t = "چهل ویک";
                        break;
                    }

                case "42":
                    {
                        t = "چهل و دو";
                        break;
                    }

                case "43":
                    {
                        t = "چهل و سه";
                        break;
                    }

                case "44":
                    {
                        t = "چهل و چهار";
                        break;
                    }

                case "45":
                    {
                        t = "چهل و پنج";
                        break;
                    }

                case "46":
                    {
                        t = "چهل و شش";
                        break;
                    }

                case "47":
                    {
                        t = "چهل و هفت";
                        break;
                    }

                case "48":
                    {
                        t = "چهل و هشت";
                        break;
                    }

                case "49":
                    {
                        t = "چهل و نه";
                        break;
                    }

                case "50":
                    {
                        t = "پنجاه";
                        break;
                    }

                case "51":
                    {
                        t = "پنجاه ویک";
                        break;
                    }

                case "52":
                    {
                        t = "پنجاه و دو";
                        break;
                    }

                case "53":
                    {
                        t = "پنجاه و سه";
                        break;
                    }

                case "54":
                    {
                        t = "پنجاه و چهار";
                        break;
                    }

                case "55":
                    {
                        t = "پنجاه و پنج";
                        break;
                    }

                case "56":
                    {
                        t = "پنجاه و شش";
                        break;
                    }

                case "57":
                    {
                        t = "پنجاه و هفت";
                        break;
                    }

                case "58":
                    {
                        t = "پنجاه و هشت";
                        break;
                    }

                case "59":
                    {
                        t = "پنجاه و نه";
                        break;
                    }


                case "60":
                    {
                        t = "شصت";
                        break;
                    }

                case "61":
                    {
                        t = "شصت ویک";
                        break;
                    }

                case "62":
                    {
                        t = "شصت و دو";
                        break;
                    }

                case "63":
                    {
                        t = "شصت و سه";
                        break;
                    }

                case "64":
                    {
                        t = "شصت و چهار";
                        break;
                    }

                case "65":
                    {
                        t = "شصت و پنج";
                        break;
                    }

                case "66":
                    {
                        t = "شصت و شش";
                        break;
                    }

                case "67":
                    {
                        t = "شصت و هفت";
                        break;
                    }

                case "68":
                    {
                        t = "شصت و هشت";
                        break;
                    }

                case "69":
                    {
                        t = "شصت و نه";
                        break;
                    }

                case "70":
                    {
                        t = "هفتاد";
                        break;
                    }

                case "71":
                    {
                        t = "هفتاد ویک";
                        break;
                    }

                case "72":
                    {
                        t = "هفتاد و دو";
                        break;
                    }

                case "73":
                    {
                        t = "هفتاد و سه";
                        break;
                    }

                case "74":
                    {
                        t = "هفتاد و چهار";
                        break;
                    }

                case "75":
                    {
                        t = "هفتاد و پنج";
                        break;
                    }

                case "76":
                    {
                        t = "هفتاد و شش";
                        break;
                    }

                case "77":
                    {
                        t = "هفتاد و هفت";
                        break;
                    }

                case "78":
                    {
                        t = "هفتاد و هشت";
                        break;
                    }

                case "79":
                    {
                        t = "هفتاد و نه";
                        break;
                    }

                case "80":
                    {
                        t = "هشتاد";
                        break;
                    }

                case "81":
                    {
                        t = "هشتاد ویک";
                        break;
                    }

                case "82":
                    {
                        t = "هشتاد و دو";
                        break;
                    }

                case "83":
                    {
                        t = "هشتاد و سه";
                        break;
                    }

                case "84":
                    {
                        t = "هشتاد و چهار";
                        break;
                    }

                case "85":
                    {
                        t = "هشتاد و پنج";
                        break;
                    }

                case "86":
                    {
                        t = "هشتاد و شش";
                        break;
                    }

                case "87":
                    {
                        t = "هشتاد و هفت";
                        break;
                    }

                case "88":
                    {
                        t = "هشتاد و هشت";
                        break;
                    }

                case "89":
                    {
                        t = "هشتاد و نه";
                        break;
                    }

                case "90":
                    {
                        t = "نود";
                        break;
                    }

                case "91":
                    {
                        t = "نود ویک";
                        break;
                    }

                case "92":
                    {
                        t = "نود و دو";
                        break;
                    }

                case "93":
                    {
                        t = "نود و سه";
                        break;
                    }

                case "94":
                    {
                        t = "نود و چهار";
                        break;
                    }

                case "95":
                    {
                        t = "نود و پنج";
                        break;
                    }

                case "96":
                    {
                        t = "نود و شش";
                        break;
                    }

                case "97":
                    {
                        t = "نود و هفت";
                        break;
                    }

                case "98":
                    {
                        t = "نود و هشت";
                        break;
                    }

                case "99":
                    {
                        t = "نود و نه";
                        break;
                    }

            } // switch (s)

            return t;
        }

        private string C3D(string s)
        {
            if (s.Length == 0) return ("");
            if (s.Length == 1) s = "00" + s;
            if (s.Length == 2) s = "0" + s;
            if (s == "000") return ("");

            var haveVa = "";
            if ((s.Substring(1, 2) != "00") && (s.Substring(0, 1) != "0")) haveVa = " و ";
            var t = F1(s.Substring(0, 1)) + haveVa + F2(s.Substring(1, 2));
            return t;
        }

        public string C15D(string s)
        {
            var t = "";
            if (s.Length == 0) return ("نامشخص");
            if (s.Length == 1) s = "00000000000000" + s;
            if (s.Length == 2) s = "0000000000000" + s;
            if (s.Length == 3) s = "000000000000" + s;
            if (s.Length == 4) s = "00000000000" + s;
            if (s.Length == 5) s = "0000000000" + s;
            if (s.Length == 6) s = "000000000" + s;
            if (s.Length == 7) s = "00000000" + s;
            if (s.Length == 8) s = "0000000" + s;
            if (s.Length == 9) s = "000000" + s;
            if (s.Length == 10) s = "00000" + s;
            if (s.Length == 11) s = "0000" + s;
            if (s.Length == 12) s = "000" + s;
            if (s.Length == 13) s = "00" + s;
            if (s.Length == 14) s = "0" + s;
            if (s == "000000000000000") return ("صفر");

            if (s.Substring(0, 3) != "000")
            {
                t = t + C3D((s.Substring(0, 3))) + " هزار";
                if (s.Substring(3, 12) != "000000000000")
                {
                    if (s.Substring(3, 3) != "000")
                        t = t + " و";
                }
            }

            if (s.Substring(3, 3) != "000")
            {
                t = t + C3D((s.Substring(3, 3))) + " میلیارد";
                if (s.Substring(6, 9) != "000000000") t = t + " و";
            }
            else
            {
                if (s.Substring(0, 3) != "000") t = t + " میلیارد و";
            }

            if (s.Substring(6, 3) != "000")
            {
                t = t + C3D((s.Substring(6, 3))) + " میلیون";
                if (s.Substring(9, 6) != "000000") t = t + " و";
            }

            if (s.Substring(9, 3) != "000")
            {
                t = t + C3D((s.Substring(9, 3))) + " هزار";
                if (s.Substring(12, 3) != "000") t = t + " و";
            }

            if (s.Substring(12, 3) != "000")
            {
                t = t + C3D((s.Substring(12, 3)));
            }
            return t;
        }



    }
}
