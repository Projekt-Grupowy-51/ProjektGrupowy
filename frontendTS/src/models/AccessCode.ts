export interface AccessCode {
  code: string;
  createdAtUtc: string;
  expiresAtUtc?: string | null;
  isValid: boolean;
}
