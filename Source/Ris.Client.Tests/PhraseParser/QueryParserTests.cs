using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ris.Client.Messages.Request;
using Ris.Client.PhraseParser;

namespace Ris.Client.Tests.PhraseParser
{
    [TestFixture]
    public class QueryParserTests
    {
        [Test]
        public void ParseSimplePhrase()
        {
            SearchExpression expr = QueryParser.Parse("Ehe");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<PhraseSearchExpression>());
        }


    }
}
