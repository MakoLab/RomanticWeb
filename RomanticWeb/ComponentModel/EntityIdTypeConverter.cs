using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb.ComponentModel
{
    /// <summary>
    /// <see cref="TypeConverter"/> between <see cref="EntityId"/> and <see cref="Uri"/>
    /// </summary>
    public class EntityIdTypeConverter<TEntityId> : TypeConverter
    {
        #region Fields
        private static readonly UriTypeConverter UriTypeConverter;
        #endregion

        #region Constructors
        static EntityIdTypeConverter()
        {
            UriTypeConverter = (UriTypeConverter)TypeDescriptor.GetConverter(typeof(Uri));
        }
        #endregion

        #region Public methods
        /// <summary>Returns whether this converter can convert an object of the given type to the type of this converter.</summary>
        /// <param name="context">Type: <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">Type: <see cref="System.Type" />
        /// A <see cref="System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
        public override bool CanConvertFrom([AllowNull] ITypeDescriptorContext context, Type sourceType)
        {
            return (UriTypeConverter.CanConvertFrom(context, sourceType)) || (base.CanConvertFrom(context, sourceType));
        }

        /// <summary>Returns whether this converter can convert the object to the specified type, using the specified context.</summary>
        /// <param name="context">Type: <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">Type: <see cref="System.Type" />
        /// A <see cref="System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
        public override bool CanConvertTo([AllowNull] ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) || (destinationType == typeof(Uri)) || (base.CanConvertTo(context, destinationType));
        }

        /// <summary>Converts the given object to the type of this converter, using the specified context and culture information.</summary>
        /// <param name="context">Type: <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">Type: <see cref="System.Globalization.CultureInfo" />
        /// The <see cref="System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">Type: <see cref="System.Object" />
        /// The <see cref="System.Object" /> to convert.</param>
        /// <returns>Type: <see cref="System.Object" />
        /// An <see cref="System.Object" /> that represents the converted value.</returns>
        public override object ConvertFrom([AllowNull] ITypeDescriptorContext context, [AllowNull] CultureInfo culture, [AllowNull] object value)
        {
            object result;
            if (value != null)
            {
                var uri = (Uri)UriTypeConverter.ConvertFrom(context, culture, value);
                if (uri != null)
                {
                    result = CreateEntityId(uri);
                }
                else
                {
                    result = base.ConvertFrom(context, culture, value);
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
        /// <param name="context">Type: <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">Type: <see cref="System.Globalization.CultureInfo" />
        /// A <see cref="System.Globalization.CultureInfo" />. If <b>null</b> is passed, the current culture is assumed.</param>
        /// <param name="value">Type: <see cref="System.Object" />
        /// The <see cref="System.Object" /> to convert.</param>
        /// <param name="destinationType">Type: <see cref="System.Type" />
        /// The <see cref="System.Type" /> to convert the value parameter to.</param>
        /// <returns>Type: <see cref="System.Object" />
        /// An <see cref="System.Object" /> that represents the converted value.</returns>
        public override object ConvertTo([AllowNull] ITypeDescriptorContext context, [AllowNull] CultureInfo culture, [AllowNull] object value, Type destinationType)
        {
            object result = null;
            if ((value is EntityId) && ((destinationType == typeof(string)) || (destinationType == typeof(Uri))))
            {
                if (destinationType == typeof(string))
                {
                    result = ((EntityId)value).Uri.ToString();
                }
                else if (destinationType == typeof(Uri))
                {
                    result = new Uri(((EntityId)value).Uri.ToString(), UriKind.RelativeOrAbsolute);
                }
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }

        /// <summary>Returns whether the given value object is valid for this type and for the specified context.</summary>
        /// <param name="context">Type: <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="value">Type: <see cref="System.Object" />
        /// The <see cref="System.Object" /> to test for validity.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified value is valid for this object; otherwise <b>false</b>.</returns>
        public override bool IsValid([AllowNull] ITypeDescriptorContext context, [AllowNull] object value)
        {
            return ((value is EntityId) && (UriTypeConverter.IsValid(((EntityId)value).Uri))) || (base.IsValid(context, value));
        }
        #endregion

        #region Non-public methods
        /// <summary>
        /// Tries to create a <typeparamref name="TEntityId"/> using a <see cref="Uri"/> constructor
        /// </summary>
        protected virtual TEntityId CreateEntityId(Uri uri)
        {
            ConstructorInfo constructorInfo = typeof(TEntityId).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Uri) }, null);
            if (constructorInfo == null)
            {
                throw new NotImplementedException(String.Format(
                    "Type '{0}' does not implement constructor that accepts single argument of type '{1}'.",
                    typeof(TEntityId),
                    typeof(Uri)));
            }

            return (TEntityId)constructorInfo.Invoke(new object[] { uri });
        }
        #endregion
    }
}