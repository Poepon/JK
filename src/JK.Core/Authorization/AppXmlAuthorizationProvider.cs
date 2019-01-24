using Abp;
using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Xml.Extensions;
using JK.IO;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace JK.Authorization
{
    public class AppXmlAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            LoadPermission(context);
        }

        protected void LoadPermission(IPermissionDefinitionContext context)
        {
            string xmlString;
            using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("JK.Web.appPermissions.config"))
            {
                if (stream == null)
                    return;
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
                    if (root == null)
                    {
                        throw new AbpException("A Permission Xml must include root as root node.");
                    }

                    if (root.HasChildNodes)
                    {
                        foreach (XmlNode item in root.ChildNodes)
                        {
                            var name = item.GetAttributeValueOrNull("name");
                            if (string.IsNullOrEmpty(name))
                            {
                                throw new AbpException("name is not defined in menu XML file!");
                            }
                            var displayName = item.GetAttributeValueOrNull("displayName");
                            var permission = context.CreatePermission(name, L(displayName));

                            Iterate(permission, item);

                        }
                    }

                }
            }
        }


        private static void Iterate(Permission permission, XmlNode xmlNode)
        {
            PopulateNode(permission, xmlNode);

            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                if (xmlChildNode.LocalName.Equals("permission", StringComparison.InvariantCultureIgnoreCase))
                {
                    var name = xmlChildNode.GetAttributeValueOrNull("name");
                    var displayName = xmlChildNode.GetAttributeValueOrNull("displayName");
                    var permissionChildNode = permission.CreateChildPermission(name, L(displayName));

                    Iterate(permissionChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(Permission permission, XmlNode xmlNode)
        {
            var description = xmlNode.GetAttributeValueOrNull("description");
            if (!string.IsNullOrEmpty(description))
                permission.Description = L(description);
            var multiTenancySides = xmlNode.GetAttributeValueOrNull("multiTenancySides");
            if (string.IsNullOrEmpty(multiTenancySides))
            {
                permission.MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant;
            }
            else
            {
                permission.MultiTenancySides = Enum.Parse<MultiTenancySides>(multiTenancySides);
            }
        }


        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, JKConsts.LocalizationSourceName);
        }
    }
}
