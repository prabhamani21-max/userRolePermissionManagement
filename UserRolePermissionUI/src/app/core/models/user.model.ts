export interface CreateUser {
  name: string;
  email: string;
  password: string;
  defaultRoleId: number;
  contactNo: string;
  profileImage?: string;
}

// for updating user
export interface UserModel {
  id: number;
  name: string;
  email: string;
  password: string;
 defaultRoleId: number;
  gender: number;
  dob: string;
  contactNo: string;
  profileImage?: string;
  statusId: number;
}
