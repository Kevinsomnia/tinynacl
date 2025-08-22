## TinyNaCl

A **N**etworking **a**nd **C**rytography **L**ibrary, but only a tiny part of it.

This is a Unity package containing Ed25519 signature verification, which can be used to verify authenticity and integrity of game files. Creating signatures is not something that is usually done in a game client, so that functionality is not present. Package is meant to be very bare-bones, a lot of .NET libraries out there contain too much bloat.

### Usage

Verifying Ed25519 signature

```csharp
byte[] data = System.IO.File.ReadAllBytes("file.txt");
byte[] signature = ... // typically signed by private key and sent from server
byte[] publicKey = ... // raw/unwrapped public key (32 bytes)
TinyNaCl.Ed25519.Verify(signature, data, publicKey);
```

### Credits

Adapted from https://github.com/CryptoManiac/Ed25519 and https://github.com/CodesInChaos/Chaos.NaCl.
