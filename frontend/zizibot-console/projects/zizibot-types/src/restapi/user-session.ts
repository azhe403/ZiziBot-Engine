export interface UserSession {
  isSessionValid: boolean;
  userName: string;
  photoUrl: string;
  userId: number;
  roleId: string;
  roleName: string;
  features: UserFeature[];
}

interface UserFeature {
  title: string;
  url: string;
  minimumRole: number;
}