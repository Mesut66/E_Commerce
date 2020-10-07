using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData , string key,T value)where T:class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData,string key) where T:class 
        {
            object obj;
            tempData.TryGetValue(key, out obj);
            return obj == null ? null : JsonConvert.DeserializeObject<T>((string)obj);
        }
    }
}
