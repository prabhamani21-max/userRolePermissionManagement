export interface UserPermissionOverride {
  id: number;
  userId: number;
  actionId: number;
  isDenied: boolean;
  statusId: number;
}