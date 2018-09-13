namespace Comet.Network.Security
{
    /// <summary>
    /// TQ Digital Entertainment's in-house developed counter-based XOR-cipher. Counters 
    /// are separated by encryption direction to create cipher streams. This implementation
    /// implements both directions for encrypting and decrypting data.
    /// </summary>
    /// <remarks>
    /// This cipher algorithm does not provide effective security, and does not make use 
    /// of any NP-hard calculations for encryption or key generation. Key derivations are
    /// susceptible to brute-force or static key attacks. Only implemented for 
    /// interoperability with the pre-existing game client.
    /// </remarks>
    public sealed class TQCipher
    {
        public TQCipher()
        {
            
        }
    }
}