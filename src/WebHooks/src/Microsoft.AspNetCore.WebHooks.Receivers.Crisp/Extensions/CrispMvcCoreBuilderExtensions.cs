// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up Crisp WebHooks in an <see cref="IMvcCoreBuilder" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CrispMvcCoreBuilderExtensions
    {
        /// <summary>
        /// <para>
        /// Add Crisp WebHook configuration and services to the specified <paramref name="builder"/>. See
        /// <see href="https://help.crisp.chat/en/article/how-to-use-webhooks-itsagz/"/> for additional details about Crisp WebHook
        /// requests.
        /// </para>
        /// <para>
        /// The '<c>WebHooks:Crisp:SecretKey:default</c>' configuration value contains the secret key for Crisp
        /// WebHook URIs of the form '<c>https://{host}/api/webhooks/incoming/crisp?key={secret key}</c>'.
        /// '<c>WebHooks:Crisp:SecretKey:{id}</c>' configuration values contain secret keys for
        /// Crisp WebHook URIs of the form
        /// '<c>https://{host}/api/webhooks/incoming/crisp/{id}?key={secret key}</c>'.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder" /> to configure.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        public static IMvcCoreBuilder AddCrispWebHooks(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            CrispServiceCollectionSetup.AddCrispServices(builder.Services);

            return builder
                .AddJsonFormatters()
                .AddWebHooks();
        }
    }
}
