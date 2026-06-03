## TinyNaCl

A **N**etworking **a**nd **C**rytography **L**ibrary, but only a tiny part of it.

This is a Unity package containing Ed25519 signature verification and signing, which can be used to verify authenticity and integrity of game files. Package is meant to be very bare-bones, a lot of .NET libraries out there contain too much bloat.

### Installation

Use the package manager within Unity and add a package via Git URL. Only tested with Unity 6.

### Usage

Verifying an Ed25519 signature

```csharp
byte[] data = System.IO.File.ReadAllBytes("file.txt");
byte[] signature = ... // typically signed by private key and sent from server
byte[] publicKey = ... // raw/unwrapped public key (32 bytes)
bool dataIsValid = TinyNaCl.Ed25519.Verify(signature, data, publicKey);
```

Signing an Ed25519 message (RFC 8032)

```csharp
byte[] seed = ... // 32-byte private key seed
byte[] message = System.IO.File.ReadAllBytes("file.txt");
byte[] signature = TinyNaCl.Ed25519.Sign(message, seed); // 64 bytes

// Derive the matching public key from the same seed
byte[] publicKey = TinyNaCl.Ed25519.PublicKeyFromSeed(seed); // 32 bytes
```

Keys are passed as raw bytes. If you have a hex-encoded seed, convert it
to a `byte[]` before calling `Sign` / `PublicKeyFromSeed`.

> [!WARNING]
> **Do not sign on end-user clients.** The private key seed is the
> entire secret. Anyone who can read it from memory, disk, or a crash
> dump can forge arbitrary signatures. Run `Sign` only in trusted
> environments where the seed lives: developer tooling, CI pipelines,
> or a server-side service. Ship only the resulting `signature` and
> `publicKey` to clients, and use `Verify` there.

### Credits

Adapted from https://github.com/CryptoManiac/Ed25519 and https://github.com/CodesInChaos/Chaos.NaCl.
