using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.WebClientService
{
    public class DateTimeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            DateTime dt;
            if (DateTime.TryParseExact(text, options.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;

            return base.ConvertFromString(options, text);
        }

        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}
