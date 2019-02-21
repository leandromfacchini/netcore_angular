using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Eventos.IO.Infra.CrossCutting.AspNetFilters
{
    public class GlobalExceptionHandlingFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionHandlingFilter> _logger;

        public GlobalExceptionHandlingFilter(ILogger<GlobalExceptionHandlingFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(1, context.Exception, context.Exception.Message);

            var result = new ViewResult { ViewName = "Error" };

            var modelData = new EmptyModelMetadataProvider();

            result.ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(modelData, context.ModelState)
            {
                {"MensagemErro",context.Exception.Message }
            };
            context.Result = result;
        }
    }
}