// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using Ris.Client.Messages.Request;

namespace Ris.Client.PhraseParser
{
	public class OutputVisitor : AbstractRisVisitor<object, object>
	{
		TextWriter writer;
		
		public OutputVisitor(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
		}
		
		public override object Visit(TermSearchExpression expr, object data)
		{
			writer.Write(expr.Value);
			return null;
		}
		
		public override object Visit(PhraseSearchExpression expr, object data)
		{
			writer.Write("'" + expr.Value + "'");
			return null;
		}
		
		public override object Visit(NotSearchExpression expr, object data)
		{
			writer.Write("nicht ");
			if (expr.Expression is TermSearchExpression || expr.Expression is PhraseSearchExpression)
				return Visit(expr.Expression, data);
			writer.Write("(");
			Visit(expr.Expression, data);
			writer.Write(")");
			return null;
		}
		
		public override object Visit(AndSearchExpression expr, object data)
		{
			bool first = true;
			foreach (var e in expr.Expressions) {
				if (!first) writer.Write(" und ");
				else first = false;
				if (e is OrSearchExpression)
					writer.Write("(");
				Visit(e, data);
				if (e is OrSearchExpression)
					writer.Write(")");
			}
			return null;
		}
		
		public override object Visit(OrSearchExpression expr, object data)
		{
			bool first = true;
			foreach (var e in expr.Expressions) {
				if (!first) writer.Write(" oder ");
				else first = false;
				if (e is AndSearchExpression)
					writer.Write("(");
				Visit(e, data);
				if (e is AndSearchExpression)
					writer.Write(")");
			}
			return null;
		}
	}
}
