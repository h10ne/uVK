using System;
using System.Linq;

namespace uVK.Helpers
{
    static class Decoder
    {
        public static Uri DecodeAudioUrl(Uri audioUrl)
        {
            var segments = audioUrl.Segments.ToList();

            if (segments.Count == 3)
            {
                return audioUrl;
            }
            segments.RemoveAt((segments.Count - 1) / 2);
            segments.RemoveAt(segments.Count - 1);

            segments[segments.Count - 1] = segments[segments.Count - 1].Replace("/", ".mp3");

            var UriToReturn = new Uri($"{audioUrl.Scheme}://{audioUrl.Host}{string.Join("", segments)}{audioUrl.Query}");

            return UriToReturn;
        }

        public static string ConvertTimeToString(int durration)
        {
            if (durration <= 0) throw new ArgumentOutOfRangeException(nameof(durration));
            int minutes = durration;
            int seconds = minutes % 60;
            minutes /= 60;
            string minutesStr = minutes.ToString();
            if (minutes < 10)
                minutesStr = "0" + minutes.ToString();
            string secondsStr = seconds.ToString();
            if (seconds < 10)
                secondsStr = "0" + seconds.ToString();
            return minutesStr + ":" + secondsStr;
        }
    }
}