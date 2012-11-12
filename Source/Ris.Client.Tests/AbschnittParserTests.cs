using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Client;

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
        public void NumberOnly(string numberString)
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
        public void Compound(string numberString)
        {
            int nummer;
            string buchstabe;

            bool ok = AbschnittParser.Parse(numberString, out nummer, out buchstabe);

            Assert.That(ok, Is.True);
            Assert.That(nummer, Is.EqualTo(6));
            Assert.That(buchstabe, Is.EqualTo("a"));
        }
    }
}
