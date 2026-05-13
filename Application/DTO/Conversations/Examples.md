### Create Normal Conversation
**POST** `/api/Conversation`
**Request:**
```json
{
  "participantIds": ["d290f1ee-6c54-4b01-90e6-d701748f0851"],
  "isAnonymous": false
}
```
**Response:**
```json
{
  "idConversation": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "conversationToken": "4e968b5a-0648-4e8c-843a-7f154316d3e8",
  "isAnonymous": false,
  "createdAt": "2026-05-13T13:15:00Z",
  "participants": [
    {
      "idParticipant": "7e968b5a-0648-4e8c-843a-7f154316d3e9",
      "userId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
      "pseudonym": null,
      "isIdentityRevealed": true,
      "displayName": "user@example.com"
    },
    {
      "idParticipant": "8e968b5a-0648-4e8c-843a-7f154316d3e0",
      "userId": "a190f1ee-6c54-4b01-90e6-d701748f0851",
      "pseudonym": null,
      "isIdentityRevealed": true,
      "displayName": "me@example.com"
    }
  ]
}
```

### Create Anonymous Conversation
**POST** `/api/Conversation`
**Request:**
```json
{
  "participantIds": ["d290f1ee-6c54-4b01-90e6-d701748f0851"],
  "isAnonymous": true
}
```
**Response:**
```json
{
  "idConversation": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "conversationToken": "4e968b5a-0648-4e8c-843a-7f154316d3e8",
  "isAnonymous": true,
  "createdAt": "2026-05-13T13:16:00Z",
  "participants": [
    {
      "idParticipant": "7e968b5a-0648-4e8c-843a-7f154316d3e9",
      "userId": null,
      "pseudonym": "Silent Wolf #412",
      "isIdentityRevealed": false,
      "displayName": "Silent Wolf #412"
    },
    {
      "idParticipant": "8e968b5a-0648-4e8c-843a-7f154316d3e0",
      "userId": "a190f1ee-6c54-4b01-90e6-d701748f0851",
      "pseudonym": "Swift Fox #089",
      "isIdentityRevealed": false,
      "displayName": "me@example.com"
    }
  ]
}
```

### Reveal Identity
**POST** `/api/Conversation/reveal`
**Request:**
```json
{
  "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
**Response:**
```json
{
  "message": "Identity revealed successfully"
}
```
