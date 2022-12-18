using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PlayniteServices.Models.Discord
{
    public class AddonUpdateNotification
    {
        public DateTimeOffset Date { get; set; }
        public string? AddonId { get; set; }
        public Version? NotifyVersion { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string? message { get; set; }
    }

    public class Guild
    {
        public string? id { get; set; }
        public string? name { get; set; }

        public override string ToString()
        {
            return name ?? base.ToString()!;
        }
    }

    public class Channel
    {
        public string? id { get; set; }
        public int type { get; set; }
        public string? name { get; set; }

        public override string ToString()
        {
            return name ?? base.ToString()!;
        }
    }

    public class MessageCreate
    {
        public string? content { get; set; }
        public List<EmbedObject>? embeds { get; set; }
    }

    public class EmbedObject
    {
        public string? url { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public int color { get; set; }
        public EmbedImage? image { get; set; }
        public EmbedImage? thumbnail { get; set; }
        public List<EmbedField>? fields { get; set; }
        public EmbedAuthor? author { get; set; }
    }

    public class EmbedImage
    {
        public string? url { get; set; }
    }

    public class EmbedField
    {
        public string? name { get; set; }
        public string? value { get; set; }
        public bool inline { get; set; } = true;
    }

    public class EmbedAuthor
    {
        public string? name { get; set; }
        public string? url { get; set; }
    }

    public class Message
    {
        public string? id { get; set; }
        public string? channel_id { get; set; }
        public string? content { get; set; }
    }

    public class RateLimitResponse
    {
        public string? message { get; set; }
        public double retry_after { get; set; }
        public bool global { get; set; }
    }

    public class RateLimitHeaders
    {
        public int Limit { get; set; }
        public int Remaining { get; set; } = 999;
        public DateTimeOffset? Reset { get; }
        public TimeSpan? ResetAfter { get; }
        public string? Bucket { get; }
        public bool Global { get; set; }
        public string? Scope { get; }

        public RateLimitHeaders()
        {
        }

        public RateLimitHeaders(HttpResponseHeaders headers)
        {
            if (headers.TryGetValues("X-RateLimit-Global", out var globalVars))
            {
                Global = bool.Parse(globalVars.First());
            }

            if (headers.TryGetValues("X-RateLimit-Limit", out var limitVars))
            {
                Limit = int.Parse(limitVars.First());
            }

            if (headers.TryGetValues("X-RateLimit-Remaining", out var remainingVars))
            {
                Remaining = int.Parse(remainingVars.First());
            }

            if (headers.TryGetValues("X-RateLimit-Reset", out var resetVars))
            {
                var resetVar = (long)(double.Parse(resetVars.First(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) * 1000);
                Reset = DateTimeOffset.FromUnixTimeMilliseconds(resetVar);
            }

            if (headers.TryGetValues("X-RateLimit-Reset-After", out var resetAfterVars))
            {
                ResetAfter = TimeSpan.FromSeconds(double.Parse(resetAfterVars.First(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
            }

            if (headers.TryGetValues("X-RateLimit-Bucket", out var bucketVars))
            {
                Bucket = bucketVars.First();
            }

            if (headers.TryGetValues("X-RateLimit-Scope", out var scopeVars))
            {
                Scope = scopeVars.First();
            }
        }
    }
}
