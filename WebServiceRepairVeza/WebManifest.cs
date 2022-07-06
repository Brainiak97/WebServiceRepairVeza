using Newtonsoft.Json;
using WebEssentials.AspNetCore.Pwa;

namespace WebService
{
    public class WebManifest
    {
        //

        //     The absolute file path to Web App Manifest file.
        [JsonIgnore]
        public string? FileName
        {
            get;
            internal set;
        }

        //

        //     The raw JSON from the manifest file.
        [JsonIgnore]
        public string? RawJson
        {
            get;
            internal set;
        }

        //

        //     A name for use in the Web App Install banner.
        [JsonProperty("name")]
        public string? Name
        {
            get;
            set;
        }

        //

        //     A short_name for use as the text on the users home screen.
        [JsonProperty("short_name")]
        public string? ShortName
        {
            get;
            set;
        }

        //

        //     Provides a general description of what the web application does.
        [JsonProperty("description")]
        public string? Description
        {
            get;
            set;
        }

        //

        //     .
        [JsonProperty("iarc_rating_id")]
        public string? IarcRatingId
        {
            get;
            set;
        }

        //

        //     .
        [JsonProperty("categories")]
        public IEnumerable<string>? Categories
        {
            get;
            set;
        }

        //

        //     Specifies the primary text direction for the name, short_name, and description
        //     members. Together with the lang member, it can help provide the correct display
        //     of right-to-left languages.
        [JsonProperty("dir")]
        public string? Dir
        {
            get;
            set;
        }

        //

        //     Specifies the primary language for the values in the name and short_name members.
        //     This value is a string containing a single language tag.
        [JsonProperty("lang")]
        public string? Lang
        {
            get;
            set;
        }

        //

        //     If you don't provide a start_url, the current page is used, which is unlikely
        //     to be what your users want.
        [JsonProperty("start_url")]
        public string? StartUrl
        {
            get;
            set;
        }

        //

        //     A list of icons.
        [JsonProperty("icons")]
        public IEnumerable<Icon>? Icons
        {
            get;
            set;
        }

        //

        //     A hex color value.
        [JsonProperty("background_color")]
        public string? BackgroundColor
        {
            get;
            set;
        }

        //

        //     A hex color value.
        [JsonProperty("theme_color")]
        public string? ThemeColor
        {
            get;
            set;
        }

        //

        //     Defines the developer's preferred display mode for the web application.
        [JsonProperty("display")]
        public string? Display
        {
            get;
            set;
        }

        [JsonProperty("orientation")]
        public string? Orientation
        {
            get;
            set;
        }

        //

        //     pecifies a boolean value that hints for the user agent to indicate to the user
        //     that the specified related applications are available, and recommended over the
        //     web application.
        [JsonProperty("prefer_related_applications")]
        public bool PreferRelatedApplications
        {
            get;
            set;
        }

        //

        //     Specifies an array of "application objects" representing native applications
        //     that are installable by, or accessible to, the underlying platform.
        [JsonProperty("related_applications")]
        public IEnumerable<RelatedApplication>? RelatedApplications
        {
            get;
            set;
        }

        //

        //     Defines the navigation scope of this web application's application context.
        [JsonProperty("scope")]
        public string? Scope
        {
            get;
            set;
        }

        //

        //     Check if the manifest is valid
        public bool IsValid(out string error)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ShortName) || string.IsNullOrEmpty(StartUrl) || Icons == null)
            {
                error = "The fields 'name', 'short_name', 'start_url' and 'icons' must be set  in " + FileName;
                return false;
            }

            if (!Icons.Any((Icon i) => i.Sizes?.Equals("512x512", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                error = "Missing icon in size 512x512 in " + FileName;
                return false;
            }

            if (!Icons.Any((Icon i) => i.Sizes?.Equals("192x192", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                error = "Missing icon in size 192x192 in " + FileName;
                return false;
            }

            error = "";
            return true;
        }
    }
}
