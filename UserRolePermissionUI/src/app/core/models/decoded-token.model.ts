// src/app/core/models/decoded-token.model.ts
export interface DecodedToken {
  userId: string;
  email: string;
  name: string;
  role: string;
  exp: number;
  roleId: string;
  [key: string]: any; // For any additional claims
}
