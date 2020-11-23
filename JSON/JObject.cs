using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSON
{
    internal class JObject : IEnumerable<object>
    {

        private Dictionary<string, JObject> _objects = new Dictionary<string, JObject>();
        private Dictionary<string, JArray> _arrays = new Dictionary<string, JArray>();
        private Dictionary<string, string> _keyValues = new Dictionary<string, string>();

        private List<object> _keys = new List<object>();
        public string JSONString { get; set; }

        public JObject(string jsonString)
        {
            if (!CheckValidJsonString(jsonString))
                throw new ArgumentException("JSON string invalid");
            JSONString = jsonString;
            ParseJSON(JSONString);
        }

        public dynamic this[string i]
        {
            get
            {
                i = String.Format("\"{0}\"", i);

                return GetValueDynamic(i);
            }
        }

        public dynamic this[int i]
        {
            get
            {
                string strI = Convert.ToString(i);

                return GetValueDynamic(strI);
            }
        }

        private bool CheckValidJsonString(string jsonString)
        {
            jsonString = jsonString.Trim();
            bool result = true;

            if(!jsonString.StartsWith("{") || !jsonString.EndsWith("}"))
            {
                result = false;
            }

            return result;
        }

        private dynamic GetValueDynamic(string i)
        {
            if (_objects.ContainsKey(i))
            {
                return _objects[i];
            }
            else if (_keyValues.ContainsKey(i))
            {
                string Data = _keyValues[i];
                return JSON.GetData(Data);
            }
            else if (_arrays.ContainsKey(i))
            {
                return _arrays[i];
            }
            else
            {
                throw new ArgumentException("JSON does not contain definition for " + i);
            }
        }

        private object GetValue(string i)
        {
            if (_objects.ContainsKey(i))
            {
                return _objects[i];
            }
            else if (_keyValues.ContainsKey(i))
            {
                
                string Data = _keyValues[i];

                return JSON.GetData(Data);

            }
            else if (_arrays.ContainsKey(i))
            {
                return _arrays[i];
            }
            else
            {
                throw new ArgumentException("JSON does not contain definition for " + i);
            }
        }

        private Dictionary<string, string> GetRawObjects(string jsonString)
        {
            jsonString = jsonString.Trim();

            Dictionary<string,string> RawObjects = new Dictionary<string, string>();

            bool IsFillName = true;

            int ArrOpen = 0;
            int ObjOpen = 0;
            bool QuoteOpen = false;

            StringBuilder TempName = new StringBuilder();
            StringBuilder TempData = new StringBuilder();


            foreach(char i in jsonString)
            {
                if (i.Equals(' '))
                {
                    if (TempName.Length ==0 || TempData.Length ==0)
                    {
                        continue;
                    }
                    else
                    {

                        (IsFillName? TempName: TempData).Append(i);
                    }
                }
                else if(i.Equals('\r') || i.Equals('\t') || i.Equals('\n'))
                {
                    if (QuoteOpen)
                    {
                        (IsFillName ? TempName : TempData).Append(i);
                    }
                    continue;
                }
                else if (i.Equals('{'))
                {
                    if (!QuoteOpen)
                    {
                        ObjOpen++;
                        if (IsFillName)
                        {
                            continue;
                        }
                    }
                    
                    (IsFillName ? TempName : TempData).Append(i);
                }
                else if (i.Equals('}'))
                {
                    if (!QuoteOpen)
                    {
                        ObjOpen--;
                        if (IsFillName)
                        {
                            continue;
                        }
                    }
                    (IsFillName ? TempName : TempData).Append(i);
                }

                else if (i.Equals('['))
                {
                    if (!QuoteOpen)
                    {
                        ArrOpen++;
                    }   
                    (IsFillName ? TempName : TempData).Append(i);
                }
                else if (i.Equals(']'))
                {
                    if (!QuoteOpen)
                    {
                        ArrOpen--;
                    }
                        
                    (IsFillName ? TempName : TempData).Append(i);
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
                    (IsFillName ? TempName : TempData).Append(i);
                }
                

                else if (i.Equals(':'))
                {
                    if (TempData.Length == 0 && !QuoteOpen && IsFillName)
                    {
                        IsFillName = false;
                    }
                    else
                    {
                        (IsFillName ? TempName : TempData).Append(i);
                    }
                }
                
                else if (i.Equals(','))
                {
                    if(ArrOpen == 0 && ObjOpen == 0 && !QuoteOpen)
                    {
                        if(TempName.Length == 0 || TempData.Length == 0)
                        {
                            continue;
                        }
                        else
                        {
                            RawObjects[TempName.ToString()] = TempData.ToString();
                            TempName.Clear();
                            TempData.Clear();
                            IsFillName = true;
                        }
                    }
                    else
                    {
                        (IsFillName ? TempName : TempData).Append(i);
                    }
                }
                else
                {
                    
                    (IsFillName ? TempName : TempData).Append(i);
                }
            }
            if (!IsFillName && !(TempData.Length == 0) && !(TempName.Length ==0))
            {
                RawObjects[TempName.ToString()] = TempData.ToString();
                TempName.Clear();
                TempData.Clear();
            }
            return RawObjects;
        }

        protected void ParseJSON(string jsonString)
        {
            jsonString = jsonString.Trim();
            Dictionary<string, string> RawObjects = GetRawObjects(jsonString.Substring(1, jsonString.Length - 2));

            Dictionary<string, string> KeyValues = RawObjects.Where((KeyValue) =>
            {
                return JSON.GetObjectType(KeyValue.Value) == ObjectType.KeyValue;
            }).ToDictionary(p=>p.Key, p=>p.Value);

            Dictionary<string, string> Objects = RawObjects.Where((KeyValue) =>
            {
                return JSON.GetObjectType(KeyValue.Value) == ObjectType.Object;
            }).ToDictionary(p => p.Key, p => p.Value);
            Dictionary<string, string> Arrays = RawObjects.Where((KeyValue) =>
            {
                return JSON.GetObjectType(KeyValue.Value) == ObjectType.Array;
            }).ToDictionary(p => p.Key, p => p.Value);

            /*List<string> RawObjects = GetRawObjects(jsonString.Substring(1,jsonString.Length - 2));

            List<string> KeyValues = RawObjects.Where(x => {
                return JSON.GetObjectType(GetNameAndValue(x).Value) == ObjectType.KeyValue;
            }).ToList();

            List<string> Objects = RawObjects.Where(x => {
                return JSON.GetObjectType(GetNameAndValue(x).Value) == ObjectType.Object;
            }).ToList();

            List<string> Arrays = RawObjects.Where(x=>{
                return JSON.GetObjectType(GetNameAndValue(x).Value) == ObjectType.Array;
            }).ToList();
            */

            foreach (KeyValuePair<string,string> KeyValue in KeyValues)
            {
                JSON.GetDataType(KeyValue.Key);
                JSON.GetDataType(KeyValue.Value);
                _keyValues[KeyValue.Key] = KeyValue.Value;
            }

            
            foreach (KeyValuePair<string, string> Object in Objects)
            {
                _objects[Object.Key] = new JObject(Object.Value);
            }

            foreach(KeyValuePair<string, string> Array in Arrays)
            {
                _arrays[Array.Key] = new JArray(Array.Key,Array.Value);
            }
        }

        
        private void LoadKeyList()
        {

            foreach (KeyValuePair<string, string> i in _keyValues)
            {
                _keys.Add(JSON.GetData(i.Key));
            }
            foreach (KeyValuePair<string, JObject> i in _objects)
            {
                _keys.Add(JSON.GetData(i.Key));
            }
            foreach (KeyValuePair<string, JArray> i in _arrays)
            {
                _keys.Add(JSON.GetData(i.Key));
            }
        }
        

        public IEnumerator<object> GetEnumerator()
        {
            LoadKeyList();
            foreach(object i in _keys)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
