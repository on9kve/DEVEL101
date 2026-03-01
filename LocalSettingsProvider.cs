using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace DEVEL101
{
    /// <summary>
    /// Saves user settings to %LOCALAPPDATA%\DEVEL101\user.config in the
    /// standard user.config XML format instead of the hash-based default path.
    /// </summary>
    internal sealed class LocalSettingsProvider : SettingsProvider
    {
        private const string AppFolder   = "DEVEL101";
        private const string SectionName = "DEVEL101.Properties.Settings";

        private static string ConfigPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppFolder, "user.config");

        public override string ApplicationName { get => AppFolder; set { } }

        public override void Initialize(string name, NameValueCollection config)
            => base.Initialize(name ?? AppFolder, config ?? new NameValueCollection());

        // ------------------------------------------------------------------ read
        public override SettingsPropertyValueCollection GetPropertyValues(
            SettingsContext context, SettingsPropertyCollection props)
        {
            var result  = new SettingsPropertyValueCollection();
            var section = ReadSection();

            foreach (SettingsProperty prop in props)
            {
                var spv = new SettingsPropertyValue(prop) { IsDirty = false };

                if (section != null)
                    foreach (var el in section.Elements("setting"))
                        if ((string)el.Attribute("name") == prop.Name)
                        {
                            spv.SerializedValue = el.Element("value")?.Value;
                            break;
                        }

                result.Add(spv);
            }
            return result;
        }

        // ----------------------------------------------------------------- write
        public override void SetPropertyValues(
            SettingsContext context, SettingsPropertyValueCollection values)
        {
            // Build / update the XML document
            XDocument doc;
            XElement  section;

            if (File.Exists(ConfigPath))
            {
                try   { doc = XDocument.Load(ConfigPath); }
                catch { doc = NewDocument(); }
            }
            else
            {
                doc = NewDocument();
            }

            section = doc.Root!
                         .Element("userSettings")!
                         .Element(SectionName)!;

            foreach (SettingsPropertyValue spv in values)
            {
                // Find existing <setting> or create a new one
                XElement? node = null;
                foreach (var el in section.Elements("setting"))
                    if ((string)el.Attribute("name")! == spv.Name)
                    { node = el; break; }

                if (node == null)
                {
                    node = new XElement("setting",
                               new XAttribute("name", spv.Name),
                               new XAttribute("serializeAs", "String"),
                               new XElement("value", ""));
                    section.Add(node);
                }

                node.Element("value")!.Value = spv.SerializedValue?.ToString() ?? string.Empty;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            doc.Save(ConfigPath);
        }

        // --------------------------------------------------------------- helpers
        private XElement? ReadSection()
        {
            try
            {
                if (File.Exists(ConfigPath))
                    return XDocument.Load(ConfigPath)
                                    .Root?.Element("userSettings")
                                         ?.Element(SectionName);
            }
            catch { }
            return null;
        }

        private static XDocument NewDocument() =>
            new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("configuration",
                    new XElement("userSettings",
                        new XElement(SectionName))));
    }
}
