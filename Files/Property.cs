namespace MaxOfEmpires.Files
{
    public class Property
    {
        /// <summary>
        /// The key of this Property. Always a string.
        /// </summary>
        public readonly string key;

        /// <summary>
        /// The value of this Property. 
        /// </summary>
        public readonly object value;

        /// <summary>
        /// Creates a new Property with the specified key-value pair.
        /// </summary>
        /// <param name="key">The key of this Property.</param>
        /// <param name="value">The value of this Property.</param>
        public Property(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// An empty Property.
        /// </summary>
        public static Property Empty => new Property("", "");
    }
}