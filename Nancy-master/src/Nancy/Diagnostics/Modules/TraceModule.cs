﻿namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Linq;

    public class TraceModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        public TraceModule(IRequestTracing sessionProvider)
            : base("/trace")
        {
            this.sessionProvider = sessionProvider;

            Get["/"] = _ =>
            {
                return View["RequestTracing"];
            };

            Get["/sessions"] = _ =>
            {
                return this.Response.AsJson(this.sessionProvider.GetSessions().Select(s => new { Id = s.Id }).ToArray());
            };

            Get["/sessions/{id}"] = ctx =>
            {
                Guid id;
                if (!Guid.TryParse(ctx.Id, out id))
                {
                    return HttpStatusCode.NotFound;
                }

                var session = 
                    this.sessionProvider.GetSessions().FirstOrDefault(s => s.Id == id);

                if (session == null)
                {
                    return HttpStatusCode.NotFound;
                }

                return this.Response.AsJson(session.RequestTraces.Select(t => new
                    {
                        t.Method,
                        t.RequestUrl,
                        ResponseType = t.ResponseType.ToString(),
                        t.RequestContentType,
                        t.ResponseContentType,
                        t.RequestHeaders,
                        t.ResponseHeaders,
                        t.StatusCode,
                        Log = t.TraceLog.ToString().Replace("\r", "").Split(new [] { "\n" }, StringSplitOptions.None),
                    }).ToArray());
            };
        }
    }
}