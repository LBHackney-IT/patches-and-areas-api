resource "aws_dynamodb_table" "patchesapi_dynamodb_table" {
  name           = "Patches"
  billing_mode   = "PROVISIONED"
  read_capacity  = 10
  write_capacity = 10
  hash_key       = "id"

  attribute {
    name = "id"
    type = "S"
  }

  local_secondary_index {
    name            = "patchesParentId"
    hash_key       = "ParentId"
    projection_type = "ALL"
  }

  tags = merge(
    local.default_tags,
    { BackupPolicy = "Dev" }
  )

  point_in_time_recovery {
    enabled = true
  }
}