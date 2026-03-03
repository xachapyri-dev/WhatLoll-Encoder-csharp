using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Whatloll.Encoder
{
    /// <summary>
    /// Версия шифратора Whatloll
    /// Whatloll encoder version
    /// </summary>
    public enum WhatLollVersion
    {
        /// <summary>
        /// Первая версия (оригинальная таблица)
        /// First version (original table)
        /// </summary>
        V1 = 1,

        /// <summary>
        /// Вторая версия (расширенная таблица с английскими буквами и новой обработкой ь/ъ/ы)
        /// Second version (extended table with English letters and new ь/ъ/ы handling)
        /// </summary>
        V2 = 2
    }

    /// <summary>
    /// Класс для шифрования и дешифрования текста по таблицам замен Whatloll версий 1 и 2.
    /// Provides encryption and decryption methods using Whatloll substitution tables versions 1 and 2.
    /// </summary>
    public static class WhatLoll
    {
        #region V1 Tables

        // Таблица шифрования V1 (исходный символ -> зашифрованный)
        private static readonly Dictionary<char, string> EncryptMapV1 = new Dictionary<char, string>
        {
            {'а', "б"}, {'б', "в"}, {'в', "г"}, {'г', "д"}, {'д', "е"},
            {'е', "е1"}, {'ё', "е2"}, {'ж', "з"}, {'з', "и"}, {'и', "и1"},
            {'й', "и2"}, {'к', "л"}, {'л', "м"}, {'м', "н"}, {'н', "6"},
            {'о', "0"}, {'п', "π"}, {'р', "п"}, {'с', "ß"}, {'т', "у"},
            {'у', "ф"}, {'ф', "х"}, {'х', "ц"}, {'ц', "ч"}, {'ч', "ш"},
            {'ш', "щ"}, {'щ', "_"}, {'ь', "?"}, {'ъ', "?"}, {'ы', "?"},
            {'э', "ю"}, {'ю', "я"}, {'я', "_"}
        };

        // Таблица дешифрования V1
        private static readonly Dictionary<string, char> DecryptMapV1 = new Dictionary<string, char>();

        #endregion

        #region V2 Tables (UPDATED)

        /// <summary>
        /// Таблица шифрования V2 (обновленная)
        /// Особенности:
        /// - ь = b
        /// - ъ = -b
        /// - ы = bI
        /// - пробел = _/
        /// - спецсимволы пропускаются
        /// </summary>
        private static readonly Dictionary<char, string> EncryptMapV2 = new Dictionary<char, string>
        {
            // Русские буквы (как в V1, но с изменениями для ь/ъ/ы)
            {'а', "б"}, {'б', "в"}, {'в', "г"}, {'г', "д"}, {'д', "е"},
            {'е', "е1"}, {'ё', "е2"}, {'ж', "з"}, {'з', "и"}, {'и', "и1"},
            {'й', "и2"}, {'к', "л"}, {'л', "м"}, {'м', "н"}, {'н', "6"},
            {'о', "0"}, {'п', "π"}, {'р', "п"}, {'с', "ß"}, {'т', "у"},
            {'у', "ф"}, {'ф', "х"}, {'х', "ц"}, {'ц', "ч"}, {'ч', "ш"},
            {'ш', "щ"}, {'щ', "_"}, 
            
            // ОБНОВЛЕНО: уникальные коды для ь, ъ, ы
            {'ь', "b"},           // ь = b
            {'ъ', "-b"},          // ъ = -b
            {'ы', "bI"},          // ы = bI
            
            {'э', "ю"}, {'ю', "я"}, {'я', "_"},
            
            // Пробел
            {' ', "_/"},
            
            // Английские буквы
            {'a', "А"}, {'b', "йу"}, {'c', "ц"}, {'d', "дъ"}, {'e', "и3"},
            {'f', "2ф"}, {'g', "жъ"}, {'h', "Н"}, {'i', "8и"}, {'j', "жъй"},
            {'k', "4а"}, {'l', "m"}, {'m', ":"}, {'n', "h"}, {'o', "0E"},
            {'p', "ПE"}, {'q', "4у"}, {'r', "0p"}, {'s', "2"}, {'t', "!"},
            {'u', "%E"}, {'v', "(w-v)"}, {'w', "(v+v)"}, {'x', "1"}, {'y', "2"}, {'z', "3"}
        };

        // Таблица дешифрования V2
        private static readonly Dictionary<string, char> DecryptMapV2 = new Dictionary<string, char>();

        #endregion

        /// <summary>
        /// Статический конструктор, инициализирующий таблицы дешифрования
        /// </summary>
        static WhatLoll()
        {
            // Инициализация V1
            foreach (var pair in EncryptMapV1)
            {
                if (!DecryptMapV1.ContainsKey(pair.Value))
                {
                    DecryptMapV1.Add(pair.Value, pair.Key);
                }
            }

            // Инициализация V2
            foreach (var pair in EncryptMapV2)
            {
                if (!DecryptMapV2.ContainsKey(pair.Value))
                {
                    DecryptMapV2.Add(pair.Value, pair.Key);
                }
            }
        }

        #region Public Methods

        /// <summary>
        /// Шифрует текст версией V1
        /// </summary>
        public static string EncryptV1(string input)
        {
            return Encrypt(input, WhatLollVersion.V1);
        }

        /// <summary>
        /// Шифрует текст версией V2 (обновленная)
        /// </summary>
        public static string EncryptV2(string input)
        {
            return Encrypt(input, WhatLollVersion.V2);
        }

        /// <summary>
        /// Дешифрует текст версии V1
        /// </summary>
        public static string DecryptV1(string input)
        {
            return Decrypt(input, WhatLollVersion.V1);
        }

        /// <summary>
        /// Дешифрует текст версии V2 (обновленная)
        /// </summary>
        public static string DecryptV2(string input)
        {
            return Decrypt(input, WhatLollVersion.V2);
        }

        /// <summary>
        /// Универсальное шифрование
        /// </summary>
        public static string Encrypt(string input, WhatLollVersion version)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var map = GetEncryptMap(version);
            var result = new StringBuilder();

            foreach (char c in input)
            {
                char lowerC = char.ToLowerInvariant(c);

                if (map.ContainsKey(lowerC))
                {
                    result.Append(map[lowerC]);
                }
                else if (map.ContainsKey(c))
                {
                    result.Append(map[c]);
                }
                else
                {
                    // Для V2 спецсимволы пропускаем (оставляем как есть)
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Универсальное дешифрование
        /// </summary>
        public static string Decrypt(string input, WhatLollVersion version)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var map = GetDecryptMap(version);
            var result = new StringBuilder();
            int i = 0;

            while (i < input.Length)
            {
                bool found = false;

                // Проверяем длинные комбинации (до 5 символов)
                for (int len = 5; len >= 2; len--)
                {
                    if (i + len <= input.Length)
                    {
                        string multiChars = input.Substring(i, len);
                        if (map.ContainsKey(multiChars))
                        {
                            result.Append(map[multiChars]);
                            i += len;
                            found = true;
                            break;
                        }
                    }
                }

                // Проверяем двухсимвольные
                if (!found && i < input.Length - 1)
                {
                    string twoChars = input.Substring(i, 2);
                    if (map.ContainsKey(twoChars))
                    {
                        result.Append(map[twoChars]);
                        i += 2;
                        found = true;
                    }
                }

                // Проверяем односимвольные
                if (!found)
                {
                    string oneChar = input[i].ToString();

                    if (map.ContainsKey(oneChar))
                    {
                        result.Append(map[oneChar]);
                    }
                    else
                    {
                        // Для V2 спецсимволы пропускаем (оставляем как есть)
                        result.Append(input[i]);
                    }
                    i++;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Автоопределение версии и дешифрование
        /// </summary>
        public static string DecryptAuto(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Сначала пробуем V2 (он более специфичный)
            string resultV2 = DecryptV2(input);

            // Пробуем V1
            string resultV1 = DecryptV1(input);

            // Если результаты отличаются, проверяем специфичные паттерны V2
            if (resultV2 != resultV1)
            {
                // Проверяем наличие уникальных паттернов V2
                if (input.Contains("-b") ||          // ъ
                    input.Contains("bI") ||          // ы
                    input.Contains("(w-v)") ||
                    input.Contains("(v+v)") ||
                    input.Contains("йу") ||
                    input.Contains("дъ") ||
                    input.Contains("жъ") ||
                    input.Contains("0E") ||
                    input.Contains("ПE") ||
                    input.Contains("%E"))
                {
                    return resultV2;
                }
            }

            return resultV1;
        }

        /// <summary>
        /// Получить таблицу шифрования
        /// </summary>
        public static string GetEncryptionTable(WhatLollVersion version)
        {
            var map = GetEncryptMap(version);
            var table = new StringBuilder();

            table.AppendLine($"WhatLoll V{(int)version} Таблица шифрования:");
            table.AppendLine("========================================");

            if (version == WhatLollVersion.V2)
            {
                table.AppendLine("НОВОЕ В V2:");
                table.AppendLine("  ь = b");
                table.AppendLine("  ъ = -b");
                table.AppendLine("  ы = bI");
                table.AppendLine("  пробел = _/");
                table.AppendLine("  спецсимволы пропускаются");
                table.AppendLine("----------------------------------------");
            }

            // Группируем по категориям для V2
            if (version == WhatLollVersion.V2)
            {
                table.AppendLine("\nРУССКИЕ БУКВЫ:");
                foreach (var pair in map.Where(p =>
                    (p.Key >= 'а' && p.Key <= 'я') || p.Key == 'ё' ||
                    p.Key == 'ь' || p.Key == 'ъ' || p.Key == 'ы')
                    .OrderBy(p => p.Key))
                {
                    table.AppendLine($"  '{pair.Key}' -> '{pair.Value}'");
                }

                table.AppendLine("\nАНГЛИЙСКИЕ БУКВЫ:");
                foreach (var pair in map.Where(p => p.Key >= 'a' && p.Key <= 'z')
                    .OrderBy(p => p.Key))
                {
                    table.AppendLine($"  '{pair.Key}' -> '{pair.Value}'");
                }

                table.AppendLine("\nСПЕЦИАЛЬНЫЕ СИМВОЛЫ:");
                table.AppendLine($"  пробел -> '_/'");
            }
            else
            {
                foreach (var pair in map.OrderBy(p => p.Key))
                {
                    table.AppendLine($"  '{pair.Key}' -> '{pair.Value}'");
                }
            }

            return table.ToString();
        }

        #endregion

        #region Helper Methods

        private static Dictionary<char, string> GetEncryptMap(WhatLollVersion version)
        {
            if (version == WhatLollVersion.V1)
                return EncryptMapV1;
            else if (version == WhatLollVersion.V2)
                return EncryptMapV2;
            else
                throw new ArgumentException($"Unsupported version: {version}");
        }

        private static Dictionary<string, char> GetDecryptMap(WhatLollVersion version)
        {
            if (version == WhatLollVersion.V1)
                return DecryptMapV1;
            else if (version == WhatLollVersion.V2)
                return DecryptMapV2;
            else
                throw new ArgumentException($"Unsupported version: {version}");
        }

        #endregion
    }
}