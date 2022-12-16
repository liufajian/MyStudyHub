using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Text.Json;

namespace WebApiDemo.AspNetCustoms
{
    /// <summary>
    /// application/json模型绑定(支持空body，避免报400 error:A non-empty request body is required.)
    /// </summary>
    internal class JsonBodyBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.ModelType.IsValueType)
            {
                return new BinderTypeModelBinder(typeof(EmptyBodyModelBinder));
            }

            return null;
        }

        class EmptyBodyModelBinder : IModelBinder
        {
            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                if (bindingContext.HttpContext.Request.ContentType?.Contains("application/json") != true)
                {
                    return;
                }

                var stream = bindingContext.HttpContext.Request.Body;
                using var reader = new StreamReader(stream);
                var jsonbody = await reader.ReadToEndAsync();
                if (jsonbody.Length > 0)
                {
                    var obj = JsonSerializer.Deserialize(jsonbody, bindingContext.ModelType);
                    bindingContext.Result = ModelBindingResult.Success(obj);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                }
            }
        }
    }
}
