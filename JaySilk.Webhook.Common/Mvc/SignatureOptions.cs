using System;
using System.Collections.Generic;
using System.Linq;

namespace JaySilk.Webhook.Common.Mvc
{

    public class SignatureOptions
    {

        // TODO: Not sure I like SignatureHeaderName or HeaderValueTransformer here, they are more
        // internal use for the derived classes which would pass them in to the base. The values
        // are very specific to the implementation of the signatures the derived classes support
        // Secret is a user provided option and needs to be exposed

        public string SignatureHeaderName { get; set; }
        public string Secret { get; set; }
        public Func<string, string> HeaderValueTransformer { get; set; }

    }
}
