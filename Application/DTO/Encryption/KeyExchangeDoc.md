# Secure E2EE Key Exchange Flow (X3DH)

This backend implements the Public Key Infrastructure (PKI) required for Extended Triple Diffie-Hellman (X3DH).

## 1. Sequence Diagram

```text
Client A (Sender)             Backend (PKI)            Client B (Recipient)
    |                             |                             |
    |                             |---- Register IdentityKey ---|
    |                             |---- Upload SignedPreKey ----|
    |                             |---- Upload OTPKs (batch) ---|
    |                             |                             |
    |-- Request Bundle (User B) ->|                             |
    |<- Returns IdentityKey B,  --|                             |
    |   SignedPreKey B, OTPK B ---|                             |
    |                             |                             |
    |-- Generate Shared Secret ---|                             |
    |-- Encrypt Initial Message --|                             |
    |                             |                             |
    |----- Send Message (A->B) -->|                             |
    |                             |----- Relay Message (A->B) ->|
    |                             |                             |
```

## 2. Key Exchange Details

1.  **Identity Key (IK_B):** Long-term public key of Client B.
2.  **Signed PreKey (SPK_B):** Medium-term public key, signed by IK_B. Backend validates rotation but not signature (validation happens client-side).
3.  **One-Time PreKeys (OPK_B):** Ephemeral keys used only once. Backend ensures an OPK is provided and then marked as used.

## 3. Security Analysis

| Method | Protection | Why it is required |
| :--- | :--- | :--- |
| **GetAndMarkUsedOneTimePreKey** | Prekey Reuse Attacks | If a server provides the same OTPK twice, it weakens forward secrecy. Marking it as used ensures each session starts with a fresh ephemeral key. |
| **Identity Key Registration** | Identity Impersonation | The long-term identity key is the root of trust. Clients verify the signature of the SignedPreKey against this IdentityKey. |
| **Device Validation** | Unauthorized Messaging | Only registered devices with valid public keys can participate in the encryption flow. |
| **Anonymous Support** | Metadata Exposure | Key bundles can be retrieved via User ID or Device ID. In anonymous conversations, the backend can return bundles without revealing the real user email if pseudonyms are used. |

## 4. API Examples

### Register Device
**POST** `/api/Encryption/devices`
```json
{
  "deviceName": "iPhone 15",
  "identityKey": "base64_ik...",
  "signedPreKey": {
    "keyId": 1,
    "publicKey": "base64_spk...",
    "signature": "base64_sig..."
  },
  "oneTimePreKeys": [
    { "publicKey": "base64_otpk1..." },
    { "publicKey": "base64_otpk2..." }
  ]
}
```

### Fetch Key Bundle
**GET** `/api/Encryption/bundle/{userId}`
```json
{
  "userId": "...",
  "deviceId": "...",
  "identityKey": "...",
  "signedPreKey": { ... },
  "oneTimePreKey": { ... }
}
```
