using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Files
{
    class FileManager
    {
        private static Dictionary<string, Configuration> loadedConfigurations = new Dictionary<string, Configuration>();

        /// <summary>
        /// Loads a Configuration from a file, populated based on its contents.
        /// </summary>
        /// <param name="configName">The name of the Configuration.</param>
        /// <returns>The Configuration that was loaded.</returns>
        /// <exception cref="FileLoadException">If the Configuration file is not valid.</exception>
        public static Configuration LoadConfig(string configName)
        {
            // Check whether we already have this configuration, before trying to load it. 
            if (loadedConfigurations.ContainsKey(configName))
                return loadedConfigurations[configName];

            // Create the new configuration
            Configuration config = new Configuration(configName);

            // Keep track of the current line number for errors.
            int lineNum = 0;

            // Get the path to the file. If it defines its own directory, assume it's not inside the "configs" directory
            StringBuilder pathToFile = new StringBuilder("Content/");
            if (configName.Contains('/'))
            {
                pathToFile.Append(configName);
            }
            else
            {
                pathToFile.Append("configs/").Append(configName);
            }

            // If the config does not define its own extention, assume it's a .cfg file
            if (!configName.Contains('.'))
            {
                pathToFile.Append(".cfg");
            }

            // Create the reader
            using (var reader = new StreamReader(pathToFile.ToString()))
            {
                // Read everything in the file
                while (!reader.EndOfStream)
                {
                    // Get the next line in the file.
                    string line = reader.ReadLine();
                    ++lineNum;

                    // Parse the line
                    Property parsedLine = ParseLine(line);

                    // Check if the line is valid
                    if (parsedLine == null)
                    {
                        // Throw an error message if the line is not valid.
                        throw new FileLoadException("Invalid configuration file '" + configName + "'. Error on line #" + lineNum + ".");
                    }

                    // Add the config to the file
                    config.AddProperty(parsedLine);
                }
            }

            // Add the configuration to the dictionary and then return it
            loadedConfigurations[configName] = config;
            return config;
        }

        /// <summary>
        /// Parses a line and returns the line as a Property if it is valid.
        /// </summary>
        /// <param name="line">The current line that should be parsed.</param>
        /// <returns>A Property based on the contents of the line, or null if there was an error.</returns>
        private static Property ParseLine(string line)
        {
            // the total length of the line
            int lineLen = line.Length;

            // Nothing on line or this whole line is a comment
            if (lineLen == 0 || line[0] == '#')
            {
                return Property.Empty;
            }

            // Placeholders for key and value
            string key = "";
            string value = "";
            bool inValue = false;

            // Parse the line
            for (int i = 0; i < line.Length; ++i)
            {
                // Get the current char
                char currentChar = line[i];

                // Check if this is the start of a comment
                if (currentChar == '#')
                {
                    break;
                }

                // Go into value, or return an error. 
                if(currentChar == '=')
                {
                    if (inValue)
                        return null;

                    // Enter the value
                    inValue = true;
                    continue;
                }

                // Add this char to either the value or the key, depending on which is active
                if (inValue)
                {
                    value += currentChar;
                }
                else
                {
                    key += currentChar;
                }
            }

            // No key or a key ending in a '.' means there are formatting errors
            if (key.Length == 0 || key.EndsWith("."))
            {
                return null;
            }

            // Check if the value is an int, and set it in the Property accordingly
            try
            {
                return new Property(key, int.Parse(value));
            }
            catch (FormatException)
            {
            }

            return new Property(key, value);
        }
    }
}
