// .NET 11 Libraries - Use newer cryptography and faster Half
// operations.
// Introduced: Preview 5 (X25519 key agreement); RC 1 (AES Key
// Wrap, FP16 hardware Half math).
//
//   * System.Security.Cryptography.X25519DiffieHellman
//     implements X25519 elliptic-curve
//     Diffie-Hellman key agreement - the curve used by TLS 1.3,
//     Signal and Noise.
//   * CryptographicOperations.
//     FixedTimeEquals(ReadOnlySpan<byte>, byte) compares a
//     buffer
//     against a single byte value in constant time.
//   * Aes gains the unpadded AES Key Wrap algorithm from RFC
//     3394: EncryptKeyWrap,
//     DecryptKeyWrap, TryDecryptKeyWrap and GetKeyWrapLength.
//   * The JIT now uses FP16 hardware instructions for Half
//     arithmetic and conversions.
//
// Sources:
//   https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/libraries.md#cryptography-adds-x25519-key-agreement
//   https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#aes-key-wrap-support
//   https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/runtime.md#half-operations-use-fp16-hardware-instructions

using System.Security.Cryptography;

Console.WriteLine("== X25519 key agreement ==");
Console.WriteLine($"supported on this platform: {X25519DiffieHellman.IsSupported}");

if (X25519DiffieHellman.IsSupported)
{
    using X25519DiffieHellman alice = X25519DiffieHellman.GenerateKey();
    using X25519DiffieHellman bob = X25519DiffieHellman.GenerateKey();

    byte[] alicePublic = alice.ExportPublicKey();
    byte[] bobPublic = bob.ExportPublicKey();

    byte[] aliceShared = new byte[X25519DiffieHellman.SecretAgreementSizeInBytes];
    byte[] bobShared = new byte[X25519DiffieHellman.SecretAgreementSizeInBytes];
    alice.DeriveRawSecretAgreement(bobPublic, aliceShared);
    bob.DeriveRawSecretAgreement(alicePublic, bobShared);

    Console.WriteLine($"alice public  : {Convert.ToHexString(alicePublic)}");
    Console.WriteLine($"bob   public  : {Convert.ToHexString(bobPublic)}");
    Console.WriteLine($"shared secrets match: {CryptographicOperations.FixedTimeEquals(aliceShared, bobShared)}");

    // PKCS#8 / PEM export is supported too.
    Console.WriteLine($"PKCS#8 bytes  : {alice.ExportPkcs8PrivateKey().Length}");
}

Console.WriteLine();
Console.WriteLine("== Constant-time check that a buffer is all zeroes ==");
byte[] cleared = new byte[32];
Console.WriteLine($"all zero: {CryptographicOperations.FixedTimeEquals(cleared, (byte)0)}");
cleared[17] = 1;
Console.WriteLine($"all zero: {CryptographicOperations.FixedTimeEquals(cleared, (byte)0)}");

Console.WriteLine();
Console.WriteLine("== AES Key Wrap (RFC 3394) ==");
using (Aes kek = Aes.Create())
{
    kek.Key = RandomNumberGenerator.GetBytes(32);

    byte[] contentKey = RandomNumberGenerator.GetBytes(32);
    Console.WriteLine($"wrapped length for a 32-byte key: {Aes.GetKeyWrapLength(contentKey.Length)}");

    byte[] wrapped = kek.EncryptKeyWrap(contentKey);
    Console.WriteLine($"wrapped  : {Convert.ToHexString(wrapped)}");

    byte[] unwrapped = kek.DecryptKeyWrap(wrapped);
    Console.WriteLine($"unwrapped matches original: {CryptographicOperations.FixedTimeEquals(contentKey, unwrapped)}");
}

Console.WriteLine();
Console.WriteLine("== Half arithmetic (JIT lowers these to FP16 instructions when available) ==");
Half a = (Half)1.5f;
Half b = (Half)2.0f;
Console.WriteLine($"{a} + {b} = {a + b}");
Console.WriteLine($"{a} * {b} = {a * b}");
Console.WriteLine($"sqrt({b}) = {Half.Sqrt(b)}");
Console.WriteLine($"(float)(Half)0.1f = {(float)(Half)0.1f}");
