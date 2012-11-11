using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ris.Client.Messages.Request;

namespace Ris.Client.PhraseParser
{
	public class QueryParser
	{
		QueryTokenizer tokenizer;
		
		QueryParser(string input)
		{
			this.tokenizer = new QueryTokenizer(input);
			la = tokenizer.Next();
		}
		
		Token t, la;
		
		void Get()
		{
			t = la;
			la = tokenizer.Next();
		}
		
		/// Root ::= AndExpressionList | ListSearchExpression
		SearchExpression Parse()
		{
			var op = ListOrSearchExpression();
			List<SearchExpression> expr = null;
			if (la.Kind == TokenKind.And) {
				OperatorList(op, ref expr, TokenKind.And);
				op = new AndSearchExpression() { Expressions = expr.ToArray() };
			} else if (la.Kind == TokenKind.Or) {
				OperatorList(op, ref expr, TokenKind.Or);
				op = new OrSearchExpression() { Expressions = expr.ToArray() };
			}
			Expect(TokenKind.EOF);
			return op;
		}
		
		/// ListOrSearchExpression ::= NotSearchExpression | ListSearchExpression
		SearchExpression ListOrSearchExpression()
		{
			if (la.Kind == TokenKind.OpenParen)
				return ListSearchExpression();
			return NotSearchExpression();
		}
		
		/// AndExpressionList ::= ListOrSearchExpression { "UND" ListOrSearchExpression }
		SearchExpression AndExpressionList()
		{
			var op1 = ListOrSearchExpression();
			List<SearchExpression> expr = null;
			OperatorList(op1, ref expr, TokenKind.And);
			if (expr != null) {
				return new AndSearchExpression() { Expressions = expr.ToArray() };
			}
			return op1;
		}
		
		void OperatorList(SearchExpression operand, ref List<SearchExpression> expr, TokenKind op)
		{
			while (la.Kind == op) {
				Get();
				if (expr == null) {
					expr = new List<SearchExpression>();
					expr.Add(operand);
				}
				expr.Add(ListOrSearchExpression());
			}
		}
		
		/// OrExpressionList ::= ListOrSearchExpression { "ODER" ListOrSearchExpression }
		SearchExpression OrExpressionList()
		{
			var op1 = ListOrSearchExpression();
			List<SearchExpression> expr = null;
			OperatorList(op1, ref expr, TokenKind.Or);
			if (expr != null) {
				return new OrSearchExpression() { Expressions = expr.ToArray() };
			}
			return op1;
		}
		
		/// ListSearchExpression ::= "(" ( AndExpressionList | OrExpressionList ) ")"
		SearchExpression ListSearchExpression()
		{
			Expect(TokenKind.OpenParen);
			var op = ListOrSearchExpression();
			List<SearchExpression> expr = null;
			if (la.Kind == TokenKind.And) {
				OperatorList(op, ref expr, TokenKind.And);
				op = new AndSearchExpression() { Expressions = expr.ToArray() };
			} else if (la.Kind == TokenKind.Or) {
				OperatorList(op, ref expr, TokenKind.Or);
				op = new OrSearchExpression() { Expressions = expr.ToArray() };
			}
			Expect(TokenKind.CloseParen);
			return op;
		}
		
		/// NotSearchExpression ::= [ "NICHT" ] PhraseSearchExpression
		SearchExpression NotSearchExpression()
		{
			if (la.Kind == TokenKind.Not) {
				Get();
				return new NotSearchExpression() { Expression = PhraseSearchExpression() };
			}
			return PhraseSearchExpression();
		}
		
		/// PhraseSearchExpression ::= PHRASE
		SearchExpression PhraseSearchExpression()
		{
			if (la.Kind == TokenKind.Phrase) {
				Get();
				return new PhraseSearchExpression() { Value = t.Value };
			}
			if (la.Kind == TokenKind.Term) {
				Get();
                return new TermSearchExpression() { Value = t.Value };
			}
			throw new ParseException("parse error: TERM or PHRASE expected!");
		}
		
		void Expect(TokenKind kind)
		{
			if (kind != la.Kind)
				throw new ParseException("parse error: " + kind + " expected");
			Get();
		}
		
		public static SearchExpression Parse(string input)
		{
			QueryParser parser = new QueryParser(input);
			return parser.Parse();
		}
	}
}
