import apiClient from '../utils/http-client';
import { ApiResponse } from 'zizibot-contracts/src/restapi/api-response';
import { TelegramUserLogin, TelegramUserLoginResponse } from 'zizibot-contracts/src/restapi/user-login';

export async function validateTelegramSession(data: TelegramUserLogin) {
  const response = await apiClient.post<ApiResponse<TelegramUserLoginResponse>>('/api/user/session/telegram', data);

  console.debug('auth telegram', response);

  return response.data;
}