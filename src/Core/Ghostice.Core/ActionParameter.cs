﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ghostice.Core
{
    [Serializable]
    public class ActionParameter
    {

        protected ActionParameter()
        {
        }

        public Type ValueType
        {
            get; set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TypeCode TypeCode
        {
            get; set;
        }

        public Object Value
        {
            get; set;
        }
     
        public static ActionParameter Create(Object value)
        {
            var typeCode = Convert.GetTypeCode(value);

            var type = typeCode == TypeCode.Object ? value.GetType() : null;

            return new ActionParameter { Value = value, ValueType = type, TypeCode = typeCode };
        }

        //public static ActionParameter[] Create(String[] parameters)
        //{

        //    List<ActionParameter> parameterList = new List<ActionParameter>();

        //    foreach (var parameter in parameters)
        //    {

        //        var typeCode = TypeCode.String;

        //        var type = typeof(String);

        //        parameterList.Add(new ActionParameter { Value = parameters, ValueType = type, TypeCode = typeCode });
        //    }

        //    return parameterList.ToArray();

        //}


        public override string ToString()
        {
            return Value != null ? Value.ToString() : "null";
        }
    }
}
