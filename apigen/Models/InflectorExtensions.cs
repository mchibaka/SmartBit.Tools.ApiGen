using System.Text.RegularExpressions;
using Humanizer.Inflections;

namespace Humanizer
{
    /// <summary>
    /// Inflector extensions
    /// </summary>
    public static class InflectorExtensions
    {
        /// <summary>
        /// Pluralizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be pluralized</param>
        /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
        /// <returns></returns>
        //public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        //{
        //    return Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);
        //}

        /// <summary>
        /// Singularizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be singularized</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
        /// <param name="skipSimpleWords">Skip singularizing single words that have an 's' on the end</param>
        /// <returns></returns>
        public static string Singularize1(this string word, bool inputIsKnownToBePlural = true, bool skipSimpleWords = false)
        {
            return Vocabularies.Default.Singularize(word, inputIsKnownToBePlural, skipSimpleWords);
        }

        /// <summary>
        /// Humanizes the input with Title casing
        /// </summary>
        /// <param name="input">The string to be titleized</param>
        /// <returns></returns>
        public static string Titleize(this string input)
        {
            return input.Humanize(LetterCasing.Title);
        }

        /// <summary>
        /// By default, pascalize converts strings to UpperCamelCase also removing underscores
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Pascalize(this string input)
        {
            return Regex.Replace(input, "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Same as Pascalize except that the first character is lower case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public static string Camelize(this string input)
        //{
        //    var word = input.Pascalize();
        //    return word.Length > 0 ? word.Substring(0, 1).ToLower() + word.Substring(1) : word;
        //}

        /// <summary>
        /// Separates the input words with underscore
        /// </summary>
        /// <param name="input">The string to be underscored</param>
        /// <returns></returns>
        public static string Underscore(this string input)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(input, @"([\p{Lu}]+)([\p{Lu}][\p{Ll}])", "$1_$2"), @"([\p{Ll}\d])([\p{Lu}])", "$1_$2"), @"[-\s]", "_").ToLower();
        }

        /// <summary>
        /// Replaces underscores with dashes in the string
        /// </summary>
        /// <param name="underscoredWord"></param>
        /// <returns></returns>
        public static string Dasherize(this string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        /// <summary>
        /// Replaces underscores with hyphens in the string
        /// </summary>
        /// <param name="underscoredWord"></param>
        /// <returns></returns>
        public static string Hyphenate(this string underscoredWord)
        {
            return Dasherize(underscoredWord);
        }

        /// <summary>
        /// Separates the input words with hyphens and all the words are converted to lowercase
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public static string Kebaberize(this string input)
        //{
        //  //  return Underscore(input).Dasherize();
        //}
    }
}

