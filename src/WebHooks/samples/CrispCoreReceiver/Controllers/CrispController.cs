using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;

namespace CrispCoreReceiver.Controllers
{
    public class CrispController : ControllerBase
    {
        private readonly ILogger _logger;

        public CrispController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CrispController>();
        }

        [CrispWebHook(Id = "It")]
        public IActionResult CrispForIt(string @event, CrispRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
               0,
               $"{nameof(CrispController)} / 'It' received event '{{EventName}}'.",
               @event);

            // foreach (var keyValuePair in data)
            // {
            //     if (string.Equals(
            //         CrispConstants.EventBodyPropertyPath,
            //         keyValuePair.Key,
            //         StringComparison.OrdinalIgnoreCase))
            //     {
            //         continue;
            //     }

            //     _logger.LogInformation(
            //         1,
            //         "{FieldName}: {FieldValue}",
            //         keyValuePair.Key,
            //         keyValuePair.Value.ToString());
            // }

            return Ok();
        }

        [CrispWebHook]
        public IActionResult Crisp(string id, string @event, CrispRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
               2,
               $"{nameof(CrispController)} / '{{ReceiverId}}' received event " +
               "'{EventName}'.",
               id,
               @event);


            return Ok();
        }
    }
}
