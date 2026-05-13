# Security Hardening Layer Documentation

The `OlimpBack` E2EE messenger is protected by a multi-layered security architecture designed to prevent abuse and maintain system integrity without ever decrypting user messages.

## 1. Security Protections

| Attack Vector | Mitigation Strategy | Component |
| :--- | :--- | :--- |
| **Replay Attacks** | Nonce tracking + Timestamp validation window (5 min). | `ReplayProtectionService` |
| **API Abuse / DoS** | Global IP & User rate limiting + Per-conversation limits. | `RateLimitService`, `SecurityHardeningMiddleware` |
| **Message Tampering** | Backend hash validation against encrypted payload. | `DeviceTrustService` |
| **Unauthorized Devices** | Identity verification for every message sent. | `DeviceTrustService` |
| **Unauthorized Access** | Strict participant validation for all conversation actions. | `ConversationAccessService` |
| **Payload Bloating** | Global request size limits (5MB). | `SecurityHardeningMiddleware` |

## 2. Security Flow (Message Sending)

1.  **Middleware:** Global rate limit check (IP/User).
2.  **Controller/Service:**
    *   **Access Check:** Verify sender is a participant.
    *   **Replay Check:** Verify nonce is unique and timestamp is fresh.
    *   **Trust Check:** Verify `DeviceId` belongs to the authenticated `UserId`.
    *   **Integrity Check:** Compute SHA256 of `EncryptedPayload` and compare with `MessageHash`.
    *   **Specific Rate Limit:** Check conversation-specific message frequency.
3.  **Persistence:** Store ciphertext and metadata.
4.  **SignalR:** Broadcast to conversation room participants.

## 3. Attack Scenarios

### Scenario A: Replaying a legitimate message
*   **Attack:** An attacker captures an encrypted message and re-sends it to the server.
*   **Protection:** The `ReplayProtectionService` will see that the `nonce` has already been used for this conversation within the allowed time window and reject the request.

### Scenario B: Injecting a message from a rogue device
*   **Attack:** An attacker tries to send a message using a stolen JWT but from their own untrusted device.
*   **Protection:** The `DeviceTrustService` verifies that the `DeviceId` in the request is registered to the user. Since the rogue device is not registered, the message is rejected.

### Scenario C: Spamming a specific conversation
*   **Attack:** A malicious participant tries to flood a chat with thousands of messages.
*   **Protection:** The `RateLimitService` enforces a 30 messages/minute limit per conversation, silently dropping or rejecting excess messages.

### Scenario D: Modifying ciphertext in transit
*   **Attack:** An attacker (or malicious proxy) modifies the `EncryptedPayload`.
*   **Protection:** The backend computes a hash of the received payload and compares it to the `MessageHash` provided by the client. Any mismatch results in immediate rejection.
