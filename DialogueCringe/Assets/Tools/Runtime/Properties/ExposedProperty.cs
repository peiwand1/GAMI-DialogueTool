using Unity.VisualScripting.Dependencies.NCalc;

namespace Tools.Runtime.Properties
{
    public class ExposedProperty
    {
        public string PropertyName = "New Property";
        public dynamic PropertyValue;
        public string PropertyType;

        public ExposedProperty(string propertyType)
        {
            PropertyType = propertyType;
        }
    }
}