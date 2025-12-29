export interface RolePermission {
  id: number;
  roleId: number;
  actionId: number;
  statusId: number;
  createdDate?: Date;
  createdBy?: number;
  updatedDate?: Date;
  updatedBy?: number;
}

export interface RolePermissionDto {
  id: number;
  roleId: number;
  actionId: number;
  statusId: number;
}