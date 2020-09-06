// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.WebHooks.Filters;

namespace Microsoft.AspNetCore.WebHooks.Metadata
{
    /// <summary>
    /// An <see cref="IWebHookMetadata"/> service containing metadata about the Crisp receiver.
    /// </summary>
    public class CrispMetadata :
        WebHookMetadata,
        IWebHookEventFromBodyMetadata,
        IWebHookFilterMetadata
    {
        private readonly CrispVerifyKeyFilter _verifyKeyFilter;

        /// <summary>
        /// Instantiates a new <see cref="CrispMetadata"/> instance.
        /// </summary>
        public CrispMetadata(CrispVerifyKeyFilter verifyKeyFilter)
            : base(CrispConstants.ReceiverName)
        {
            this._verifyKeyFilter = verifyKeyFilter;
        }

        // IWebHookBodyTypeMetadataService...

        /// <inheritdoc />
        public override WebHookBodyType BodyType => WebHookBodyType.Json;

        // IWebHookEventFromBodyMetadata...

        /// <inheritdoc />
        public bool AllowMissing => false;

        /// <inheritdoc />
        public string BodyPropertyPath => CrispConstants.EventBodyPropertyPath;

        // IWebHookFilterMetadata...

        /// <inheritdoc />
        public void AddFilters(WebHookFilterMetadataContext context)
        {
            context.Results.Add(_verifyKeyFilter);
        }
    }
}
