import apiClient from '../utils/http-client';
import { ApiResponse } from 'zizibot-contracts/src/restapi/api-response';
import { TelegramUserLogin, TelegramUserLoginResponse, UserSessionInfo } from 'zizibot-contracts/src/restapi/user-login';

export async function validateTelegramSession(data: TelegramUserLogin) {
  const response = await apiClient.post<ApiResponse<TelegramUserLoginResponse>>('/api/user/session/telegram', data);

  console.debug('auth telegram', response);

  return response.data;
}

export async function validateCurrentSession() {
  const response = await apiClient.post<ApiResponse<TelegramUserLoginResponse>>('/api/user/session/validate', {});
  return response.data;
}

export async function getUserInfo() {
  const response = await apiClient.get<ApiResponse<UserSessionInfo>>('/api/user/info');
  console.debug(getUserInfo.name, response.status);
  return response.data;
}