using Unity.VisualScripting.Dependencies.NCalc;

namespace Tools.Runtime.Properties
{
    public class ExposedProperty
    {
        public string PropertyName;
        public dynamic PropertyValue;
        public string PropertyType;

        public ExposedProperty(string propertyName, string propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }
}