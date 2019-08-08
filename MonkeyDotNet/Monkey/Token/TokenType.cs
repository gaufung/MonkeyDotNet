namespace Monkey.Token
{
    /// <summary>
    /// Token type enumeration
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// illegal character
        /// </summary>
        ILLEGAL,

        /// <summary>
        /// eof identifier
        /// </summary>
        EOF,

        /// <summary>
        /// identifier
        /// </summary>
        IDENT,

        /// <summary>
        /// integer
        /// </summary>
        INT,

        /// <summary>
        /// assignment “=”
        /// </summary>
        ASSIGN,

        /// <summary>
        /// plus “+”
        /// </summary>
        PLUS,

        /// <summary>
        /// comma ","
        /// </summary>
        COMMA,

        /// <summary>
        /// semicolon ";"
        /// </summary>
        SEMICOLON,

        /// <summary>
        /// minus "-"
        /// </summary>
        MINUS,

        /// <summary>
        /// bang "!"
        /// </summary>
        BANG,

        /// <summary>
        /// atersik "*"
        /// </summary>
        ATERISK,

        /// <summary>
        /// slash "/"
        /// </summary>
        SLASH,

        /// <summary>
        /// less than "&lt;"
        /// </summary>
        LT,

        /// <summary>
        /// great than "&gt;"
        /// </summary>
        GT,

        /// <summary>
        /// left parenthess "("
        /// </summary>
        LPAREN,

        /// <summary>
        /// right parenthess ")"
        /// </summary>
        RPAREN,

        /// <summary>
        /// left brace "{"
        /// </summary>
        LBRACE,

        /// <summary>
        /// right brace "}"
        /// </summary>
        RBRACE,

        /// <summary>
        /// function "fn"
        /// </summary>
        FUNCTION,

        /// <summary>
        /// let "let"
        /// </summary>
        LET,

        /// <summary>
        /// true "true"
        /// </summary>
        TRUE,

        /// <summary>
        /// false "false"
        /// </summary>
        FALSE,

        /// <summary>
        /// if "if"
        /// </summary>
        IF,

        /// <summary>
        /// else "else"
        /// </summary>
        ELSE,

        /// <summary>
        /// return "return"
        /// </summary>
        RETURN,

        /// <summary>
        /// for "for"
        /// </summary>
        FOR,

        /// <summary>
        /// equal "=="
        /// </summary>
        EQ,

        /// <summary>
        /// not equal "!="
        /// </summary>
        NOT_EQ,

        /// <summary>
        /// string """
        /// </summary>
        STRING,

        /// <summary>
        /// left bracket "["
        /// </summary>
        LBRACKET,

        /// <summary>
        /// right bracket "]"
        /// </summary>
        RBRACKET,

        /// <summary>
        /// colon ":"
        /// </summary>
        COLON,

    }
}