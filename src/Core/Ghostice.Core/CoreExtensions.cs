﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ghostice.Core
{
    public static class CoreExtensions
    {

        delegate String UIThreadSafeDescribe(Control Target);

        public static String Describe(this Control Target)
        {

            if (Target != null)
            {

                if (Target.InvokeRequired)
                {
                    return (String)Target.Invoke(new UIThreadSafeDescribe(Describe), new Object[] {Target});
                }
                else
                {
                    StringBuilder descriptionBuilder = new StringBuilder();

                    try
                    {
                        descriptionBuilder.AppendFormat("Control Name: {0} Handle: [{1}] Hex [{2}] Type: [{3}]", Target.Name,
                            Target.Handle, Target.Handle.ToString("X"), Target.GetType().FullName);

                    }
                    catch (ObjectDisposedException)
                    {

                        descriptionBuilder.AppendFormat("Control Has Been Disposed");
                    }
                    catch (Exception ex)
                    {

                        descriptionBuilder.AppendFormat(
                            "Exception Describing Control!\r\nError: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                    }

                    return descriptionBuilder.ToString();
                }
            }
            else
            {
                return "null";
            }
        }

        public static String Describe(this Component Target)
        {

            StringBuilder descriptionBuilder = new StringBuilder();

            try
            {
                descriptionBuilder.AppendFormat("Component Type: [{0}]", Target.GetType().FullName);

            }
            catch (ObjectDisposedException)
            {

                descriptionBuilder.AppendFormat("Component Has Been Disposed");
            }
            catch (Exception ex)
            {

                descriptionBuilder.AppendFormat("Exception Describing Component!\r\nError: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }

            return descriptionBuilder.ToString();

        }

    }
}
