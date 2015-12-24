﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ghostice.Core
{
    public static class WindowWalker
    {
        delegate Object UIThreadSafeLocate(Control Root, ControlPath Path);

        public static Control Locate(ControlDescription Description)
        {
            var targetWindows = WindowManager.GetWindowControls();

            Control targetWindow = null;

            var windowDescriptor = Description;

            foreach (var window in targetWindows)
            {

                if (window != null)
                {

                    if (WindowWalker.Compare(windowDescriptor, window))
                    {
                        targetWindow = window;
                        break;
                    }

                }
            }

            return targetWindow;
        }

        public static Object Locate(Control Root, ControlPath Path)
        {

            Object currentControl = Root;
            Boolean failed = false;

            if (Root.InvokeRequired)
            {
                return Root.Invoke(new UIThreadSafeLocate(Locate), new Object[] { Root, Path });
            }
            else
            {

                foreach (var description in Path.Path)
                {
                    Control childControl = null;

                    if (TryLocate((Control)currentControl, description, out childControl))
                    {
                        currentControl = childControl;
                    }
                    else
                    {
                        failed = true;
                        break;
                    }

                }

                if (failed && (currentControl is Form || currentControl is UserControl))
                {

                    failed = false;

                    var form = currentControl;

                    var currentFields = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    var description = Path.Path.Count == 0 ? null : Path.Path[0];

                    if (description != null)
                    {

                        foreach (var field in currentFields)
                        {

                            var childComponent = field.GetValue(form) as Component;

                            if (childComponent != null)
                            {
                                var properties = GetProperties(childComponent, description.RequiredProperties);

                                if (!properties.HasProperty("Name"))
                                {
                                    properties.List.Add(Property.Create("Name", field.Name));
                                }

                                if (Compare(description, properties))
                                {
                                    currentControl = field.GetValue(form) as Component;
                                    failed = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (failed) throw new LocationNotFoundException(Root, Path.ToString());

            }

            return currentControl;
        }

        private static Boolean TryLocate(Object Root, ControlDescription Description, out Control Found)
        {

            if (Root is Control)
            {
                var root = Root as Control;

                var properties = GetProperties(root, Description.RequiredProperties);

                if (Compare(Description, properties))
                {
                    Found = root;
                    return true;

                }

                foreach (Control control in root.Controls)
                {

                    if (TryLocate(control, Description, out Found))
                    {
                        return true;
                    }
                }

                Found = null;
                return false;

            }
            else
            {
                Found = null;
                return false;
            }
        }

        public static Boolean Compare(ControlDescription Description, Control Control)
        {
            return Compare(Description, GetProperties(Control, Description.RequiredProperties));
        }

        public static Boolean Compare(ControlDescription Description, PropertyCollection Properties)
        {
            foreach (var propertyName in Description.RequiredProperties)
            {
                var expected = Description.GetProperty(propertyName);

                foreach (var property in Properties.List)
                {
                    if (property.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)
                        && !property.Value.Equals(expected.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static PropertyCollection GetProperties(Component Target, params String[] Names)
        {
            var propertiesCollection = new PropertyCollection();

            //var controlProperties = Path.GetType().GetProperties();

            foreach (var propertyName in Names)
            {

                if (propertyName.Equals("Type", StringComparison.InvariantCultureIgnoreCase) || propertyName.Equals("Class", StringComparison.InvariantCultureIgnoreCase))
                {
                    var newProperty = new Property(propertyName, (String)Target.GetType().Name);

                    propertiesCollection.List.Add(newProperty);
                    
                }
                else if (propertyName.Equals("FullType", StringComparison.InvariantCultureIgnoreCase) || propertyName.Equals("FullClass", StringComparison.InvariantCultureIgnoreCase))
                {
                    var newProperty = new Property(propertyName, (String)Target.GetType().FullName);

                    propertiesCollection.List.Add(newProperty);
                }
                else
                {

                    var property = Target.GetType().GetProperty(propertyName);

                    if (property != null && Names.Contains<String>(property.Name) || Names == null)
                    {
                        var newProperty = new Property(property.Name, (String)property.GetValue(Target, null));

                        propertiesCollection.List.Add(newProperty);
                    }
                }
            }

            return propertiesCollection;
        }

    }

    [Serializable]
    public class WindowWalkerException : Exception
    {

        protected WindowWalkerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public WindowWalkerException(String Message) :
            base(Message)
        {

        }

        public WindowWalkerException(String Message, Exception Inner) :
            base(Message, Inner)
        {

        }

    }

    [Serializable]
    public class LocationNotFoundException : WindowWalkerException
    {

        protected LocationNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public LocationNotFoundException(Control Root, String Path) :
            base(String.Format("Window Walker Can't Locate Control!\r\nPath: {0}\r\nRoot: {1}", Path, Root.GetType().FullName))
        {

        }


    }

}