using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ris.Client.Messages.Request;
using Ris.Client.PhraseParser;

namespace Ris.Client.Tests.PhraseParser
{
    [TestFixture]
    public class OutputVisitorTests
    {
        [Test]
        public void VisitSimpleUndQuery()
        {
            SearchExpression expr = QueryParser.Parse("Ehe UND Familie");

            var buffer = new StringBuilder();
            var visitor = new OutputVisitor(new StringWriter(buffer));
            visitor.Visit(expr, null);
            var output = buffer.ToString();

            Assert.That(output, Is.EqualTo("Ehe und Familie"));
        }
    }
}
