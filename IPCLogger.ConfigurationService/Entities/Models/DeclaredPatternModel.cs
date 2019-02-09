using IPCLogger.ConfigurationService.Helpers;
using IPCLogger.Core.Patterns;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredPatternModel
    {
        public string Id { get; protected set; }

        public string Description { get; protected set; }

        public string Events { get; protected set; }

        public bool ImmediateFlush { get; protected set; }

        public XmlNode RootXmlNode { get; set; }

        protected void InstPatternSettings(XmlNode cfgNode)
        {
            bool isNew = cfgNode == null;
            RootXmlNode = cfgNode = cfgNode ?? new XmlDocument().CreateElement("Pattern");
        }

        private void RecalculateId()
        {
            int uniqueId = 0;
            XmlNode prewNode = RootXmlNode;
            do
            {
                uniqueId += prewNode.InnerXml.Length;
                prewNode = prewNode.PreviousSibling;
            } while (prewNode != null);
            Id = BaseHelpers.CalculateMD5(uniqueId.ToString());
        }

        private void InitializeSettings(XmlNode cfgNode = null)
        {
            InstPatternSettings(cfgNode);
            RecalculateId();
        }

        internal static DeclaredPatternModel FromDeclaredPattern(DeclaredPattern source)
        {
            DeclaredPatternModel model = new DeclaredPatternModel();
            model.Description = source.Description;
            model.Events = source.Events;
            model.ImmediateFlush = source.ImmediateFlush;
            model.InitializeSettings(source.CfgNode);
            return model;
        }
    }
}
