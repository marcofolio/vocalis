using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocalisBot
{
    public static class Constants
    {
        public const string LUIS_MODEL_ID = "<GUID_LUIS_MODEL_ID>";
        public const string LUIS_SUBSCRIPTION_KEY = "<LUIS_SUBSCRIPTION_KEY>";

        public const string COMPUTER_VISION_KEY = "<COMPUTER_VISION_KEY>";
        public const string COMPUTER_VISION_ROOT = "https://<AZURE_LOCATION>.api.cognitive.microsoft.com/vision/v1.0";

        public const string FACE_API_KEY = "<FACE_API_KEY>";
        public const string FACE_API_ROOT = "https://<AZURE_LOCATION>.api.cognitive.microsoft.com/face/v1.0";

        public const string BING_API_KEY = "<BING_API_KEY>";
        public const string BING_IMAGE_API_ROOT = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?";
    }
}