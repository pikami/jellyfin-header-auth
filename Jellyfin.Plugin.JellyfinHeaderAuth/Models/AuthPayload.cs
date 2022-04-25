namespace Jellyfin.Plugin.JellyfinHeaderAuth.Models
{
    /// <summary>
    /// The data the client should pass back to the API.
    /// </summary>
    public class AuthPayload
    {
        /// <summary>
        /// Gets or sets the device ID of the client.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets the device name of the client.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the app name of the client.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the app version of the client.
        /// </summary>
        public string AppVersion { get; set; }
    }
}