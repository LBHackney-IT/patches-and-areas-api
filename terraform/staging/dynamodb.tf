resource "aws_dynamodb_table" "PatchesAndAreasApi_dynamodb_table" {
  name           = "PatchesAndAreas"
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
  
  attribute {
    name = "name"
    type = "S"
  }
  
  global_secondary_index {
    name               = "PatchByParentId"
    hash_key           = "parentId"
    write_capacity     = 10
    read_capacity      = 10
    projection_type    = "ALL"
  }

  global_secondary_index {
    name               = "PatchByPatchName"
    hash_key           = "name"
    write_capacity     = 10
    read_capacity      = 10
    projection_type    = "ALL"
  }

  tags = merge(
    local.default_tags,
    { BackupPolicy = "Stg" }
  )

  point_in_time_recovery {
    enabled = true
  }
}
