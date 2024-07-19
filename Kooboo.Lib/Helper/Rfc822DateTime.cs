using System;
using System.Globalization;

namespace Kooboo.Lib.Helper
{

    /// <summary>
    /// Provides methods for converting <see cref="DateTime"/> structures 
    /// to and from the equivalent <a href="http://www.w3.org/Protocols/rfc822/#z28">RFC 822</a> 
    /// string representation.
    /// </summary>
    public class Rfc822DateTime
    {
        //============================================================
        //  Private members
        //============================================================
        #region Private Members
        /// <summary>
        /// Private member to hold array of formats that RFC 822 date-time representations conform to.
        /// </summary>
        private static string[] formats = new string[0];
        /// <summary>
        /// Private member to hold the DateTime format string for representing a DateTime in the RFC 822 format.
        /// </summary>
        private const string format = "ddd, dd MMM yyyy HH:mm:ss K";
        #endregion

        //============================================================
        //  Public Properties
        //============================================================
        #region Rfc822DateTimeFormat
        /// <summary>
        /// Gets the custom format specifier that may be used to represent a <see cref="DateTime"/> in the RFC 822 format.
        /// </summary>
        /// <value>A <i>DateTime format string</i> that may be used to represent a <see cref="DateTime"/> in the RFC 822 format.</value>
        /// <remarks>
        /// <para>
        /// This method returns a string representation of a <see cref="DateTime"/> that utilizes the time zone 
        /// offset (local differential) to represent the offset from Greenwich mean time in hours and minutes. 
        /// The <see cref="Rfc822DateTimeFormat"/> is a valid date-time format string for use 
        /// in the <see cref="DateTime.ToString(String, IFormatProvider)"/> method.
        /// </para>
        /// <para>
        /// The <a href="http://www.w3.org/Protocols/rfc822/#z28">RFC 822</a> Date and Time specification 
        /// specifies that the year will be represented as a two-digit value, but the 
        /// <a href="http://www.rssboard.org/rss-profile#data-types-datetime">RSS Profile</a> recommends that 
        /// all date-time values should use a four-digit year. The <see cref="Rfc822DateTime"/> class 
        /// follows the RSS Profile recommendation when converting a <see cref="DateTime"/> to the equivalent 
        /// RFC 822 string representation.
        /// </para>
        /// </remarks>
        public static string Rfc822DateTimeFormat
        {
            get
            {
                return format;
            }
        }
        #endregion

        #region Rfc822DateTimePatterns
        /// <summary>
        /// Gets an array of the expected formats for RFC 822 date-time string representations.
        /// </summary>
        /// <value>
        /// An array of the expected formats for RFC 822 date-time string representations 
        /// that may used in the <see cref="DateTime.TryParseExact(String, string[], IFormatProvider, DateTimeStyles, out DateTime)"/> method.
        /// </value>
        /// <remarks>
        /// The array of the expected formats that is returned assumes that the RFC 822 time zone 
        /// is represented as or converted to a local differential representation.
        /// </remarks>
        /// <seealso cref="ConvertZoneToLocalDifferential(String)"/>
        public static string[] Rfc822DateTimePatterns
        {
            get
            {
                if (formats.Length > 0)
                {
                    return formats;
                }
                else
                {
                    formats = new string[35];

                    // two-digit day, four-digit year patterns
                    formats[0] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fffffff zzzz";
                    formats[1] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ffffff zzzz";
                    formats[2] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fffff zzzz";
                    formats[3] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ffff zzzz";
                    formats[4] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fff zzzz";
                    formats[5] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ff zzzz";
                    formats[6] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'f zzzz";
                    formats[7] = "ddd',' dd MMM yyyy HH':'mm':'ss zzzz";

                    // two-digit day, two-digit year patterns
                    formats[8] = "ddd',' dd MMM yy HH':'mm':'ss'.'fffffff zzzz";
                    formats[9] = "ddd',' dd MMM yy HH':'mm':'ss'.'ffffff zzzz";
                    formats[10] = "ddd',' dd MMM yy HH':'mm':'ss'.'fffff zzzz";
                    formats[11] = "ddd',' dd MMM yy HH':'mm':'ss'.'ffff zzzz";
                    formats[12] = "ddd',' dd MMM yy HH':'mm':'ss'.'fff zzzz";
                    formats[13] = "ddd',' dd MMM yy HH':'mm':'ss'.'ff zzzz";
                    formats[14] = "ddd',' dd MMM yy HH':'mm':'ss'.'f zzzz";
                    formats[15] = "ddd',' dd MMM yy HH':'mm':'ss zzzz";

                    // one-digit day, four-digit year patterns
                    formats[16] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fffffff zzzz";
                    formats[17] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ffffff zzzz";
                    formats[18] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fffff zzzz";
                    formats[19] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ffff zzzz";
                    formats[20] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fff zzzz";
                    formats[21] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ff zzzz";
                    formats[22] = "ddd',' d MMM yyyy HH':'mm':'ss'.'f zzzz";
                    formats[23] = "ddd',' d MMM yyyy HH':'mm':'ss zzzz";

                    // two-digit day, two-digit year patterns
                    formats[24] = "ddd',' d MMM yy HH':'mm':'ss'.'fffffff zzzz";
                    formats[25] = "ddd',' d MMM yy HH':'mm':'ss'.'ffffff zzzz";
                    formats[26] = "ddd',' d MMM yy HH':'mm':'ss'.'fffff zzzz";
                    formats[27] = "ddd',' d MMM yy HH':'mm':'ss'.'ffff zzzz";
                    formats[28] = "ddd',' d MMM yy HH':'mm':'ss'.'fff zzzz";
                    formats[29] = "ddd',' d MMM yy HH':'mm':'ss'.'ff zzzz";
                    formats[30] = "ddd',' d MMM yy HH':'mm':'ss'.'f zzzz";
                    formats[31] = "ddd',' d MMM yy HH':'mm':'ss zzzz";

                    // Fall back patterns
                    formats[32] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"; // RoundtripDateTimePattern
                    formats[33] = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;
                    formats[34] = DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern;

                    return formats;
                }
            }
        }
        #endregion

        //============================================================
        //  Public Methods
        //============================================================
        #region Parse(string s)
        /// <summary>
        /// Converts the specified string representation of a date and time to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a date and time to convert.</param>
        /// <returns>
        /// A <see cref="DateTime"/> equivalent to the date and time contained in <paramref name="s"/>, 
        /// expressed as <i>Coordinated Universal Time (UTC)</i>.
        /// </returns>
        /// <remarks>
        /// The string <paramref name="s"/> is parsed using formatting information in the <see cref="DateTimeFormatInfo.InvariantInfo"/> object.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is a <b>null</b> reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is an empty string.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not contain a valid RFC 822 string representation of a date and time.</exception>
        public static DateTime Parse(string s)
        {
            //------------------------------------------------------------
            //  Validate parameter
            //------------------------------------------------------------
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }

            DateTime result;
            if (Rfc822DateTime.TryParse(s, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException(String.Format(null, "{0} is not a valid RFC 822 string representation of a date and time.", s));
            }
        }
        #endregion

        #region ConvertZoneToLocalDifferential(string s)
        /// <summary>
        /// Converts the time zone component of an RFC 822 date and time string representation to its local differential (time zone offset).
        /// </summary>
        /// <param name="s">A string containing an RFC 822 date and time to convert.</param>
        /// <returns>A date and time string that uses local differential to describe the time zone equivalent to the date and time contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is a <b>null</b> reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is an empty string.</exception>
        public static string ConvertZoneToLocalDifferential(string s)
        {
            string zoneRepresentedAsLocalDifferential = String.Empty;

            //------------------------------------------------------------
            //  Validate parameter
            //------------------------------------------------------------
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }

            if (s.EndsWith(" UT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" UT") + 1)), "+00:00");
            }
            else if (s.EndsWith(" GMT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" GMT") + 1)), "+00:00");
            }
            else if (s.EndsWith(" EST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" EST") + 1)), "-05:00");
            }
            else if (s.EndsWith(" EDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" EDT") + 1)), "-04:00");
            }
            else if (s.EndsWith(" CST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" CST") + 1)), "-06:00");
            }
            else if (s.EndsWith(" CDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" CDT") + 1)), "-05:00");
            }
            else if (s.EndsWith(" MST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" MST") + 1)), "-07:00");
            }
            else if (s.EndsWith(" MDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" MDT") + 1)), "-06:00");
            }
            else if (s.EndsWith(" PST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" PST") + 1)), "-08:00");
            }
            else if (s.EndsWith(" PDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" PDT") + 1)), "-07:00");
            }
            else if (s.EndsWith(" Z", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" Z") + 1)), "+00:00");
            }
            else if (s.EndsWith(" A", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" A") + 1)), "-01:00");
            }
            else if (s.EndsWith(" M", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" M") + 1)), "-12:00");
            }
            else if (s.EndsWith(" N", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" N") + 1)), "+01:00");
            }
            else if (s.EndsWith(" Y", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(s.Substring(0, (s.LastIndexOf(" Y") + 1)), "+12:00");
            }
            else
            {
                zoneRepresentedAsLocalDifferential = s;
            }

            return zoneRepresentedAsLocalDifferential;
        }
        #endregion

        #region ToString(DateTime utcDateTime)
        /// <summary>
        /// Converts the value of the specified <see cref="DateTime"/> object to its equivalent string representation.
        /// </summary>
        /// <param name="utcDateTime">The Coordinated Universal Time (UTC) <see cref="DateTime"/> to convert.</param>
        /// <returns>A RFC 822 string representation of the value of the <paramref name="utcDateTime"/>.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="utcDateTime"/> object does not represent a <see cref="DateTimeKind.Utc">Coordinated Universal Time (UTC)</see> value.</exception>
        public static string ToString(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("utcDateTime");
            }

            return utcDateTime.ToString(Rfc822DateTime.Rfc822DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }
        #endregion

        #region TryParse(string s, out DateTime result)
        /// <summary>
        /// Converts the specified string representation of a date and time to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a date and time to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time 
        /// contained in <paramref name="s"/>, expressed as <i>Coordinated Universal Time (UTC)</i>, 
        /// if the conversion succeeded, or <see cref="DateTime.MinValue">MinValue</see> if the conversion failed. 
        /// The conversion fails if the s parameter is a <b>null</b> reference (Nothing in Visual Basic), 
        /// or does not contain a valid string representation of a date and time. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><b>true</b> if the <paramref name="s"/> parameter was converted successfully; otherwise, <b>false</b>.</returns>
        /// <remarks>
        /// The string <paramref name="s"/> is parsed using formatting information in the <see cref="DateTimeFormatInfo.InvariantInfo"/> object. 
        /// </remarks>
        public static bool TryParse(string s, out DateTime result)
        {
            //------------------------------------------------------------
            //  Attempt to convert string representation
            //------------------------------------------------------------
            bool wasConverted = false;
            result = DateTime.MinValue;

            if (!String.IsNullOrEmpty(s))
            {
                DateTime parseResult;
                if (DateTime.TryParseExact(Rfc822DateTime.ConvertZoneToLocalDifferential(s), Rfc822DateTime.Rfc822DateTimePatterns, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out parseResult))
                {
                    result = DateTime.SpecifyKind(parseResult, DateTimeKind.Utc);
                    wasConverted = true;
                }
            }

            return wasConverted;
        }
        #endregion
    }


}
