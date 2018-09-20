namespace Comet.Account.Tests
{
    using System;
    using Xunit;
    using Comet.Account;

    /// <summary>
    /// Validates that the server is configurable using the default configuration file.
    /// The configuration file can be in any format, but must be readable without 
    /// errors and overrideable using command-line arguments. 
    /// </summary>
    public class ConfigurationTests
    {
        [Theory]
        [InlineData()]
        [InlineData("--Network:Port=9960")]
        [InlineData("/Network:Port=9960")]
        [InlineData("--Network:Port", "9960")]
        [InlineData("/Network:Port", "9960")]
        [InlineData("Network:Port=9960")]
        public void ValidConfiguration(params string[] args)
        {
            var config = new ServerConfiguration(args);
            Assert.True(config.Valid);
            Assert.Equal("0.0.0.0", config.Network.IPAddress);
            Assert.Equal("localhost", config.Database.Hostname);
            if (args.Length > 0) Assert.Equal(9960, config.Network.Port);
        }
    }
}
