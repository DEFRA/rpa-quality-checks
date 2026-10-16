using System.Net;

namespace RPA.QualityPortal.Helpers
{
    public enum HtmlTextWriterTag
    {
        Br,
        Div,
        H2,
        H3,
        H4,
        P
    }

    // Minimal replacement for System.Web.UI.HtmlTextWriter (not available on .NET Core) covering what the
    // e-mail builders use: nested begin/end tags, style attributes on the next tag, and raw text.
    public sealed class HtmlTextWriter : IDisposable
    {
        private readonly TextWriter writer;
        private readonly Stack<HtmlTextWriterTag?> openTags = new Stack<HtmlTextWriterTag?>();
        private readonly List<KeyValuePair<string, string>> pendingStyles = new List<KeyValuePair<string, string>>();

        public HtmlTextWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void AddStyleAttribute(string name, string value)
        {
            pendingStyles.Add(new KeyValuePair<string, string>(name, value));
        }

        public void RenderBeginTag(HtmlTextWriterTag tag)
        {
            var name = tag.ToString().ToLowerInvariant();

            writer.Write("<" + name);

            if (pendingStyles.Count > 0)
            {
                writer.Write(" style=\"" + string.Concat(pendingStyles.Select(s => s.Key + ":" + WebUtility.HtmlEncode(s.Value) + ";")) + "\"");
                pendingStyles.Clear();
            }

            if (tag == HtmlTextWriterTag.Br)
            {
                writer.Write(" />");
                openTags.Push(null);
            }
            else
            {
                writer.Write(">");
                openTags.Push(tag);
            }
        }

        public void RenderEndTag()
        {
            var tag = openTags.Pop();

            if (tag.HasValue)
            {
                writer.Write("</" + tag.Value.ToString().ToLowerInvariant() + ">");
            }
        }

        public void Write(string text)
        {
            writer.Write(text);
        }

        public void Write(object value)
        {
            writer.Write(value);
        }

        public void Dispose()
        {
            writer.Flush();
        }
    }
}
