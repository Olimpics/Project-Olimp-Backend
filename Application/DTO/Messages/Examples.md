### Send Encrypted Message
**POST** `/api/Message`
**Request:**
```json
{
  "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "senderDevicePublicKey": "base64_public_key...",
  "encryptedPayload": "base64_ciphertext...",
  "nonce": "base64_nonce..."
}
```
**Response:**
```json
{
  "idMessage": "5fa85f64-5717-4562-b3fc-2c963f66afa6",
  "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "senderId": "a190f1ee-6c54-4b01-90e6-d701748f0851",
  "senderDevicePublicKey": "base64_public_key...",
  "encryptedPayload": "base64_ciphertext...",
  "nonce": "base64_nonce...",
  "isDelivered": false,
  "createdAt": "2026-05-13T13:45:00Z"
}
```

### Get Message History (Cursor Pagination)
**GET** `/api/Message/history?conversationId=3fa85f64-5717-4562-b3fc-2c963f66afa6&limit=20`
**Response:**
```json
{
  "messages": [...],
  "nextCursor": "MjAyNi0wNS0xM1QxMzo0NTowMFp8NWZhODVmNjQtNTcxNy00NTYyLWIzZmMtMmM5NjNmNjZhZmE2"
}
```

### Sync Offline Messages
**GET** `/api/Message/sync?conversationId=3fa85f64-5717-4562-b3fc-2c963f66afa6&since=2026-05-13T13:00:00Z`
**Response:**
```json
[
  {
    "idMessage": "...",
    "encryptedPayload": "...",
    ...
  }
]
```

### Mark Delivered/Read
**POST** `/api/Message/{id}/delivered`
**POST** `/api/Message/{id}/read`
