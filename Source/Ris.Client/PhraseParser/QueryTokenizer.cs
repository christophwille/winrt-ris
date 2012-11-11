using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ris.Client.PhraseParser
{
    public class QueryTokenizer
    {
        LATextReader reader;
        
        public QueryTokenizer(string input)
        {
            this.reader = new LATextReader(new StringReader(input));
        }
        
        Token last = null, savedToken = null;
        
        public Token Next()
        {
            if (savedToken != null) {
                Token s = savedToken;
                savedToken = null;
                return s;
            }
            
            Token t = NextInternal();
            
            if (last != null && IsElementOrExprStart(t) && IsElement(last)) {
                savedToken = t;
                last = t;
                return new Token(TokenKind.And, t.Offset - 1);
            }
            last = t;
            return t;
        }
        
        bool IsElement(Token t)
        {
            return t.Kind == TokenKind.Term || t.Kind == TokenKind.Phrase;
        }
        
        bool IsElementOrExprStart(Token t)
        {
            return IsElement(t) || t.Kind == TokenKind.OpenParen || t.Kind == TokenKind.Not;
        }
        
        readonly char[] tokens = new char[] { '(', ')', '\'' };
        
        int offset = 0;
        int start;
        StringBuilder builder = new StringBuilder();
        
        Token NextInternal()
        {
            int c = reader.Peek();
            while (c > -1 && char.IsWhiteSpace((char)c)) {
                reader.Read();
                c = reader.Peek();
                offset++;
            }
            
            if (c > -1) {
                char ch = (char)c;
                switch (ch) {
                    case '(':
                        reader.Read();
                        return new Token(TokenKind.OpenParen, offset++);
                    case ')':
                        reader.Read();
                        return new Token(TokenKind.CloseParen, offset++);
                    case '\'':
                        builder = new StringBuilder(); // builder.Clear();
                        reader.Read();
                        start = offset;
                        offset++;
                        c = reader.Peek();
                        while (c > -1 && ((char)c) != '\'') {
                            builder.Append((char)c);
                            reader.Read();
                            c = reader.Peek();
                            offset++;
                        }
                        if (c == -1) {
                            Error("unexpected end of expression", offset);
                            return new Token(builder.ToString(), true, start);
                        } else {
                            reader.Read();
                            offset++;
                            return new Token(builder.ToString(), true, start);
                        }
                    default:
                        builder = new StringBuilder(); // builder.Clear();
                        start = offset;
                        while (c > -1 && !(Array.IndexOf(tokens, (char)c) > -1 || char.IsWhiteSpace((char)c))) {
                            builder.Append((char)c);
                            reader.Read();
                            c = reader.Peek();
                            offset++;
                        }
                        string result = builder.ToString();
                        if (result.Equals("und", StringComparison.OrdinalIgnoreCase))
                            return new Token(TokenKind.And, start);
                        if (result.Equals("oder", StringComparison.OrdinalIgnoreCase))
                            return new Token(TokenKind.Or, start);
                        if (result.Equals("nicht", StringComparison.OrdinalIgnoreCase))
                            return new Token(TokenKind.Not, start);
                        return new Token(result, offset: start);
                }
            }
            
            return new Token(TokenKind.EOF, offset);
        }
        
        void Error(string text, int offset)
        {
            throw new ParseException(string.Format("parse error: {0} at {1}", text, offset));
        }
    }
}
