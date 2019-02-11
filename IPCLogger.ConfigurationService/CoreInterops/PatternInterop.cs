using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using System.Xml;

namespace IPCLogger.ConfigurationService.CoreInterops
{
    // ReSharper disable PossibleNullReferenceException
    internal static class PatternInterop
    {
        public static XmlNode AppendConfigurationNode(XmlDocument xmlCfg, XmlNode cfgNode)
        {
            XmlNode rootNode = xmlCfg.SelectSingleNode(Constants.RootPatternsCfgPath);
            XmlNode newNode = xmlCfg.ImportNode(cfgNode, true);
            rootNode.AppendChild(newNode);
            return newNode;
        }

        public static string GetPropertyAttributeName(string propertyName)
        {
            return PFactory.PropertyAttributes[propertyName];
        }
    }
    // ReSharper restore PossibleNullReferenceException
}
