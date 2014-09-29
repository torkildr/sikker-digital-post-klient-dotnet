﻿using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SikkerDigitalPost.Net.Tests
{
    [TestClass]
    public class ManifestTester : TestBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Initialiser();
        }

        [TestMethod]
        public void ValidereManifestMotXsdValiderer()
        {
            var settings = new XmlReaderSettings();
            settings.Schemas.Add("http://begrep.difi.no/sdp/schema_v10", ManifestXsdPath());
            settings.Schemas.Add("http://begrep.difi.no/sdp/schema_v10", FellesXsdPath());
            settings.Schemas.Add("http://www.w3.org/2000/09/xmldsig#", XmlDsigCoreSchema());
            settings.ValidationType = ValidationType.Schema;
            
            try
            {
                var reader = XmlReader.Create(new MemoryStream(Manifest.Bytes), settings);
                var document = new XmlDocument();
                document.Load(reader);
            }
            catch (Exception e)
            {
                var message = String.Format("Validering feilet: {0} Inndre feilmelding: {1}", e.Message,e.InnerException);
                Assert.Fail(message);
            }
        }
        
        private string ManifestXsdPath()
        {
            return XsdFil("sdp-manifest.xsd");
        }

        private string FellesXsdPath()
        {
            return XsdFil("sdp-felles.xsd");
        }

        private string XmlDsigCoreSchema()
        {
            return XsdFil("xmldsig-core-schema.xsd");
            
        }

        private string XsdFil(string bareFilnavn)
        {
            return Path.Combine(TestDataMappe, "xsd", bareFilnavn);
        }
    }
}