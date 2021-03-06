﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ghostice.Core
{
    [Serializable]
    public class WindowInfo
    {

        delegate WindowInfo UIThreadSafeCreate(Control Window);

        //[JsonConstructor]
        //public WindowInfo(long handle, String type, String name, String title, String tag)
        //{
        //    this.Handle = handle;
        //    this.Type = type;
        //    this.Name = name;
        //    this.Title = title;
        //    this.Tag = tag;
        //}

        [JsonConstructor]
        public WindowInfo(IntPtr handle, String type, String name, String title, String tag, Dictionary<String, String> AdditionalProperties)
        {
            this.Handle = handle;
            this.Type = type;
            this.Name = name;
            this.Title = title;
            this.Tag = tag;
            this.AdditionalProperties = AdditionalProperties;
        }

        public IntPtr Handle { get; protected set; }

        public String Type { get; protected set; }

        public String Name { get; protected set; }

        public String Title { get; protected set; }

        public String Tag { get; protected set; }

        public Dictionary<String, String> AdditionalProperties { get; protected set; }

        public override string ToString()
        {
            return String.Format("Windows [Name: {0} Title: {1} Handle: {2}]", Name, Title, Convert.ToString(Handle));
        }

        public static WindowInfo Create(Control Window)
        {
            if (Window.InvokeRequired)
            {
                return (WindowInfo)Window.Invoke(new UIThreadSafeCreate(Create), new Object[] { Window });
            }
            else
            {
                return new WindowInfo(Window.Handle, Window.GetType().FullName, Window.Name, Window.Text, Window.Tag == null ? "null" : Window.Tag.ToString(), null);
            }
        }

        public static WindowInfo Create(Control Window, String[] AdditionalProperytList)
        {
            if (Window.InvokeRequired)
            {
                return (WindowInfo)Window.Invoke(new UIThreadSafeCreate(Create), new Object[] { Window });
            }
            else
            {
                Dictionary<String, String> additionalPropertyValues = new Dictionary<string, string>();

                foreach (var propertyName in AdditionalProperytList)
                {

                    var propertyValue = WindowManager.GetNestedControlPropertyValue(Window, propertyName);

                    //var propertyValue = ReflectionManager.Get(Window, propertyName);

                    additionalPropertyValues.Add(propertyName, ValueConvert.ToString(propertyValue));
                }

                return new WindowInfo(Window.Handle, Window.GetType().FullName, Window.Name, Window.Text, Window.Tag == null ? "null" : Window.Tag.ToString(), additionalPropertyValues);
            }
        }


    }
}
