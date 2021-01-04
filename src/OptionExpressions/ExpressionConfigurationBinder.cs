using Microsoft.Extensions.Configuration;
using OptionExpressions.Engine;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace OptionExpressions
{
    // This class was adapted from Microsoft.Extensions.Configuration.Binder source code which is licensed under the MIT License.
    // https://github.com/dotnet/runtime/blob/35d70817611f53a5c1ecda5c93eaf969a30ca667/src/libraries/Microsoft.Extensions.Configuration.Binder/src/ConfigurationBinder.cs#L484

    internal sealed class ExpressionConfigurationBinder
    {
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private readonly ExpressionOptions options;

        public ExpressionConfigurationBinder(ExpressionOptions options)
        {
            this.options = options;
        }

        public void BindInstance(object instance, IConfiguration config)
        {
            foreach (PropertyInfo property in instance.GetType().GetProperties(DeclaredOnlyLookup))
            {
                object propertyValue = property.GetValue(instance);
                bool hasSetter = property.SetMethod is not null && property.SetMethod.IsPublic;

                if (propertyValue is null && !hasSetter)
                {
                    continue;
                }

                IConfigurationSection configSection = config?.GetSection(property.Name);

                if (property.PropertyType == typeof(IConfigurationSection))
                {
                    propertyValue = configSection;
                }
                else if (configSection is not null && configSection.GetChildren().Any())
                {
                    propertyValue = property.GetValue(instance) ?? Activator.CreateInstance(property.PropertyType);
                    BindInstance(propertyValue, configSection);
                }
                else
                {
                    string configValue = configSection?.Value;

                    if (IsExpressionEnabled(property) && TryGetExpression(configValue, out string expression))
                    {
                        string result = ExpressionEngine.Evaluate(expression, GetOptionsForProperty(property));
                        if (TryConvertValue(property.PropertyType, result, out object convertedValue, out _))
                        {
                            propertyValue = convertedValue;
                        }
                    }
                    else
                    {
                        if (TryConvertValue(property.PropertyType, configValue, out object convertedValue, out _))
                        {
                            propertyValue = convertedValue;
                        }
                    }
                }

                if (hasSetter)
                {
                    property.SetValue(instance, propertyValue);
                }
            }
        }

        private bool IsExpressionEnabled(PropertyInfo property)
        {
            return
                property.GetCustomAttribute<EnableExpressionsAttribute>() is not null ||
                property.DeclaringType.GetCustomAttribute<EnableExpressionsAttribute>() is not null;
        }

        private ExpressionOptions GetOptionsForProperty(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<EnableExpressionsAttribute>()
                ?? property.DeclaringType.GetCustomAttribute<EnableExpressionsAttribute>();

            return attribute is null ? this.options : attribute.ApplyTo(this.options);
        }

        private bool TryGetExpression(string value, out string expression)
        {
            string prefix = this.options.Prefix ?? string.Empty;
            string suffix = this.options.Suffix ?? string.Empty;

            if (value is not null && value.StartsWith(prefix) && value.EndsWith(suffix))
            {
                expression = value.Substring(prefix.Length, value.Length - suffix.Length - prefix.Length);
                return true;
            }
            else
            {
                expression = null;
                return false;
            }
        }

        private bool TryConvertValue(Type type, string value, out object result, out Exception error)
        {
            error = null;
            result = null;

            if (type == typeof(object))
            {
                result = value;
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }

                return TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                return true;
            }

            if (type == typeof(byte[]))
            {
                try
                {
                    result = Convert.FromBase64String(value);
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                return true;
            }

            return false;
        }
    }
}