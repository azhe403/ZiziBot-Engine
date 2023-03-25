export interface DashboardSession {
    userId: number;
    isSessionValid: boolean;
    roleId: string;
    roleName: string;
    bearerToken: string;
}