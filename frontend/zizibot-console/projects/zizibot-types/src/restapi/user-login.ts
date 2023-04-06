export interface TelegramUserLogin {
  id: number;
  first_name?: string;
  last_name?: string;
  username?: string
  photo_url?: string
  auth_date: number;
  hash: string;
  sessionId: string;
}