using System;
using System.Collections.Generic;
using Jellyfin.Plugin.JellyfinHeaderAuth.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.JellyfinHeaderAuth
{
    public class HeaderAuthPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public HeaderAuthPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static HeaderAuthPlugin Instance { get; private set; }

        public override string Name => "Jellyfin-Header-Auth";

        public override Guid Id => Guid.Parse("df78a0dc-a52a-4a9b-bae4-3f60390b83c4");

        public IEnumerable<PluginPageInfo> GetPages()
        {
            yield return new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = $"{GetType().Namespace}.Config.configPage.html"
            };
        }
    }
}
