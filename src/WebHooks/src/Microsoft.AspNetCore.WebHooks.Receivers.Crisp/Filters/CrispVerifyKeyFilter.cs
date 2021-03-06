// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebHooks.Metadata;
using Microsoft.AspNetCore.WebHooks.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.WebHooks.Filters
{
    /// <summary>
    /// An <see cref="IResourceFilter"/> that verifies the <c>key</c> query parameter. Short-circuits the request if
    /// the <c>key</c> query parameter is missing or does not match the receiver's configuration. Also confirms the
    /// request URI uses the <c>HTTPS</c> scheme.
    /// </summary>
    public class CrispVerifyKeyFilter : WebHookSecurityFilter, IResourceFilter, IWebHookReceiver
    {


        /// <summary>
        /// Instantiates a new <see cref="CrispVerifyKeyFilter"/> instance to verify the receiver's
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to initialize <see cref="WebHookSecurityFilter.Configuration"/>.
        /// </param>
        /// <param name="hostingEnvironment">
        /// The <see cref="IHostingEnvironment" /> used to initialize
        /// <see cref="WebHookSecurityFilter.HostingEnvironment"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The <see cref="ILoggerFactory"/> used to initialize <see cref="WebHookSecurityFilter.Logger"/>.
        /// </param>
        /// <remarks>This overload is intended for use with <see cref="GeneralWebHookAttribute"/>.</remarks>
        public CrispVerifyKeyFilter(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
            : base(configuration, hostingEnvironment, loggerFactory)
        {
        }


        /// <inheritdoc />
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var routeData = context.RouteData;

            var result = EnsureValidKey(context.HttpContext.Request, routeData, CrispConstants.ReceiverName);
            if (result != null)
            {
                context.Result = result;
            }
        }

        public string ReceiverName => CrispConstants.ReceiverName;

        public bool IsApplicable(string receiverName)
        {

            if (receiverName == null)
            {
                throw new ArgumentNullException(nameof(receiverName));
            }

            return string.Equals(ReceiverName, receiverName, StringComparison.OrdinalIgnoreCase);

        }

        /// <inheritdoc />
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // No-op
        }

        /// <summary>
        /// For WebHook providers with insufficient security considerations, the receiver can require that the WebHook
        /// URI must be an <c>https</c> URI and contain a 'key' query parameter with a value configured for that
        /// particular <c>id</c>. A sample WebHook URI is
        /// '<c>https://{host}/api/webhooks/incoming/{receiver name}?key=83699ec7c1d794c0c780e49a5c72972590571fd8</c>'.
        /// The 'key' parameter must be between 32 and 128 characters long.
        /// </summary>
        /// <param name="request">The current <see cref="HttpRequest"/>.</param>
        /// <param name="routeData">
        /// The <see cref="RouteData"/> for this request. A (potentially empty) ID value in this data allows a
        /// <see cref="CrispVerifyKeyFilter"/> to support multiple senders with individual configurations.
        /// </param>
        /// <param name="receiverName">The name of an available <see cref="IWebHookReceiver"/>.</param>
        /// <returns>
        /// <see langword="null"/> in the success case. When a check fails, an <see cref="IActionResult"/> that when
        /// executed will produce a response containing details about the problem.
        /// </returns>
        protected virtual IActionResult EnsureValidKey(
            HttpRequest request,
            RouteData routeData,
            string receiverName)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (routeData == null)
            {
                throw new ArgumentNullException(nameof(routeData));
            }
            if (receiverName == null)
            {
                throw new ArgumentNullException(nameof(receiverName));
            }

            var result = EnsureSecureConnection(receiverName, request);
            if (result != null)
            {
                return result;
            }

            var key = request.Query[CrispConstants.KeyQueryParameterName];
            if (StringValues.IsNullOrEmpty(key))
            {
                Logger.LogWarning(
                    400,
                    $"A '{receiverName}' WebHook verification request must contain a " +
                    $"'{CrispConstants.KeyQueryParameterName}' query " +
                    "parameter.",
                    receiverName);

                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    $"A '{ReceiverName}' WebHook request must contain a '{CrispConstants.KeyQueryParameterName}' query parameter.",
                    receiverName,
                    CrispConstants.KeyQueryParameterName);
                var noKey = new BadRequestObjectResult(message);

                return noKey;
            }

            var secretKey = GetSecretKey(receiverName, routeData, CrispConstants.KeyParameterMinLength);
            if (secretKey == null)
            {
                return new NotFoundResult();
            }

            if (!SecretEqual(key, secretKey))
            {
                Logger.LogWarning(
                    401,
                    $"The '{CrispConstants.KeyQueryParameterName}' query parameter provided in the HTTP request " +
                    "did not match the expected value.");

                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "The '{0}' query parameter provided in the HTTP request did not match the expected value.",
                    CrispConstants.KeyQueryParameterName);
                var invalidKey = new BadRequestObjectResult(message);

                return invalidKey;
            }

            return null;
        }


    }
}
