export interface TelegramUserLogin {
  id?: string;
  first_name?: string;
  last_name?: string;
  username?: string;
  photo_url?: string;
  auth_date?: string;
  hash?: string;
  session_id?: string;
}

export interface TelegramUserLoginResponse {
  isSessionValid: boolean;
  bearerToken: string;
}

export interface UserSessionInfo {
  isSessionValid: boolean;
  userName: string;
  userId: number;
  name: string;
}