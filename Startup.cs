using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EinarEgilsson.Chords
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ChordMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
