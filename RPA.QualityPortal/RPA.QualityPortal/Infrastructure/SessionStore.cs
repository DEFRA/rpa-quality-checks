using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace RPA.QualityPortal.Infrastructure
{
    // Server-side store for objects that span several requests of one user's browser session
    // (replaces ASP.NET's InProc Session). Values are kept by reference, exactly as the old
    // InProc session did, so entity graphs, proxies and view models round-trip unchanged.
    public interface ISessionStore
    {
        object Get(string key);

        void Set(string key, object value);

        void Clear();
    }

    public class SessionStore : ISessionStore
    {
        // The old web.config used a session timeout of 480 minutes
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(480);
        private const string CookieName = ".QualityPortal.Session";

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;

        public SessionStore(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
        }

        public object Get(string key)
        {
            return Values(create: false)?.GetValueOrDefault(key);
        }

        public void Set(string key, object value)
        {
            var values = Values(create: true);

            if (value == null)
            {
                values.TryRemove(key, out _);
            }
            else
            {
                values[key] = value;
            }
        }

        public void Clear()
        {
            Values(create: false)?.Clear();
        }

        private ConcurrentDictionary<string, object> Values(bool create)
        {
            var context = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("Session data is only available during a request.");

            // A session created earlier in this request is not in the request cookies yet
            var sessionId = context.Items[CookieName] as string;

            if (string.IsNullOrEmpty(sessionId))
            {
                context.Request.Cookies.TryGetValue(CookieName, out sessionId);
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                if (!create)
                {
                    return null;
                }

                sessionId = Guid.NewGuid().ToString("N");

                context.Response.Cookies.Append(CookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });

                context.Items[CookieName] = sessionId;
            }
            var cacheKey = "session:" + sessionId;

            if (create)
            {
                return cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.SlidingExpiration = Timeout;
                    return new ConcurrentDictionary<string, object>();
                });
            }

            return cache.TryGetValue(cacheKey, out ConcurrentDictionary<string, object> existing) ? existing : null;
        }
    }

    // Store that is not tied to a browser session (single owner, e.g. a unit test or a controller created by hand)
    public class InMemorySessionStore : ISessionStore
    {
        private readonly ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>();

        public object Get(string key) => values.GetValueOrDefault(key);

        public void Set(string key, object value)
        {
            if (value == null) { values.TryRemove(key, out _); } else { values[key] = value; }
        }

        public void Clear() => values.Clear();
    }

    // Dictionary-style view of the session store, so controllers keep using Session["key"] = value.
    public class SessionBag
    {
        private readonly ISessionStore store;

        public SessionBag(ISessionStore store)
        {
            this.store = store;
        }

        public object this[string key]
        {
            get => store.Get(key);
            set => store.Set(key, value);
        }

        public void Clear()
        {
            store.Clear();
        }
    }
}
