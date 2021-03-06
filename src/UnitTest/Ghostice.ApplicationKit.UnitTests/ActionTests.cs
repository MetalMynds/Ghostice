﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using Ghostice.Core;
using Newtonsoft.Json;
using System.Drawing;
using System.Threading;

namespace Ghostice.ApplicationKit.UnitTests
{
    [TestClass]
    public class ActionTests
    {

        [TestMethod]
        public void GetTextFromNestedTextBox()
        {

            using (var form = new FormNestedTabPageControls())
            {

                form.Show();


                var nestedTextBoxLocator = new Locator(new Descriptor(DescriptorType.Window, new Property("Name", "FormNestedTabPageControls")), new Descriptor(DescriptorType.Control, new Property("Name", "tabctrlTabControl")), new Descriptor(DescriptorType.Control, new Property("Name", "tabpgeTabPage1")), new Descriptor(DescriptorType.Control, new Property("Name", "txtboxTextBox")));

                TextBox textbox1 = WindowWalker.Locate(form, nestedTextBoxLocator) as TextBox;

                Assert.IsNotNull(textbox1);

                var getTextRequest = ActionRequest.Get(nestedTextBoxLocator, "Text");

                var appManager = new AutomationAvatar(String.Empty);

                //System.Threading.Thread.Sleep(512);

                var getActionResult = appManager.Perform(getTextRequest);

                Assert.IsTrue(getActionResult.Status == ActionResult.ActionStatus.Successful);

                Assert.AreEqual<String>("AMP Rules", JsonConvert.DeserializeObject<String>(getActionResult.ReturnValue));

            }

        }

    }

}