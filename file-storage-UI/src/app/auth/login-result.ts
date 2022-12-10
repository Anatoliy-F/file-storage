export interface LoginResult {
    success: boolean;
    message: string;
    token?: string;
    isAdmin: boolean;
}