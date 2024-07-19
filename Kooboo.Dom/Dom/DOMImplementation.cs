//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    [Serializable]
    public class DOMImplementation
    {

        //http://www.w3.org/TR/dom/#domimplementation

        public DocumentType createDocumentType(string qualifiedName, string publicId, string systemId)
        {
            DocumentType doctype = new DocumentType();
            doctype.name = qualifiedName;
            doctype.publicId = publicId;
            doctype.systemId = systemId;

            return doctype;

        }

        public XmlDocument createDocument(string @namespace, string qualifiedName, DocumentType doctype)
        {
            throw new NotImplementedException();
        }

        public Document createHTMLDocument(string title)
        {
            throw new NotImplementedException();
        }

        public bool hasFeature(string featurename)
        {
            throw new NotImplementedException();
        }

    }
}
