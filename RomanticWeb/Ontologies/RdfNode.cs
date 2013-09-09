using System;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    public class RdfNode
    {
        private string _literal;
        private string _language;
        private Uri _dataType;
        private Uri _uri;

        private RdfNode() { }

        public bool IsUri
        {
            get { return _uri != null; }
        }

        public bool IsLiteral
        {
            get { return _literal != null; }
        }

        public Uri Uri
        {
            get
            {
                if (IsLiteral)
                {
                    throw new InvalidOperationException("Literal node does not have a Uri");
                }

                return _uri;
            }
            private set { _uri = value; }
        }

        public string Literal
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a literal value");
                }

                return _literal;
            }
            private set { _literal = value; }
        }

        [AllowNull]
        public Uri DataType
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a data type");
                }

                return _dataType;
            }
            set { _dataType = value; }
        }

        [AllowNull]
        public string Language
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a language tag");
                }

                return _language;
            }
            set { _language = value; }
        }

        public override string ToString()
        {
            if (IsLiteral)
            {
                return Literal;
            }
            if(IsUri)
            {
                return Uri.ToString();
            }

            throw new InvalidOperationException();
        }

        public static RdfNode ForUri(Uri uri)
        {
            return new RdfNode
                {
                    Uri = uri
                };
        }

        public static RdfNode ForLiteral(string value, [AllowNull] string language, [AllowNull] Uri dataType)
        {
            return new RdfNode
                {
                    Literal = value,
                    Language = language,
                    DataType = dataType
                };
        }
    }
}