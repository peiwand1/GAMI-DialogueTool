using System;

namespace Tools.Runtime.Properties
{

    [Serializable]
    public class ExposedProperty<T>
    {
        public string PropertyName;
        public T PropertyValue;
    }
}