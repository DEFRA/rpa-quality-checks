using Microsoft.AspNetCore.Mvc;
using RPA.QualityPortal.Infrastructure;

namespace RPA.QualityPortal.Controllers
{
    public abstract class QualityController : Controller
    {
        private SessionBag session;

        protected QualityController()
        {
        }

        // Tests can assign a fake store; at run time it is resolved from the request services.
        public ISessionStore SessionStore { get; set; }

        public SessionBag Session
        {
            get
            {
                if (session == null)
                {
                    // Outside a request (e.g. a controller created directly in a unit test) a private store is used
                    var store = SessionStore
                        ?? ControllerContext?.HttpContext?.RequestServices?.GetService<ISessionStore>()
                        ?? new InMemorySessionStore();

                    SessionStore = store;
                    session = new SessionBag(store);
                }

                return session;
            }
        }
    }
}
