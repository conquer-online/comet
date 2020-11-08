namespace Comet.Network.Security.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Utilities.Encoders;
    using Xunit;

    /// <summary>
    /// The Diffie-Hellman key exchange implementation in Comet must be interoperable with
    /// the Conquer Online game client. These tests ensure that the native implementation
    /// matches the results of the client's expected OpenSSL library.
    /// </summary>
    public class DiffieHellmanTests
    {
        const string ClientKey = 
            "59c152b3fa860f115316b5458f604e90f8ee9b253ecc137f46728a7701afb35b" + 
            "861c5b97a1d40b3640efc96782f0b2971c61bd5606944e7cf3015da456eeba62";
        const string Modulus = 
            "aafe7d12da3aad2d3b4957beb8887f02535a858cffbc0d7514ca2a413255a249";

        const string ExpectedPublicKey = 
            "710f779e2d4d6d763de21082fd21202c93ede9c97ecaeb48e290444e7685b590" +
            "511884d2bfb7c8f29b8aedd19a6cf6951519f3a123385910a3dac97a363de44e";
        const string ExpectedPrivateKey = 
            "01805cfc75fa3834ae160015c5bd7d6883eea835bfe6b277b6cd7968e9bccb60" +
            "28197772c760abd280419cc52245492511df455cfb1578ab2d58c2e1289d1853";

        [Fact]
        public async Task ComputePrivateKeyTestAsync()
        {
            DiffieHellman dh = new DiffieHellman();
            dh.Modulus = new BigInteger(DiffieHellmanTests.Modulus, 16);
            await dh.ComputePublicKeyAsync();
            Assert.Equal(ExpectedPublicKey, Hex.ToHexString(dh.PublicKey.ToByteArrayUnsigned()));
            
            dh.ComputePrivateKey(DiffieHellmanTests.ClientKey);
            Assert.Equal(ExpectedPrivateKey, Hex.ToHexString(dh.PrivateKey.ToByteArrayUnsigned()));
        }
    }
}
