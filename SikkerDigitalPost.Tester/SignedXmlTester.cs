﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SikkerDigitalPost.Klient.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SikkerDigitalPost.Tester
{
    [TestClass]
    public class SignedXmlTester : TestBase
    {        
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Initialiser();
        }

        [TestMethod]
        public void FindIdElement()
        {
            var tests = new[] {
                "<{0} {2}='{1}'></{0}>", 
                "<{0} {2}='{1}'></{0}>", 
                "<{0} {2}='{1}'></{0}>", 
                "<container><{0} {2}='{1}'></{0}></container>", 
                "<container><invalid Id='notThis' /><{0} {2}='{1}'></{0}></container>", 
                "<container><invalid ID='notThis'><{0} {2}='{1}'></{0}></invalid></container>",
                "<container xmlns='http://example.org'><{0} {2}='{1}'></{0}></container>", 
                "<a:container xmlns:a='http://example.org'><{0} {2}='{1}'></{0}></a:container>", 
                "<a:container xmlns:a='http://example.org'><a:{0} {2}='{1}'></a:{0}></a:container>", 
                "<a:container xmlns:a='http://example.org'><b:{0} xmlns:b='http://nowhere.com' {2}='{1}'></b:{0}></a:container>", 
                "<a:container xmlns:a='http://example.org'><{0} xmlns='' {2}='{1}'></{0}></a:container>", 
                "<a:container xmlns:a='http://example.org'><{0} xmlns='' a:{2}='{1}'></{0}></a:container>"};

            foreach (var item in tests)
            {
                foreach (var id in new string[] { "Id", "ID", "id" })
                {
                    var xml = new XmlDocument() { PreserveWhitespace = true };
                    xml.LoadXml(string.Format(item, "element", "value", id));

                    var signed = new SignedXmlWithAgnosticId(xml);
                    var response = signed.GetIdElement(xml, "value");

                    Assert.IsNotNull(response);
                    Assert.IsTrue(response.Attributes.OfType<XmlAttribute>().Any(a => a.LocalName == id && a.Value == "value"));
                }
            }
        }

        [TestMethod]
        public void GetPublicKey()
        {            
            XmlDocument doc = new XmlDocument { PreserveWhitespace = false };
            var path = Path.Combine(TestDataMappe, "1_response_kvittering_for_mottatt_forretningsmelding_fra_meldingsformidler_til_postavsender.xml");
            doc.Load(path);

            var mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            mgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

            // Find key
            var token = doc.SelectSingleNode("//wsse:BinarySecurityToken", mgr);
            var key = new X509Certificate2(System.Convert.FromBase64String(token.InnerText));

            var signed = new SignedXmlWithAgnosticId(doc);
            var signatureNode = (XmlElement)doc.SelectSingleNode("//ds:Signature", mgr);
            signed.LoadXml(signatureNode);            

            var result = typeof(SignedXmlWithAgnosticId).GetMethod("GetPublicKey", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(signed, null) as AsymmetricAlgorithm;

            Assert.AreEqual(result.ToXmlString(false), key.PublicKey.Key.ToXmlString(false));
        }


    }
}
