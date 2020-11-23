using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JSON
{
    internal class JArray : IEnumerable<object>
    {
        private string _name;
        private int _count;

        private Dictionary<int, JObject> _objects = new Dictionary<int, JObject>();
        private Dictionary<int, JArray> _arrays = new Dictionary<int, JArray>();
        private Dictionary<int, string> _values = new Dictionary<int, string>();

        public string Name 
        { 
            get
            {

                return _name ?? throw new NullReferenceException("Name is null");
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }
        public JArray(string name, string value)
        {
            _name = name;
            ParseArray(value.Trim());
        }

        public JArray(string value)
        {
            //_name = name;
            ParseArray(value.Trim());
        }


        private object GetValue(int i)
        {
            if (_arrays.ContainsKey(i))
            {
                return _arrays[i];
            }
            else if (_objects.ContainsKey(i))
            {
                return _objects[i];
            }
            else if (_values.ContainsKey(i))
            {
                string Data = _values[i];
                DataType Type = JSON.GetDataType(Data);
                if (Type == DataType.String)
                {
                    return Data.Substring(1, Data.Length - 2);
                }
                else if (Type == DataType.Bool)
                {
                    return Data == "true" ? true : false;
                }
                else if (Type == DataType.Number)
                {
                    return float.Parse(Data);
                }
                else if (Type == DataType.Null)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Sorry, Something went wrong");
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Array index out of range");
            }
        }

        private dynamic GetValueDynamic(int i)
        {
            if (_arrays.ContainsKey(i))
            {
                return _arrays[i];
            }
            else if (_objects.ContainsKey(i))
            {
                return _objects[i];
            }
            else if (_values.ContainsKey(i))
            {
                string Data = _values[i];
                DataType Type = JSON.GetDataType(Data);
                if (Type == DataType.String)
                {
                    return Data.Substring(1, Data.Length - 2);
                }
                else if (Type == DataType.Bool)
                {
                    return Data == "true" ? true : false;
                }
                else if (Type == DataType.Number)
                {
                    return float.Parse(Data);
                }
                else if (Type == DataType.Null)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Sorry, Something went wrong");
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Array index out of range");
            }
        }

        public dynamic this[int i]
        {
            get
            {
                return GetValueDynamic(i);
            }
        }

        private List<string> GetRawObjects(string jsonString)
        {
            jsonString = jsonString.Trim();
            List<string> RawObjects = new List<string>();


            int ArrOpen = 0;
            int ObjOpen = 0;
            bool QuoteOpen = false;

            StringBuilder Temp = new StringBuilder();

            foreach (char i in jsonString)
            {
                if(i.Equals(' '))
                {
                    if (Temp.Length == 0)
                    {
                        continue;
                    }
                    Temp.Append(i);
                }
                else if (i.Equals('\r') || i.Equals('\t') || i.Equals('\n'))
                {
                    if (QuoteOpen)
                    {
                        Temp.Append(i);
                    }
                    continue;
                }


                else if (i.Equals('{'))
                {
                    if(!QuoteOpen)
                        ObjOpen++;
                    Temp.Append(i);
                }
                else if (i.Equals('}'))
                {
                    if (!QuoteOpen)
                        ObjOpen--;

                    Temp.Append(i);
                }

                else if (i.Equals('['))
                {
                    if (!QuoteOpen)
                        ArrOpen++;
                    Temp.Append(i);
                }
                else if (i.Equals(']'))
                {
                    if (!QuoteOpen)
                        ArrOpen--;
                    Temp.Append(i);
                }

                else if (i.Equals('"'))
                {
                    if (QuoteOpen)
                    {
                        QuoteOpen = false;
                    }
                    else
                    {
                        QuoteOpen = true;
                    }
                    Temp.Append(i);
                }

                else if (i.Equals(','))
                {
                    if (ArrOpen == 0 & ObjOpen == 0 && !QuoteOpen)
                    {
                        if (!(Temp.Length == 0))
                        {
                            RawObjects.Add(Temp.ToString());
                            Temp.Clear();
                        }
                        else
                        {
                            continue;
                        }
                        
                    }
                    else
                    {
                        Temp.Append(i);
                    }
                }

                else
                {
                    Temp.Append(i);
                }
            }
            if (!Temp.Equals(""))
            {
                RawObjects.Add(Temp.ToString());
                Temp.Clear();
            }
            return RawObjects;
        }

        private void ParseArray(string value)
        {
            value = value.Trim();
            if (value.Contains(","))
            {
                string Raw = value.Substring(1, value.Length - 2);
                List<string> RawValues = GetRawObjects(Raw);

                foreach (string i in RawValues)
                {
                    
                    if (JSON.GetObjectType(i) == ObjectType.Array)
                    {
                        _arrays[_count] = new JArray(i);
                    }
                    else if (JSON.GetObjectType(i) == ObjectType.Object)
                    {
                        _objects[_count] = new JObject(i);
                    }
                    else
                    {
                        JSON.GetDataType(i);
                        JSON.GetDataType(i);
                        _values[_count] = i;
                    }
                    _count++;
                }
            }
            

        }

        public IEnumerator<object> GetEnumerator()
        {
            for(int i=0; i<_count; i++)
            {
                yield return(GetValue(i));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
