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

  tags = merge(
    local.default_tags,
    { BackupPolicy = "Dev" }
  )

  point_in_time_recovery {
    enabled = true
  }
}
