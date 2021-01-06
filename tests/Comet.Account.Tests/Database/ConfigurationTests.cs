namespace Comet.Account.Database.Tests
{
    using System;
    using Xunit;
    using Comet.Account.Database;

    /// <summary>
    /// Validates that the server is configurable using the default configuration file.
    /// The configuration file can be in any format, but must be readable without 
    /// errors and overrideable using command-line arguments. 
    /// </summary>
    public class ConfigurationTests
    {
        [Theory]
        [InlineData()]
        [InlineData("--Network:Port=9961")]
        [InlineData("/Network:Port=9961")]
        [InlineData("--Network:Port", "9961")]
        [InlineData("/Network:Port", "9961")]
        [InlineData("Network:Port=9961")]
        public void ValidConfiguration(params string[] args)
        {
            var config = new ServerConfiguration(args);
            Assert.True(config.Valid);
            Assert.Equal("0.0.0.0", config.Network.IPAddress);
            Assert.Equal("localhost", config.Database.Hostname);
            Assert.Equal(args.Length > 0 ? 9961 : 9960, config.Network.Port);
        }
    }
}
