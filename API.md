# GreenTrace GraphQL API — Frontend Developer Guide

## Overview

GreenTrace is a supply-chain-agnostic value chain intelligence platform. The API is a **HotChocolate GraphQL** server built on .NET 9.

The API tracks **value flow** through supply chains — who handled what, where, and what it was worth at each stage.

---

## Getting Started

### Endpoint

```
POST /graphql
```

All queries and mutations go through this single endpoint.

### Authentication

The API uses **JWT Bearer tokens**. Include the token in every request header:

```
Authorization: Bearer <accessToken>
```

### Request Format

Standard GraphQL over HTTP — send JSON with `query`, `variables`, and optionally `operationName`.

```json
{
  "query": "mutation Login($email: String!, $password: String!) { ... }",
  "variables": { "email": "user@example.com", "password": "secret" }
}
```

---

## 1. Authentication

### Login

```graphql
mutation Login($email: String!, $password: String!) {
  login(input: { email: $email, password: $password }) {
    success
    message
    code
    result {
      accessToken
      refreshToken
      expiresAt
    }
  }
}
```

### Register

```graphql
mutation Register(
  $email: String!,
  $password: String!,
  $fullName: String!,
  $phone: String,
  $organizationId: Long!,
  $roleName: String
) {
  register(input: {
    email: $email
    password: $password
    fullName: $fullName
    phone: $phone
    organizationId: $organizationId
    roleName: $roleName
  }) {
    success
    message
    code
    result {
      accessToken
      refreshToken
      expiresAt
    }
  }
}
```

### Refresh Token

```graphql
mutation RefreshAccessToken($refreshToken: String!) {
  refreshAccessToken(input: { refreshToken: $refreshToken }) {
    success
    message
    code
    result {
      accessToken
      refreshToken
      expiresAt
    }
  }
}
```

### Logout

```graphql
mutation Logout($refreshToken: String!) {
  logout(input: { refreshToken: $refreshToken }) {
    success
    message
    code
  }
}
```

### Get Current User

```graphql
query Me {
  me {
    userId
    organizationId
    username
    email
    fullName
    role
    permissions
  }
}
```

---

## 2. Mutation Response Format

Every mutation returns a `CallResult` wrapper. Always check `success` first.

```ts
{
  success: boolean      // true = operation succeeded
  message: string       // human-readable status message
  code: number          // HTTP-style code: 200, 400, 401, 403, 404
  result: T | null      // returned data (only on success, only some mutations)
}
```

**Code reference:**
| Code | Meaning |
|------|---------|
| 200  | Success |
| 400  | Validation error / bad input |
| 401  | Not authenticated |
| 403  | Not permitted |
| 404  | Entity not found |

---

## 3. Permissions

Permissions are stored as strings on each user's role. The `me` query returns the current user's `permissions` array. Platform admins bypass all permission checks.

| Key | Description |
|-----|-------------|
| `organizations.view` | View organizations |
| `organizations.create` | Create organizations |
| `organizations.update` | Update organizations |
| `organizations.delete` | Delete organizations |
| `users.view` | View users |
| `users.create` | Create users |
| `users.update` | Update users |
| `users.delete` | Deactivate users |
| `roles.view` | View roles |
| `roles.create` | Create roles |
| `roles.update` | Update roles |
| `roles.delete` | Deactivate roles |
| `domains.view` | View domains |
| `domains.create` | Create domains |
| `domains.update` | Update domains |
| `domains.delete` | Deactivate domains |
| `stages.view` | View stages |
| `stages.create` | Create stages |
| `stages.update` | Update stages |
| `stages.delete` | Deactivate stages |
| `products.view` | View products |
| `products.create` | Create products |
| `products.update` | Update products |
| `products.delete` | Deactivate products |
| `batches.view` | View batches |
| `batches.create` | Create batches |
| `batches.update` | Update batches |
| `batches.delete` | Delete batches |
| `stage_records.view` | View stage records |
| `stage_records.create` | Create stage records |
| `stage_records.update` | Update stage records |
| `stage_records.validate` | Validate / reject stage records |
| `analytics.view` | View analytics queries |
| `audit.view` | View audit logs |
| `integrations.view` | View integrations |
| `integrations.create` | Create integrations |
| `integrations.update` | Update integrations |
| `integrations.delete` | Delete integrations |

**Org-scoping:** All list queries automatically filter to the current user's organization. Platform admins see data across all organizations.

---

## 4. Queries — Common Patterns

All list queries support **offset pagination**, **filtering**, and **sorting** via HotChocolate conventions.

### Pagination

```graphql
query {
  batches(skip: 0, take: 20) {
    items {
      id
      batchCode
      status
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
    }
    totalCount
  }
}
```

### Filtering

```graphql
query {
  batches(where: { status: { eq: "open" } }) {
    items { id batchCode status }
    totalCount
  }
}
```

```graphql
query {
  actors(where: { name: { contains: "John" } }) {
    items { id name }
  }
}
```

### Sorting

```graphql
query {
  batches(order: [{ createdOn: DESC }]) {
    items { id batchCode createdOn }
  }
}
```

---

## 5. Organizations

### Query

```graphql
query Organizations {
  organizations(skip: 0, take: 20) {
    items {
      id
      name
      slug
      isActive
      createdOn
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateOrganization($name: String!, $slug: String!) {
  createOrganization(input: { name: $name, slug: $slug }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateOrganization($id: Long!, $name: String, $slug: String) {
  updateOrganization(input: { id: $id, name: $name, slug: $slug }) {
    success
    message
    code
  }
}
```

### Deactivate

```graphql
mutation DeactivateOrganization($id: Long!) {
  deactivateOrganization(input: { id: $id }) {
    success
    message
    code
  }
}
```

---

## 6. Users

### Query

```graphql
query Users {
  users(skip: 0, take: 20) {
    items {
      id
      email
      fullName
      isActive
      organizationId
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateUser(
  $email: String!,
  $password: String!,
  $fullName: String!,
  $phone: String,
  $organizationId: Long!,
  $roleName: String!
) {
  createUser(input: {
    email: $email
    password: $password
    fullName: $fullName
    phone: $phone
    organizationId: $organizationId
    roleName: $roleName
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateUser(
  $id: Long!,
  $fullName: String,
  $phone: String,
  $roleName: String,
  $isActive: Boolean
) {
  updateUser(input: {
    id: $id
    fullName: $fullName
    phone: $phone
    roleName: $roleName
    isActive: $isActive
  }) {
    success
    message
    code
  }
}
```

### Deactivate

```graphql
mutation DeactivateUser($id: Long!) {
  deactivateUser(input: { id: $id }) {
    success
    message
    code
  }
}
```

---

## 7. Roles

### Query

```graphql
query Roles {
  roles(skip: 0, take: 50) {
    items {
      id
      name
      description
      permissions
      isActive
    }
    totalCount
  }
}
```

### Available Permissions (for role builder UI)

```graphql
query AvailablePermissions {
  availablePermissions
}
```

Returns all valid permission strings — useful for populating a permissions picker.

### Create

```graphql
mutation CreateRole(
  $name: String!,
  $description: String,
  $permissions: [String!]!
) {
  createRole(input: {
    name: $name
    description: $description
    permissions: $permissions
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateRole(
  $id: Long!,
  $description: String,
  $permissions: [String!],
  $isActive: Boolean
) {
  updateRole(input: {
    id: $id
    description: $description
    permissions: $permissions
    isActive: $isActive
  }) {
    success
    message
    code
  }
}
```

---

## 8. Domains

A domain represents a supply chain type (e.g., "Cocoa", "Gold Mining", "Pharmaceuticals").

### Query

```graphql
query Domains {
  domains(skip: 0, take: 20) {
    items {
      id
      organizationId
      name
      code
      description
      isActive
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateDomain(
  $organizationId: Long!,
  $name: String!,
  $code: String!,
  $description: String
) {
  createDomain(input: {
    organizationId: $organizationId
    name: $name
    code: $code
    description: $description
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateDomain(
  $id: Long!,
  $name: String,
  $description: String,
  $isActive: Boolean
) {
  updateDomain(input: {
    id: $id
    name: $name
    description: $description
    isActive: $isActive
  }) {
    success
    message
    code
  }
}
```

---

## 9. Stages

Stages are the sequential steps in a domain's supply chain (e.g., "Harvesting → Processing → Export").

### Query

```graphql
query Stages($domainId: Long) {
  stages(where: { domainId: { eq: $domainId } }, order: [{ sequence: ASC }]) {
    items {
      id
      domainId
      name
      code
      sequence
      isRequired
      description
      isActive
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateStage(
  $domainId: Long!,
  $name: String!,
  $code: String!,
  $sequence: Short!,
  $isRequired: Boolean!,
  $description: String
) {
  createStage(input: {
    domainId: $domainId
    name: $name
    code: $code
    sequence: $sequence
    isRequired: $isRequired
    description: $description
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateStage(
  $id: Long!,
  $name: String,
  $sequence: Short,
  $isRequired: Boolean,
  $description: String,
  $isActive: Boolean
) {
  updateStage(input: {
    id: $id
    name: $name
    sequence: $sequence
    isRequired: $isRequired
    description: $description
    isActive: $isActive
  }) {
    success
    message
    code
  }
}
```

---

## 10. Stage Fields

Stage fields define the form schema for a stage — what data needs to be captured when a batch passes through it.

### Query

```graphql
query StageFields($stageId: Long) {
  stageFields(
    where: { stageId: { eq: $stageId }, isActive: { eq: true } },
    order: [{ fieldOrder: ASC }]
  ) {
    items {
      id
      stageId
      code
      label
      dataType
      isRequired
      fieldOrder
      validation
      options
      uiHint
    }
    totalCount
  }
}
```

**`dataType` values:** `text`, `number`, `decimal`, `boolean`, `date`, `datetime`, `select`, `multiselect`, `file`

**`validation`** — JSON string with rules, e.g.: `{"min": 0, "max": 1000}`

**`options`** — JSON string for select fields, e.g.: `[{"value": "A", "label": "Option A"}]`

**`uiHint`** — JSON string for rendering hints, e.g.: `{"widget": "slider", "unit": "kg"}`

### Create

```graphql
mutation CreateStageField(
  $stageId: Long!,
  $code: String!,
  $label: String!,
  $dataType: String!,
  $isRequired: Boolean!,
  $fieldOrder: Int,
  $validation: String,
  $options: String,
  $uiHint: String
) {
  createStageField(input: {
    stageId: $stageId
    code: $code
    label: $label
    dataType: $dataType
    isRequired: $isRequired
    fieldOrder: $fieldOrder
    validation: $validation
    options: $options
    uiHint: $uiHint
  }) {
    success
    message
    code
  }
}
```

---

## 11. Products

### Query

```graphql
query Products {
  products(skip: 0, take: 20) {
    items {
      id
      organizationId
      domainId
      name
      sku
      unitOfMeasure
      description
      isActive
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateProduct(
  $organizationId: Long!,
  $domainId: Long,
  $name: String!,
  $sku: String,
  $unitOfMeasure: String,
  $description: String
) {
  createProduct(input: {
    organizationId: $organizationId
    domainId: $domainId
    name: $name
    sku: $sku
    unitOfMeasure: $unitOfMeasure
    description: $description
  }) {
    success
    message
    code
  }
}
```

---

## 12. Actors

Actors are real-world participants in the supply chain (farmers, processors, transporters, etc.).

### Query

```graphql
query Actors {
  actors(skip: 0, take: 20) {
    items {
      id
      organizationId
      roleId
      name
      externalId
      contact
      location
      registrationMeta
      isActive
    }
    totalCount
  }
}
```

**JSON field shapes:**

`contact`: `{ "phone": "...", "email": "...", "address": "..." }`

`location`: `{ "lat": 5.6037, "lng": -0.1870, "region": "Greater Accra", "district": "Accra Metro", "community": "Osu" }`

`registrationMeta`: `{ "idNumber": "GHA-001", "cooperativeMembership": "Coop-A" }`

### Create

```graphql
mutation CreateActor(
  $organizationId: Long!,
  $roleId: Long,
  $name: String!,
  $externalId: String,
  $contact: String,
  $location: String,
  $registrationMeta: String
) {
  createActor(input: {
    organizationId: $organizationId
    roleId: $roleId
    name: $name
    externalId: $externalId
    contact: $contact
    location: $location
    registrationMeta: $registrationMeta
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateActor(
  $id: Long!,
  $roleId: Long,
  $name: String,
  $externalId: String,
  $contact: String,
  $location: String,
  $registrationMeta: String,
  $isActive: Boolean
) {
  updateActor(input: {
    id: $id
    roleId: $roleId
    name: $name
    externalId: $externalId
    contact: $contact
    location: $location
    registrationMeta: $registrationMeta
    isActive: $isActive
  }) {
    success
    message
    code
  }
}
```

---

## 13. Actor Roles

Actor roles categorize actors within a domain (e.g., "Farmer", "Cooperative Agent", "Exporter").

### Query

```graphql
query ActorRoles($domainId: Long) {
  actorRoles(where: { domainId: { eq: $domainId } }) {
    items {
      id
      organizationId
      domainId
      code
      label
      description
      allowedStages
      isActive
    }
    totalCount
  }
}
```

`allowedStages` — array of stage IDs this role can participate in. `null` means the role can participate in any stage.

### Create

```graphql
mutation CreateActorRole(
  $organizationId: Long!,
  $domainId: Long,
  $code: String!,
  $label: String!,
  $description: String,
  $allowedStages: [Long!]
) {
  createActorRole(input: {
    organizationId: $organizationId
    domainId: $domainId
    code: $code
    label: $label
    description: $description
    allowedStages: $allowedStages
  }) {
    success
    message
    code
  }
}
```

---

## 14. Credentials

Credentials are certifications or licenses attached to actors (e.g., organic certification, mining license).

### Query

```graphql
query Credentials($actorId: Long) {
  credentials(where: { actorId: { eq: $actorId } }) {
    items {
      id
      actorId
      organizationId
      type
      issuer
      reference
      issuedAt
      expiresAt
      isActive
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateCredential(
  $actorId: Long!,
  $type: String!,
  $issuer: String,
  $reference: String,
  $issuedAt: DateTime,
  $expiresAt: DateTime
) {
  createCredential(input: {
    actorId: $actorId
    type: $type
    issuer: $issuer
    reference: $reference
    issuedAt: $issuedAt
    expiresAt: $expiresAt
  }) {
    success
    message
    code
  }
}
```

---

## 15. Batches

Batches are the core tracking unit — a specific quantity of product moving through the supply chain.

**Status values:** `open` → `in_progress` → `completed` → `archived`

### Query

```graphql
query Batches($status: String) {
  batches(
    where: { status: { eq: $status }, isDeleted: { eq: false } },
    order: [{ createdOn: DESC }]
  ) {
    items {
      id
      organizationId
      domainId
      productId
      batchCode
      originActorId
      originDate
      originLocation
      initialQuantity
      initialUnitPrice
      currentQuantity
      currentStageId
      totalValue
      status
      metadata
      createdOn
    }
    totalCount
  }
}
```

### Create

```graphql
mutation CreateBatch(
  $organizationId: Long!,
  $domainId: Long!,
  $productId: Long,
  $batchCode: String!,
  $originActorId: Long,
  $originDate: DateTime,
  $originLocation: String,
  $initialQuantity: Decimal,
  $initialUnitPrice: Decimal,
  $metadata: String
) {
  createBatch(input: {
    organizationId: $organizationId
    domainId: $domainId
    productId: $productId
    batchCode: $batchCode
    originActorId: $originActorId
    originDate: $originDate
    originLocation: $originLocation
    initialQuantity: $initialQuantity
    initialUnitPrice: $initialUnitPrice
    metadata: $metadata
  }) {
    success
    message
    code
  }
}
```

### Update

```graphql
mutation UpdateBatch(
  $id: Long!,
  $productId: Long,
  $originLocation: String,
  $currentQuantity: Decimal,
  $status: String,
  $metadata: String
) {
  updateBatch(input: {
    id: $id
    productId: $productId
    originLocation: $originLocation
    currentQuantity: $currentQuantity
    status: $status
    metadata: $metadata
  }) {
    success
    message
    code
  }
}
```

### Split Batch

Creates a child batch from part of a parent batch's quantity.

```graphql
mutation SplitBatch(
  $parentBatchId: Long!,
  $childBatchCode: String!,
  $quantity: Decimal!,
  $notes: String
) {
  splitBatch(input: {
    parentBatchId: $parentBatchId
    childBatchCode: $childBatchCode
    quantity: $quantity
    notes: $notes
  }) {
    success
    message
    code
  }
}
```

### Merge Batches

Combines multiple parent batches into one child batch.

```graphql
mutation MergeBatches(
  $parentBatchIds: [Long!]!,
  $childBatchCode: String!,
  $notes: String
) {
  mergeBatches(input: {
    parentBatchIds: $parentBatchIds
    childBatchCode: $childBatchCode
    notes: $notes
  }) {
    success
    message
    code
  }
}
```

### Batch Lineage

Track split/merge history.

```graphql
query BatchLineage($batchId: Long) {
  batchLineages(
    where: {
      or: [
        { parentBatchId: { eq: $batchId } },
        { childBatchId: { eq: $batchId } }
      ]
    }
  ) {
    items {
      id
      parentBatchId
      childBatchId
      type
      quantity
      notes
      createdOn
    }
    totalCount
  }
}
```

`type` values: `split`, `merge`

---

## 16. Stage Records

Stage records capture what happened at each stage for a batch — data collected, actor involved, quantity processed, validation status.

**Status flow:** `draft` → `submitted` → `validated` (or `rejected` → back to `draft` or re-submitted)

### Query

```graphql
query StageRecords($batchId: Long, $status: String) {
  stageRecords(
    where: {
      batchId: { eq: $batchId },
      status: { eq: $status },
      isDeleted: { eq: false }
    }
  ) {
    items {
      id
      organizationId
      batchId
      stageId
      actorId
      quantity
      unitPrice
      currency
      geoPoint
      notes
      recordedAt
      status
      validatedAt
      validationNotes
      createdOn
    }
    totalCount
  }
}
```

### Query with Field Values

```graphql
query StageRecordDetail($stageRecordId: Long!) {
  stageRecordFields(where: { stageRecordId: { eq: $stageRecordId } }) {
    items {
      id
      stageRecordId
      stageFieldId
      value
    }
  }
}
```

### Create

Creates a `draft` stage record. Optionally include field values in the same call.

```graphql
mutation CreateStageRecord(
  $batchId: Long!,
  $stageId: Long!,
  $actorId: Long,
  $quantity: Decimal,
  $unitPrice: Decimal,
  $currency: String,
  $geoPoint: String,
  $notes: String,
  $recordedAt: DateTime,
  $fields: [StageRecordFieldInput!]
) {
  createStageRecord(input: {
    batchId: $batchId
    stageId: $stageId
    actorId: $actorId
    quantity: $quantity
    unitPrice: $unitPrice
    currency: $currency
    geoPoint: $geoPoint
    notes: $notes
    recordedAt: $recordedAt
    fields: $fields
  }) {
    success
    message
    code
  }
}
```

**`StageRecordFieldInput`:**
```json
{ "stageFieldId": 5, "value": "42.5" }
```

`geoPoint` JSON shape: `{ "lat": 5.6037, "lng": -0.1870, "accuracy": 10 }`

### Submit (draft → submitted)

```graphql
mutation SubmitStageRecord($id: Long!) {
  submitStageRecord(input: { id: $id }) {
    success
    message
    code
  }
}
```

### Validate (submitted → validated)

Requires `stage_records.validate` permission. This also automatically creates a `ValueRecord`.

```graphql
mutation ValidateStageRecord($id: Long!, $validationNotes: String) {
  validateStageRecord(input: { id: $id, validationNotes: $validationNotes }) {
    success
    message
    code
  }
}
```

### Reject (submitted → rejected)

```graphql
mutation RejectStageRecord($id: Long!, $validationNotes: String) {
  rejectStageRecord(input: { id: $id, validationNotes: $validationNotes }) {
    success
    message
    code
  }
}
```

---

## 17. Evidence

Evidence are file attachments linked to stage records or actor credentials (photos, documents, certificates).

### Query

```graphql
query Evidence($stageRecordId: Long) {
  evidences(where: { stageRecordId: { eq: $stageRecordId } }) {
    items {
      id
      organizationId
      stageRecordId
      credentialId
      storageKey
      originalFilename
      mimeType
      sizeBytes
      type
      description
      uploadedBy
      createdOn
    }
    totalCount
  }
}
```

**`type` values:** `photo`, `document`, `certificate`, `lab_result`, `receipt`, `other`

### Create

Upload the file to storage first, then register it here with the returned `storageKey`.

```graphql
mutation CreateEvidence(
  $stageRecordId: Long,
  $credentialId: Long,
  $storageKey: String!,
  $originalFilename: String,
  $mimeType: String,
  $sizeBytes: Long,
  $type: String,
  $description: String
) {
  createEvidence(input: {
    stageRecordId: $stageRecordId
    credentialId: $credentialId
    storageKey: $storageKey
    originalFilename: $originalFilename
    mimeType: $mimeType
    sizeBytes: $sizeBytes
    type: $type
    description: $description
  }) {
    success
    message
    code
  }
}
```

### Delete

```graphql
mutation DeleteEvidence($id: Long!) {
  deleteEvidence(input: { id: $id }) {
    success
    message
    code
  }
}
```

---

## 18. Value Records

Value records are **automatically created by the system** when a stage record is validated. They are immutable — do not attempt to create or modify them directly.

### Query

```graphql
query ValueRecords($batchId: Long) {
  valueRecords(where: { batchId: { eq: $batchId } }) {
    items {
      id
      organizationId
      batchId
      stageRecordId
      stageId
      actorId
      quantity
      unitPrice
      totalValue
      currency
      recordedAt
      validatedAt
    }
    totalCount
  }
}
```

---

## 19. Analytics

All analytics queries require `analytics.view` permission.

### Batch Status Summary

Count of batches grouped by status.

```graphql
query BatchStatusSummary {
  batchStatusSummary {
    status
    count
  }
}
```

### Value by Domain

Total value generated per supply chain domain.

```graphql
query ValueByDomain {
  valueByDomain {
    domainId
    domainName
    totalValue
    currency
  }
}
```

### Top Actors by Value

Actors ranked by total value they contributed.

```graphql
query TopActorsByValue($topN: Int!) {
  topActorsByValue(topN: $topN) {
    actorId
    actorName
    totalValue
    currency
    recordCount
  }
}
```

Default `topN` is 10.

### Stage Completion Rates

For a given domain, how far through the pipeline batches have progressed.

```graphql
query StageCompletionRates($domainId: Long!) {
  stageCompletionRates(domainId: $domainId) {
    stageId
    stageName
    sequence
    batchCount
    completionRate
  }
}
```

`completionRate` — percentage (0–100) of batches that have a validated record at this stage.

### Value Over Time

Monthly aggregation of value. Filter to a specific year or leave blank for all time.

```graphql
query ValueOverTime($year: Int) {
  valueOverTime(year: $year) {
    year
    month
    totalValue
    recordCount
  }
}
```

---

## 20. Audit Logs

Requires `audit.view` permission. Audit logs are immutable — they record every create/update/delete with a JSON diff.

```graphql
query AuditLogs($entityType: String, $entityId: Long) {
  auditLogs(
    where: {
      entityType: { eq: $entityType },
      entityId: { eq: $entityId }
    },
    order: [{ occurredAt: DESC }]
  ) {
    items {
      id
      organizationId
      userId
      userEmail
      entityType
      entityId
      action
      changes
      occurredAt
    }
    totalCount
  }
}
```

`action` values: `created`, `updated`, `deleted`

`changes` — JSON string with field-level diff: `{ "status": ["draft", "submitted"] }`

---

## 21. All Base Fields (Auditable)

Every entity (except `AuditLog`) includes these fields automatically:

```graphql
id: Long!
createdBy: String!
createdOn: DateTime!
updatedBy: String!
updatedOn: DateTime
revision: Int!
```

---

## 22. Entity Quick Reference

| Entity | Key Fields | Org-Scoped? |
|--------|-----------|-------------|
| `Organization` | name, slug, isActive | No |
| `User` | email, fullName, organizationId, isActive | Yes |
| `Role` | name, permissions[], isActive | No |
| `Domain` | organizationId, name, code, isActive | Yes |
| `Stage` | domainId, name, code, sequence, isRequired | Via domain |
| `StageField` | stageId, code, label, dataType, isRequired, fieldOrder | Via stage |
| `Product` | organizationId, domainId, name, sku, unitOfMeasure | Yes |
| `ActorRole` | organizationId, domainId, code, label, allowedStages[] | Yes |
| `Actor` | organizationId, roleId, name, externalId, contact (JSON), location (JSON) | Yes |
| `Credential` | actorId, type, issuer, reference, issuedAt, expiresAt | Yes |
| `Batch` | organizationId, domainId, productId, batchCode, status, totalValue | Yes |
| `BatchLineage` | parentBatchId, childBatchId, type (split/merge), quantity | Yes |
| `StageRecord` | batchId, stageId, actorId, quantity, unitPrice, status | Yes |
| `StageRecordField` | stageRecordId, stageFieldId, value | Via record |
| `Evidence` | stageRecordId, credentialId, storageKey, type | Yes |
| `ValueRecord` | batchId, stageRecordId, actorId, quantity, unitPrice, totalValue | Yes |
| `AuditLog` | entityType, entityId, action, changes (JSON), occurredAt | By orgId field |

---

## 23. JSONB Fields

Several fields store JSON as strings. Parse/stringify on the frontend as needed.

| Field | Entity | Shape |
|-------|--------|-------|
| `contact` | Actor | `{ phone, email, address }` |
| `location` | Actor | `{ lat, lng, region, district, community }` |
| `registrationMeta` | Actor | `{ idNumber, cooperativeMembership }` |
| `geoPoint` | StageRecord | `{ lat, lng, accuracy }` |
| `originLocation` | Batch | `{ lat, lng, region, district }` |
| `metadata` | Batch | Domain-specific arbitrary JSON |
| `validation` | StageField | `{ min, max, pattern, ... }` |
| `options` | StageField | `[{ value, label }, ...]` |
| `uiHint` | StageField | `{ widget, unit, ... }` |
| `allowedStages` | ActorRole | `[stageId, stageId, ...]` or `null` |
| `changes` | AuditLog | `{ fieldName: [oldValue, newValue], ... }` |

---

## 24. Typical User Flows

### Onboarding a New Organization
1. `createOrganization` — register the org
2. `createRole` — define roles with permissions
3. `createUser` — create admin user for the org
4. `createDomain` — define a supply chain type (e.g., "Cocoa")
5. `createStage` × N — define the pipeline steps
6. `createStageField` × N — define data capture forms per stage
7. `createActorRole` × N — define participant types
8. `createProduct` — register the product being tracked

### Registering a Batch Through the Pipeline
1. `createActor` — register the originating actor
2. `createBatch` — open a new batch
3. For each stage: `createStageRecord` → `submitStageRecord` → `validateStageRecord`
4. Validating creates a `ValueRecord` automatically
5. Attach evidence: `createEvidence` at any point

### Viewing Analytics
1. `batchStatusSummary` — dashboard overview
2. `valueByDomain` — revenue by supply chain
3. `topActorsByValue(topN: 10)` — leaderboard
4. `stageCompletionRates(domainId)` — pipeline funnel
5. `valueOverTime(year: 2024)` — monthly trend chart
