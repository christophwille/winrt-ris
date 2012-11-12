using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Client;
using Ris.Client.Models;

namespace Ris.Client.Tests
{
    [TestFixture]
    public class AbschnittParserTests
    {
        [TestCase("1")]
        [TestCase(" 1")]
        [TestCase("1     ")]
        [TestCase("1 ")]
        [TestCase(" 1 ")]
        public void BaseNumberOnly(string numberString)
        {
            int nummer;
            string buchstabe;

            bool ok = AbschnittParser.Parse(numberString, out nummer, out buchstabe);

            Assert.That(ok, Is.True);
            Assert.That(nummer, Is.EqualTo(1));
            Assert.That(buchstabe.Length, Is.EqualTo(0));
        }

        [TestCase("6 a")]
        [TestCase("6a")]
        [TestCase(" 6a ")]
        [TestCase("6    a ")]
        public void BaseCompound(string numberString)
        {
            int nummer;
            string buchstabe;

            bool ok = AbschnittParser.Parse(numberString, out nummer, out buchstabe);

            Assert.That(ok, Is.True);
            Assert.That(nummer, Is.EqualTo(6));
            Assert.That(buchstabe, Is.EqualTo("a"));
        }

        [Test]
        public void TestEmptyVonBis()
        {
            var abschnitt = AbschnittParser.Parse("", "", AbschnittTypEnum.Paragraph);

            Assert.That(abschnitt, Is.Null);
        }

        [Test]
        public void TestEmptyVon()
        {
            var abschnitt = AbschnittParser.Parse("", "1a", AbschnittTypEnum.Paragraph);

            Assert.That(abschnitt, Is.Null);
        }

        [Test]
        public void TestVonBis()
        {
            var abschnitt = AbschnittParser.Parse("1", "6c", AbschnittTypEnum.Paragraph);

            Assert.That(abschnitt, Is.Not.Null);
            Assert.That(abschnitt.NummerVon, Is.EqualTo("1"));
            Assert.That(abschnitt.BuchstabeVon, Is.EqualTo(""));
            Assert.That(abschnitt.NummerBis, Is.EqualTo(6));
            Assert.That(abschnitt.BuchstabeBis, Is.EqualTo("c"));
            Assert.That(abschnitt.NummerBisSpecified, Is.True);
        }

        [Test]
        public void TestVonOnly()
        {
            var abschnitt = AbschnittParser.Parse("1", "", AbschnittTypEnum.Paragraph);

            Assert.That(abschnitt, Is.Not.Null);
            Assert.That(abschnitt.NummerVon, Is.EqualTo("1"));
            Assert.That(abschnitt.BuchstabeVon, Is.EqualTo(""));
            Assert.That(abschnitt.BuchstabeBis, Is.EqualTo(""));
            Assert.That(abschnitt.NummerBisSpecified, Is.False);
        }

        [Test]
        public void TestVonBisWithInvalidBis()
        {
            var abschnitt = AbschnittParser.Parse("1", "c", AbschnittTypEnum.Paragraph);

            Assert.That(abschnitt, Is.Null);
        }
    }
}
