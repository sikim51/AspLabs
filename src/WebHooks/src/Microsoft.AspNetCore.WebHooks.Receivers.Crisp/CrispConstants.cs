// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.WebHooks
{
    /// <summary>
    /// Well-known names used in Crisp receivers and handlers.
    /// </summary>
    public static class CrispConstants
    {
        /// <summary>
        /// Gets the name of the property in a Crisp WebHook request entity body (formatted as HTML form
        /// URL-encoded data) containing the Crisp event name.
        /// </summary>
        public static string EventBodyPropertyPath => "event";

        /// <summary>
        /// Gets the name of the Crisp WebHook receiver.
        /// </summary>
        public static string ReceiverName => "crisp";

        /// <summary>
        /// Gets the minimum length of the <see cref="KeyQueryParameterName"/> query parameter value and secret key
        /// for Crisp receivers .
        /// </summary>
        public static int KeyParameterMinLength => 32;

        public static string KeyQueryParameterName => "key";
    }
}
