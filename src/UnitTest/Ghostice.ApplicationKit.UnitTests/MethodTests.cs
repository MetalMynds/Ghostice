﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using Ghostice.Core;

namespace Ghostice.ApplicationKit.UnitTests
{
    [TestClass]
    public class MethodTests
    {
        [TestMethod]
        public void FindStandardMethod()
        {

            Button button = new Button();

            var buttonClickMethod = MethodManager.Resolve(button, "PerformClick", null);

            Assert.IsNotNull(buttonClickMethod);

            
        }

        [TestMethod]
        public void FindExtensionMethodNoParameters()
        {

            ListView listview = new ListView();

            ExtensionManager.AddExtension(typeof(ListView), typeof(TestListViewExtension));

            var getValuesMethod = MethodManager.ResolveExtension(listview, "GetGridValues", null);

            Assert.IsNotNull(getValuesMethod);

        }

        [TestMethod]
        public void FindExtensionMethodWithParameters()
        {

            ListView listview = new ListView();

            ExtensionManager.AddExtension(typeof(ListView), typeof(TestListViewExtension));

            var getValuesMethod = MethodManager.ResolveExtension(listview, "GetGridColumnName", new Object[] { 0 } );

            Assert.IsNotNull(getValuesMethod);

        }


        [TestMethod]
        public void LoadExtensions()
        {

            ExtensionManager.LoadExtensions(".\\Extensions");

            var listViewextensionClass = ExtensionManager.GetExtensions(typeof(ListView));

            Assert.IsNotNull(listViewextensionClass);

            var menuStripExtensionClass = ExtensionManager.GetExtensions(typeof(MenuStrip));

            Assert.IsNotNull(menuStripExtensionClass);

            

        }

        [TestMethod]
        public void DoubleLoadExtensions()
        {

            ExtensionManager.LoadExtensions(".\\Extensions");

            var listViewextensionClass1 = ExtensionManager.GetExtensions(typeof(ListView));

            Assert.IsNotNull(listViewextensionClass1);

            ExtensionManager.LoadExtensions(".\\Extensions");

            var listViewextensionClass2 = ExtensionManager.GetExtensions(typeof(ListView));

            Assert.IsNotNull(listViewextensionClass2);

        }

        [TestMethod]
        public void LoadExtensionsFindExtensionMethod()
        {

            ExtensionManager.LoadExtensions(".\\Extensions");

            var listViewextensionClass = ExtensionManager.GetExtensions(typeof(ListView));

            Assert.IsNotNull(listViewextensionClass);

            var menuStripExtensionClass = ExtensionManager.GetExtensions(typeof(MenuStrip));

            Assert.IsNotNull(menuStripExtensionClass);

            using (var form = new FormExtensions())
            {


                var listviewGetValuesMethod = MethodManager.ResolveExtension(form.ListView, "GetColumns", null);

                Assert.IsNotNull(listviewGetValuesMethod);


                var menuStripGetValuesMethod = MethodManager.ResolveExtension(form.MenuStrip, "PerformClickMenu", new Object[] { "&File\\Hello Wurld" });

                Assert.IsNotNull(menuStripGetValuesMethod);

            }

        }

        [TestMethod]
        public void CallExtensionMethodOnComponent()
        {

            using (var form = new FormWithComponents())
            {

                form.Show();

                // Find Component Font Dialog

                var menuStrip1Locator = new Locator(new Descriptor(DescriptorType.Window, new Property("Name", "FormWithComponents")), new Descriptor(DescriptorType.Component, new Property("Name", "menuStrip1")));

                var formWithComponentns = WindowWalker.LocateWindow(menuStrip1Locator);

                var menuStrip1 = WindowWalker.Locate(formWithComponentns, menuStrip1Locator.GetRelativePath()) as MenuStrip;

                Assert.IsNotNull(menuStrip1);
                 
                var executeExtensionMethodRequest = ActionRequest.Execute(menuStrip1Locator, "PerformClickMenu", new ActionParameter[] { ActionParameter.Create("Hello\\Wurld") } );

                var appManager = new AutomationAvatar(".//Extensions");

                //var getActionResult = ActionResult.FromJson(appManager.Perform(getTextRequest.ToJson()));
                var executeActionResult = appManager.Perform(executeExtensionMethodRequest);

                Assert.IsNull(executeActionResult.Error);
            }

        }

  
    }
}
