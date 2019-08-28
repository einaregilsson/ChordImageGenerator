using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace EinarEgilsson.Chords
{
    public class ChordMiddleware
    {
        private readonly RequestDelegate _next;

        public ChordMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.ToString().ToLower().EndsWith(".png")) {
                await _next(context);
                return;
            }

            var query = context.Request.Query;
            
            var chordName = Regex.Match(context.Request.Path.ToString(), @"^/(.*)\.png$").Groups[1].Value;
            var pos = query["pos"].FirstOrDefault() ?? query["p"].FirstOrDefault() ?? "000000";
            var fingers = query["fingers"].FirstOrDefault() ?? query["f"].FirstOrDefault() ?? "------";
            var size = query["size"].FirstOrDefault() ?? query["s"].FirstOrDefault() ?? "1";

            context.Response.ContentType = "image/png";
            context.Response.GetTypedHeaders().CacheControl = 
                new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(7)
                };

            var image = new ChordBoxImage(chordName, pos, fingers, size);
            image.Save(context.Response.Body);
        }
    }
}
