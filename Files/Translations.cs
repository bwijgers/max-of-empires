using System.Text;

namespace MaxOfEmpires.Files
{
    class Translations
    {
        private static Configuration language;

        public static void LoadLanguage(string langName)
        {
            StringBuilder fileLocation = new StringBuilder();
            fileLocation.Append("lang/").Append(langName).Append(".lang");
            language = FileManager.LoadConfig(fileLocation.ToString());
        }

        public static string GetTranslation(string unlocalizedString)
        {
            return language.GetProperty<string>(unlocalizedString);
        }
    }
}
