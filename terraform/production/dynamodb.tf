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
  
  attribute {
    name = "parentId"
    type = "S"
  }
  global_secondary_index {
    name               = "PatchByParentId"
    hash_key           = "parentId"
    write_capacity     = 10
    read_capacity      = 10
    projection_type    = "ALL"
  }

  tags = merge(
    local.default_tags,
    { BackupPolicy = "Prod" }
  )

  point_in_time_recovery {
    enabled = true
  }
}