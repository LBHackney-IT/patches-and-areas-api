# Patches and Areas API

The Patches and Areas API stores housing patch and area records and the people responsible for them. Callers can list patches, look one up by id, parent id, or name, and change those responsible entities. Replacing the responsible entities publishes `PatchOrAreaResEntityEditedEvent`. The API runs as an AWS Lambda behind API Gateway.

## Tech stack

- .NET 10 and ASP.NET Core
- AWS Lambda (`dotnet10`), API Gateway, DynamoDB, SNS, and X-Ray
- Serverless Framework for the Lambda deployment
- Terraform in this repository for the DynamoDB table
- CircleCI
- Docker Compose with DynamoDB Local and LocalStack for local SNS
- xUnit, FluentAssertions, Moq, and BDDfy
- Swashbuckle for Swagger

## Endpoints

| Verb | Route | Request | Response | Summary |
| --- | --- | --- | --- | --- |
| GET | `/api/v1/healthcheck/ping` | none | health-check report | Checks that the API and its DynamoDB dependency are available. |
| GET | `/api/v1/patch/all` | none | `List<PatchesResponseObject>` | Returns every patch record. |
| GET | `/api/v1/patch/{id}` | `PatchesQueryObject` | `PatchesResponseObject` | Returns one patch and sets an `ETag` from its version number. |
| GET | `/api/v1/patch` | `GetPatchByParentIdQuery` | `List<PatchEntity>` | Returns the patches for the supplied `parentId`. |
| GET | `/api/v1/patch/patchName/{patchName}` | `GetByPatchNameQuery` | `PatchEntity` | Returns the patch with the supplied name. |
| PATCH | `/api/v1/patch/{id}/responsibleEntity/{responsibileEntityId}` | `UpdatePatchesResponsibilityRequest`, `UpdatePatchesResponsibilitiesRequestObject` | none | Updates one responsible entity. `If-Match` carries the expected version. |
| PUT | `/api/v1/patch/{id}/responsibleEntities` | `PatchesQueryObject`, `List<ResponsibleEntities>` | none | Replaces the responsible entities. Callers must belong to a group in `ASSET_ADMIN_GROUPS`. Publishes `PatchOrAreaResEntityEditedEvent`. |
| DELETE | `/api/v1/patch/{id}/responsibleEntity/{responsibileEntityId}` | `DeleteResponsibilityFromPatchRequest` | none | Removes one responsible entity from the patch. |

`PATCH` and `PUT` return `204` when the update is stored, `404` when the patch is missing, and `409` when `If-Match` does not match the stored version. `GET` by parent id and by name return the domain entity. The end-to-end tests read that JSON as `PatchesResponseObject`.

## Linked repositories

| Repository | Summary |
| --- | --- |
| [patches-and-areas-shared](https://github.com/LBHackney-IT/patches-and-areas-shared) | Shared patch and area models, request and response types, and factories, published as `Hackney.Shared.PatchesAndAreas`. |
| [lbh-core](https://github.com/LBHackney-IT/lbh-core) | Shared libraries used here for DynamoDB, SNS, JWT, authorisation, validation, logging, middleware, HTTP context, and health checks. |

DynamoDB and the Lambda deployment live in this repository, under `terraform/` and `PatchesAndAreasApi/serverless.yml`. There is no `patches-and-areas-listener` repository.

## Data sources

| Source | Summary |
| --- | --- |
| DynamoDB table `PatchesAndAreas` | Stores patch records and their responsible entities. `PatchesGateway` reads and writes the table, including the parent-id and patch-name indexes. |
| Amazon SNS topic `patches-and-areas` | Receives `PatchOrAreaResEntityEditedEvent` when a patch's responsible entities are replaced. |

The API does not call another service over HTTP.

## Contacts

**Housing product team** at [mmh-project-team@hackney.gov.uk](mailto:mmh-project-team@hackney.gov.uk)

## Contributing

Build, test, and release steps are in [CONTRIBUTION.md](CONTRIBUTION.md).
