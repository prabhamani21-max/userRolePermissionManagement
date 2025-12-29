// src/app/core/constants/claim-types.ts
import { JwtPayload } from 'jwt-decode';

/**
 * JWT Claim Type Constants
 */
export const ClaimTypes = {
  NAME_IDENTIFIER:
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
  EMAIL: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
  NAME: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
  ROLE: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  EXPIRATION: 'exp',
} as const;

/**
 * Custom JWT Payload with strongly-typed claims
 */
export interface CustomJwtPayload extends JwtPayload {
  [ClaimTypes.NAME_IDENTIFIER]: string;
  [ClaimTypes.EMAIL]: string;
  [ClaimTypes.NAME]: string;
  [ClaimTypes.ROLE]: string;
  [ClaimTypes.EXPIRATION]: number;
  RoleId: string;
}
