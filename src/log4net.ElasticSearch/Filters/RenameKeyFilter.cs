using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatters;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RenameKeyFilter : IElasticAppenderFilter
    {
        private LogEventSmartFormatter _key;
        private LogEventSmartFormatter _renameTo;
        private const string FailedToRename = "RenameFailed";

        public bool Overwrite { get; set; }

        [PropertyNotEmpty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [PropertyNotEmpty]
        public string RenameTo
        {
            get { return _renameTo; }
            set { _renameTo = value; }
        }

        public RenameKeyFilter()
        {
            Overwrite = true;
        }

        public void PrepareConfiguration(IElasticsearchClient client)
        {
        }

        public void PrepareEvent(JObject logEvent)
        {
            JToken token;
            if (logEvent.TryGetValue(_key.Format(logEvent), out token))
            {
                token.Parent.Remove();

                var renameTo = _renameTo.Format(logEvent);

                if (!Overwrite && logEvent.HasKey(renameTo))
                {
                    logEvent.AddTag(FailedToRename);
                    return;
                }

                logEvent[renameTo] = token;
            }
        }
    }
}