using System;
namespace JSON
{

    public class JSON
    {
        internal static DataType GetDataType(string data)
        {

            data = data.Trim();
            if ((data.StartsWith("\"") && data.EndsWith("\"")))
            {
                return DataType.String;
            }
            else if (data.Equals("true") || data.Equals("false"))
            {
                return DataType.Bool;
            }
            else if (data.Equals("null"))
            {
                return DataType.Null;
            }
            else
            {
                try
                {
                    float.Parse(data);
                    return DataType.Number;
                }
                catch
                {
                    throw new Exception("Invalid JSON string format");
                }
            }

        }

        public static dynamic Parse(string jsonString)
        {
            if (GetObjectType(jsonString) == ObjectType.Array)
            {
                return new JArray(jsonString);
            }
            else if (GetObjectType(jsonString) == ObjectType.Object)
            {
                return new JObject(jsonString);
            }
            else
            {
                throw new System.Exception("Invalid JSON string");
            }
        }

        internal static ObjectType GetObjectType(string _object)
        {
            _object = _object.Trim();
            if (_object.StartsWith("{") && _object.EndsWith("}"))
                return ObjectType.Object;
            else if (_object.StartsWith("[") && _object.EndsWith("]"))
                return ObjectType.Array;
            else
                return ObjectType.KeyValue;
        }

        internal static dynamic GetData(string data)
        {
            DataType Type = JSON.GetDataType(data);
            if (Type == DataType.String)
            {
                return data.Substring(1, data.Length - 2);
            }
            else if (Type == DataType.Bool)
            {
                return data == "true" ? true : false;
            }
            else if (Type == DataType.Number)
            {
                return float.Parse(data.Trim());
            }
            else if (Type == DataType.Null)
            {
                return null;
            }
            else
            {
                throw new Exception("Sorry Something went wrong");
            }
        }
    }


    struct NameValue
    {
        public string Name { get; }
        public string Value { get; }
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    internal enum ObjectType { Array, Object, KeyValue };

    internal enum DataType { String, Number, Bool, Null }
}
