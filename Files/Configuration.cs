using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Files
{
    class Configuration
    {
        /// <summary>
        /// A function called when iterating over the Configuration using ForEach
        /// </summary>
        /// <param name="key">The key of the Property at the current iteration.</param>
        /// <param name="value">The value of the Property at the current iteration.</param>
        private delegate void IteratorFunction(string key, object value);

        /// <summary>
        /// The filename this Configuration corresponds with. Used for error messages.
        /// </summary>
        private string filename;

        /// <summary>
        /// The Properties contained in this Configuration.
        /// </summary>
        private List<Property> properties;

        /// <summary>
        /// Creates a new Configuration with a filename. Will not be populated using this constructor.
        /// </summary>
        /// <param name="filename">The name of this configuration.</param>
        public Configuration(string filename)
        {
            this.filename = filename;
            this.properties = new List<Property>();
        }

        /// <summary>
        /// Adds a Property to this Configuration.
        /// </summary>
        /// <param name="p">The Property to add.</param>
        public void AddProperty(Property p)
        {
            // If this is an empty property, do nothing
            if (p.key == "")
                return;

            // Else, set the key and value in this file
            properties.Add(p);
        }

        /// <summary>
        /// Executes a function for every Property in this Configuration.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        private void ForEach(IteratorFunction func)
        {
            // Check if the function does anything at all
            if (func == null || func.GetInvocationList().Length == 0)
                return;

            // Iterate over all properties
            foreach (Property p in properties)
            {
                func.Invoke(p.key, p.value);
            }
        }

        /// <summary>
        /// Gets a Property's value based on the name of the Property, casted to a specified type, if possible.
        /// </summary>
        /// <typeparam name="T">The type to cast the requested value to.</typeparam>
        /// <param name="propName">The name of the Property asked.</param>
        /// <returns>The value casted to the specific type.</returns>
        /// <exception cref="ArgumentException">When the key is invalid or null.</exception>
        /// <exception cref="InvalidCastException">When the key is valid, but the type to cast it to is not.</exception>
        public T GetProperty<T>(string propName)
        {
            // Check if the requested property exists
            if (propName == null || !properties.Exists(property => property.key.Equals(propName)))
            {
                throw new ArgumentException("Property name '" + propName + "' not found in file '" + filename + "'. File is probably incorrect.");
            }

            // If it exists, return it if it is of the correct type
            object prop = properties.Find(property => property.key.Equals(propName)).value;
            if (prop is T)
            {
                return (T)prop;
            }

            // The property is of invalid type
            throw new InvalidCastException("Property name '" + propName + "' is of another type than requested in file '" + filename + "'.");
        }

        /// <summary>
        /// Creates a new Configuration based on a subsection of this Configuration.
        /// </summary>
        /// <param name="sectionName">The name of the section to narrow down to.</param>
        /// <returns>The specified subsection of this Configuration. Empty if the section does not exist.</returns>
        public Configuration GetPropertySection(string sectionName)
        {
            // Create the sub-configuration
            Configuration subConf = new Configuration(filename + "=>" + sectionName);

            // Populate the sub-configuration
            ForEach((k, v) => {
                // If the key is in the section...
                if (IsInSection(k, sectionName))
                {
                    // ... add the key, minus the section identifier, to the new configuration list with its corresponding value.
                    subConf.AddProperty(new Property(k.Substring(sectionName.Length + 1), v));
                }
            });

            // Return the sub-configuration
            return subConf;
        }

        /// <summary>
        /// Checks to see if the specified key is in the specified section.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="sectionName">The section to check whether the key is in it.</param>
        /// <returns>True if the key is part of the specified section, false otherwise.</returns>
        private bool IsInSection(string key, string sectionName)
        {
            // Create the section identifier
            string sectionIdent = sectionName + '.';

            // Return whether the key is made up of the identifier + more text
            return key.StartsWith(sectionIdent); // Cannot be just this, as a key can never end with a '.'
        }
    }
}
