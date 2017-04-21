using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.WebClientService
{
    public class TimeSpanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (TimeSpan.TryParseExact(text, options.Format, CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan ts))
                return ts;
            else
            {
                var hour = text.Substring(0, 2);
                if (int.TryParse(hour, out int hourInt))
                {
                    if (hourInt >= 24)
                    {
                        hourInt = hourInt - 24;
                        text = text.Remove(0, 2);
                        text = '0' + hourInt.ToString() + text;
                        if (TimeSpan.TryParseExact(text, options.Format, CultureInfo.InvariantCulture, TimeSpanStyles.None, out ts))
                            return ts;
                    }
                }
            }

            return base.ConvertFromString(options, text);
        }

        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}
