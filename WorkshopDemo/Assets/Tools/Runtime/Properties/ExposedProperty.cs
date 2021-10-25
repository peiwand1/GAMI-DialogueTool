namespace Tools.Runtime.Properties
{
    public class ExposedProperty
    {
        public string PropertyName;
        public string PropertyType;
        public dynamic PropertyValue;

        public ExposedProperty(string propertyName, string propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }
}