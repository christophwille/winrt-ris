using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ris.Client.PhraseParser
{
	public enum TokenKind
	{
		EOF,
		Phrase, Term,
		And, Or, Not,
		OpenParen, CloseParen
	}
	
	public class Token
	{
		public int Offset { get; private set; }
		public TokenKind Kind { get; private set; }
		public string Value { get; private set; }
		
		public Token(TokenKind kind, int offset = -1)
		{
			this.Kind = kind;
			this.Offset = offset;
		}
		
		public Token(string value, bool isPhrase = false, int offset = -1)
		{
			this.Kind = isPhrase ? TokenKind.Phrase : TokenKind.Term;
			this.Value = value;
			this.Offset = offset;
		}
		
		public override string ToString()
		{
			return string.Format("[Token Offset={0}, Kind={1}, Value={2}]", Offset, Kind, Value);
		}
	}
}
