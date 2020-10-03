using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Middleware
{
    public class JsonStringInputFormatter : TextInputFormatter
    {
        public JsonStringInputFormatter() : base()
        {
            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            SupportedMediaTypes.Add(MediaTypeNames.Application.Json);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.ModelType == typeof(string);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (var streamReader = new StreamReader(
                context.HttpContext.Request.Body,
                encoding))
            {
                return await InputFormatterResult.SuccessAsync(
                    (await streamReader.ReadToEndAsync()).Trim('"'));
            }
        }
    }
}
