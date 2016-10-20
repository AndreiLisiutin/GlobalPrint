using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalPrint.ClientWeb.Binders
{
    public class FloatModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;

            if (valueResult.AttemptedValue != string.Empty)
            {
                try
                {
                    NumberFormatInfo format = new NumberFormatInfo();
                    format.CurrencyDecimalSeparator = ".";
                    format.CurrencyDecimalDigits = 99;
                    double result = double.Parse(valueResult.AttemptedValue.Replace(",", "."), NumberStyles.Currency | NumberStyles.AllowDecimalPoint, format);

                    //define destination type, in case of Nullable use underlying rype
                    Type destinationType = bindingContext.ModelMetadata.ModelType;
                    destinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
                    
                    return Convert.ChangeType(result, destinationType);
                }
                catch (FormatException e)
                {
                    modelState.Errors.Add(e);
                }
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}