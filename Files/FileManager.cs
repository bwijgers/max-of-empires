using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                    Property parsedLine;
                    try
                    {
                        parsedLine = ParseLine(line);
                    }
                    catch (FormatException e)
                    {
                        // Throw an error message if the line is not valid.
                        throw new FileLoadException("Error on line #" + lineNum + " in file '" + configName + "': " + e.Message);
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
            StringBuilder keyBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();
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
                    valueBuilder.Append(currentChar);
                }
                else
                {
                    keyBuilder.Append(currentChar);
                }
            }

            string key = keyBuilder.ToString().Trim();
            string value = valueBuilder.ToString().Trim();

            // No key or a key ending in a '.' means there are formatting errors
            if (key.Length == 0 || key.EndsWith("."))
            {
                throw new FormatException("Invalid key.");
            }

            return new Property(key, ParseValue(value));
        }

        private static object ParseValue(string rawValue)
        {
            // Try to see if it's a list
            if (rawValue.Length > 0 && rawValue[0] == '[')
            {
                return ParseValueAsList(rawValue);
            }

            // Check if the value is an int, and set it in the Property accordingly
            try
            {
                return int.Parse(rawValue);
            }
            catch (FormatException)
            {
            }

            // Guess it's just a normal string
            return rawValue;
        }

        private static List<string> ParseValueAsList(string rawValue)
        {
            // Initialize things we'll need later
            List<string> retVal = new List<string>();
            StringBuilder currentValue = new StringBuilder();
            bool listEnd = false;
            int i;

            // Extract elements from this list
            for (i = 1; i < rawValue.Length; ++i)
            {
                // Get the current character
                char c = rawValue[i];

                // Start of list... inside a list?
                if (c == '[')
                {
                    throw new FormatException("List declaration inside list encountered.");
                }

                // End of list
                if (c == ']')
                {
                    // End of the list
                    if (currentValue.Length == 0 && retVal.Count != 0) // Allow empty lists
                        throw new FormatException("List with empty element encountered.");
                    retVal.Add(currentValue.ToString().Trim());
                    listEnd = true;
                    break;
                }

                // End of list element
                if (c == ',')
                {
                    // If this is an empty element, throw an exception
                    if (currentValue.Length == 0)
                    {
                        throw new FormatException("List with empty element encountered.");
                    }

                    // The element is not empty
                    retVal.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                    continue;
                }

                // Space? Depending on where we find this, add it
                if (c == ' ')
                {
                    if (currentValue.Length != 0)
                        currentValue.Append(c);
                    continue;
                }

                // Anything else? Just add it to the value
                currentValue.Append(c);
            }

            // We didn't end the list, throw an exception.
            if (!listEnd)
            {
                throw new FormatException("List was not closed.");
            }

            // The list was ended prematurely, throw an exception.
            if (i != rawValue.Length - 1)
            {
                throw new FormatException("List has information after end.");
            }

            // No errors? Sad. Just return the list already :c
            return retVal;
        }
    }
}
