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
        public void ParseSimpleTerm()
        {
            SearchExpression expr = QueryParser.Parse("Ehe");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<TermSearchExpression>());
            Assert.That(((TermSearchExpression)expr).Value, Is.EqualTo("Ehe"));
        }

        [Test]
        public void ParseSimplePhrase()
        {
            SearchExpression expr = QueryParser.Parse("'Ehe Recht'");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<PhraseSearchExpression>());
            Assert.That(((PhraseSearchExpression)expr).Value, Is.EqualTo("Ehe Recht"));
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void ParseSimpleIncompleteSearch()
        {
            SearchExpression expr = QueryParser.Parse("Ehe und");
        }

        [Test]
        public void ParseImplicitUnd()
        {
            SearchExpression expr = QueryParser.Parse("Ehe Recht");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<AndSearchExpression>());
            Assert.That(((AndSearchExpression)expr).Expressions.Length, Is.EqualTo(2));
        }

        [Test]
        public void ParseExplicitUnd()
        {
            SearchExpression expr = QueryParser.Parse("Ehe UnD Recht");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<AndSearchExpression>());
            Assert.That(((AndSearchExpression)expr).Expressions.Length, Is.EqualTo(2));
        }

        [Test]
        public void ParseOder()
        {
            SearchExpression expr = QueryParser.Parse("Ehe oDeR Recht");

            Assert.That(expr, Is.Not.Null);
            Assert.That(expr, Is.InstanceOf<OrSearchExpression>());
            Assert.That(((OrSearchExpression)expr).Expressions.Length, Is.EqualTo(2));
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void FailParseOpenParenOnly()
        {
            SearchExpression expr = QueryParser.Parse("(");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void FailParseCloseParenOnly()
        {
            SearchExpression expr = QueryParser.Parse(")");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void FailIncompleteParenStatement()
        {
            SearchExpression expr = QueryParser.Parse("(Ehe Kinder");
        }
    }
}
