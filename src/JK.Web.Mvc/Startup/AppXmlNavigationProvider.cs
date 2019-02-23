using Abp;
using Abp.Application.Navigation;
using Abp.Localization;
using Abp.Xml.Extensions;
using JK.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace JK.Web.Startup
{
    
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class AppXmlNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            var list = LoadMenu();
            foreach (var item in list)
            {
                if (context.Manager.Menus.ContainsKey(item.Name))
                {
                    context.Manager.Menus[item.Name] = item;
                }
                else
                {
                    context.Manager.Menus.Add(item.Name, item);
                }
            }
        }

        private List<MenuDefinition> LoadMenu()
        {
            string xmlString;
            using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("JK.Web.appMenus.config"))
            {
                xmlString = Utf8Helper.ReadStringFromStream(stream);
            }

            using (var sr = new StringReader(xmlString))
            {
                using (var xr = XmlReader.Create(sr,
                        new XmlReaderSettings
                        {
                            CloseInput = true,
                            IgnoreWhitespace = true,
                            IgnoreComments = true,
                            IgnoreProcessingInstructions = true
                        }))
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(xr);
                    var root = xmlDocument.SelectSingleNode("/root");
                    var menuDefinitionNode = root.SelectNodes("menuDefinition");
                    if (menuDefinitionNode == null || menuDefinitionNode.Count <= 0)
                    {
                        throw new AbpException("A Menu Xml must include menuDefinition as root node.");
                    }
                    List<MenuDefinition> list = new List<MenuDefinition>();

                    foreach (XmlNode node in menuDefinitionNode)
                    {
                        var name = node.GetAttributeValueOrNull("name");
                        if (string.IsNullOrEmpty(name))
                        {
                            throw new AbpException("name is not defined in menu XML file!");
                        }
                        var displayName = node.GetAttributeValueOrNull("displayName");
                        var menuDefinition = new MenuDefinition(name, L(displayName));
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode item in node.ChildNodes)
                            {
                                var itemName = item.GetAttributeValueOrNull("name");
                                if (string.IsNullOrEmpty(name))
                                {
                                    throw new AbpException("name is not defined in menu XML file!");
                                }
                                var itemDisplayName = item.GetAttributeValueOrNull("displayName");
                                var menuItemDefinition = new MenuItemDefinition(itemName, L(itemDisplayName));
                                Iterate(menuItemDefinition, item);
                                menuDefinition.AddItem(menuItemDefinition);
                            }
                        }
                        list.Add(menuDefinition);
                    }
                    return list;
                }
            }
        }


        private static void Iterate(MenuItemDefinition menuItemDefinition, XmlNode xmlNode)
        {
            PopulateNode(menuItemDefinition, xmlNode);

            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                if (xmlChildNode.LocalName.Equals("menuItemDefinition", StringComparison.InvariantCultureIgnoreCase))
                {
                    var name = xmlChildNode.GetAttributeValueOrNull("name");
                    var displayName = xmlChildNode.GetAttributeValueOrNull("displayName");
                    var siteMapChildNode = new MenuItemDefinition(name, L(displayName));
                    menuItemDefinition.Items.Add(siteMapChildNode);

                    Iterate(siteMapChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(MenuItemDefinition menuItemDefinition, XmlNode xmlNode)
        {
            var isEnabledStr = xmlNode.GetAttributeValueOrNull("isEnabled");
            menuItemDefinition.IsEnabled = string.IsNullOrEmpty(isEnabledStr) || bool.Parse(isEnabledStr);
            var isVisibleStr = xmlNode.GetAttributeValueOrNull("isVisible");
            menuItemDefinition.IsVisible = string.IsNullOrEmpty(isVisibleStr) || bool.Parse(isVisibleStr);
            menuItemDefinition.RequiredPermissionName = xmlNode.GetAttributeValueOrNull("requiredPermissionName");

            var requiresAuthenticationStr = xmlNode.GetAttributeValueOrNull("requiresAuthentication");
            if (!string.IsNullOrEmpty(requiresAuthenticationStr))
            {
                menuItemDefinition.RequiresAuthentication = bool.Parse(requiresAuthenticationStr);
            }
            menuItemDefinition.Url = xmlNode.GetAttributeValueOrNull("url");
            menuItemDefinition.Target = xmlNode.GetAttributeValueOrNull("target");
            menuItemDefinition.Icon = xmlNode.GetAttributeValueOrNull("icon");
            var orderStr = xmlNode.GetAttributeValueOrNull("order");
            if (!string.IsNullOrEmpty(orderStr))
            {
                menuItemDefinition.Order = int.Parse(orderStr);
            }
        }


        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, JKConsts.LocalizationSourceName);
        }
    }
}
