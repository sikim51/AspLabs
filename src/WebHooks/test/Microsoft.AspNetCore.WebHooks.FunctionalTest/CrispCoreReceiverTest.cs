// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using CrispCoreReceiver;
using Xunit;

namespace Microsoft.AspNetCore.WebHooks.FunctionalTest
{
    public class CrispCoreReceiverTest : IClassFixture<WebHookTestFixture<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebHookTestFixture<Startup> _fixture;

        public CrispCoreReceiverTest(WebHookTestFixture<Startup> fixture)
        {
            _client = fixture.CreateClient();
            _fixture = fixture;
        }

        [Fact]
        public async Task HomePage_IsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public static TheoryData<HttpMethod> NonPostDataSet
        {
            get
            {
                return new TheoryData<HttpMethod>
                {
                    HttpMethod.Get,
                    HttpMethod.Head,
                    HttpMethod.Put,
                };
            }
        }

        [Theory]
        [MemberData(nameof(NonPostDataSet))]
        public async Task WebHookAction_NonPost_IsNotAllowed(HttpMethod method)
        {
            // Arrange
            var expectedErrorMessage = "The 'crisp' WebHook receiver does not support the HTTP " +
                $"'{method.Method}' method.";
            var request = new HttpRequestMessage(
                method,
                "/api/webhooks/incoming/crisp?key=01234567890123456789012345678901");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedErrorMessage, responseText);
        }

        [Fact]
        public async Task WebHookAction_NoKey_IsBadRequest()
        {
            // Arrange
            var expectedErrorMessage = "A 'crisp' WebHook request must contain a 'key' query parameter.";
            var content = new StringContent(string.Empty);

            // Act
            var response = await _client.PostAsync("/api/webhooks/incoming/crisp", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedErrorMessage, responseText);
        }

        [Fact]
        public async Task WebHookAction_WrongKey_IsBadRequest()
        {
            // Arrange
            var expectedErrorMessage = "The 'key' query parameter provided in the HTTP request did not match the " +
                "expected value.";
            var content = new StringContent(string.Empty);

            // Act
            var response = await _client.PostAsync(
                // One changed character in code query parameter.
                "/api/webhooks/incoming/crisp?key=01234567890123456789012345678902",
                content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedErrorMessage, responseText);
        }

        [Fact]
        public async Task WebHookAction_NoBody_IsBadRequest()
        {
            // Arrange
            var expectedErrorMessage = "The 'crisp' WebHook receiver does not support an empty request body.";
            var content = new StringContent(string.Empty);

            // Act
            var response = await _client.PostAsync(
                "/api/webhooks/incoming/crisp?key=01234567890123456789012345678901",
                content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedErrorMessage, responseText);
        }

        [Fact]
        public async Task WebHookAction_WithBody_Succeeds()
        {
            // Arrange
            var fixture = _fixture.WithTestLogger(out var testSink);
            var client = fixture.CreateClient();

            var path = Path.Combine("Resources", "RequestBodies", "Crisp.json");
            var stream = await ResourceFile.GetResourceStreamAsync(path, normalizeLineEndings: true);
            var content = new StreamContent(stream)
            {
                Headers =
                {
                    { HeaderNames.ContentLength, stream.Length.ToString() },
                    { HeaderNames.ContentType, "application/json" },
                },
            };

            // Act
            var response = await client.PostAsync(
                "/api/webhooks/incoming/crisp?key=01234567890123456789012345678901",
                content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseText);
        }
    }
}
