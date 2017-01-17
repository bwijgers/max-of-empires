using System.Text;

namespace MaxOfEmpires.Files
{
    class Translations
    {
        private static Configuration language;

        /// <summary>
        /// Loads a language as the new current language. Used to localize strings.
        /// </summary>
        /// <param name="langName">The language to load.</param>
        public static void LoadLanguage(string langName)
        {
            // Get the file location
            StringBuilder fileLocation = new StringBuilder();
            fileLocation.Append("lang/").Append(langName).Append(".lang");

            // Load the language
            language = FileManager.LoadConfig(fileLocation.ToString());
        }

        /// <summary>
        /// Gets a localized string from an unlocalized string.
        /// </summary>
        /// <param name="unlocalizedString">The unlocalized string to localize.</param>
        /// <returns>The localized string.</returns>
        public static string GetTranslation(string unlocalizedString)
        {
            return language.GetProperty<string>(unlocalizedString);
        }
    }
}
